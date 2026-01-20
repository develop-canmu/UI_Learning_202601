using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Colosseum;
using Pjfb.Community;
using Pjfb.Extensions;
using Pjfb.LeagueMatch;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.SystemUnlock;
using Pjfb.UserData;
using TMPro;
using UnityEngine;

namespace Pjfb.ClubRoyal
{
    public class ClubRoyalTopPage : Page
    {
        public class Param
        {
            private LeagueMatchInfo matchInfo = null;
            public LeagueMatchInfo MatchInfo => matchInfo;

            public Param(LeagueMatchInfo matchInfo)
            {
                this.matchInfo = matchInfo;
            }
        }

        // メニューバッチ
        [SerializeField] private UIBadgeNotification menuBadge;
        // 開催期間
        [SerializeField] private TMP_Text leagueDateTerm;
        // シーズン戦期間中の表示
        [SerializeField] private ClubRoyalSeasonBattleView seasonBattleView;
        // 入れ替え戦期間中の表示
        [SerializeField] private ClubRoyalShiftBattleSeasonView shiftBattleSeasonView;
        // 入場用ボタン
        [SerializeField] private ClubRoyalEntryButton entryButton;
        // 自動配置On
        [SerializeField] private GameObject autoFormationSettingOn;
        // 自動配置Off
        [SerializeField] private GameObject autoFormationSettingOff;
        
        // 更新をかける時間
        private const float updateTimer = 0.5f;

        private float timer = 0f;
        
        // メニューバッチの条件
        private bool showMenuBadge => CommunityManager.ShowClubChatBadge;
        
        // 対戦表の情報
        private GroupLeagueMatchBoardInfo boardInfo = null;
        private GroupLeagueMatchGroupStatusDetail groupStatusDetail = null;

        private LeagueMatchInfo matchInfo => param.MatchInfo;
        
        // キャッシュ
        private Param param = null;

        // 次に画面更新をかける時間
        private DateTime nextUpdateTime = DateTime.MinValue;
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            param = (Param)args;
            
            // メニューバッチの表示
            menuBadge.SetActive(showMenuBadge);

            // データがないならエラーを出す
            if (matchInfo?.SeasonData == null)
            {
                CruFramework.Logger.LogError("クラブロワイヤルのシーズンデータが見つかりません");
                return;
            }
            
            // Viewを更新
            await UpdateView(true);
        }

        protected override async UniTask OnOpen(object args)
        {
            // 遊び方表示
            CheckShowHowToPlay().Forget();
            await base.OnOpen(args);
        }

        private void Update()
        {
            // タイマーがセットされてないなら更新しない
            if (timer <= 0)
            {
                return;
            }
         
            timer -= Time.deltaTime;
            // セットされた時間が経過した
            if (timer <= 0)
            {
                UpdateView().Forget();
            }
        }

        //// <summary> 時間経過によるViewの更新 </summary>
        private async UniTask UpdateView(bool isForceUpdate = false)
        {
            // 時間での更新を止める
            timer = 0f;
            
            // データがないなら更新しない
            if (matchInfo == null)
            {
                return;
            }

            // シーズンデータがない場合も更新なし
            if (matchInfo.SeasonData == null)
            {
                return;
            }

            // 更新時間を超えた時か強制更新の場合
            if (nextUpdateTime.IsPast(AppTime.Now) || isForceUpdate)
            {
                // シーズンデータ
                ColosseumSeasonData seasonData = matchInfo.SeasonData;
                // 対戦表の情報を更新
                await UpdateBoardInfo();
                
                // 開始時間
                DateTime startAt = seasonData.SeasonHome.startAt.TryConvertToDateTime();
                // 終了時間
                DateTime endAt = seasonData.SeasonHome.endAt.TryConvertToDateTime();
                // 開催期間を表示
                leagueDateTerm.text = string.Format(StringValueAssetLoader.Instance["club-royal.event_period"], startAt.GetNewsDateTimeString(), endAt.GetNewsDateTimeString());

                // クラブロワイヤルのスケジュールを取得
                ClubRoyalManager.Schedule schedule = ClubRoyalManager.GetSchedule(matchInfo);
                
                // 入れ替え戦期間中の表示
                if (matchInfo.IsOnShiftBattle)
                {
                    seasonBattleView.gameObject.SetActive(false);
                    shiftBattleSeasonView.gameObject.SetActive(true);
                    
                    // 入れ替え戦期間中の表示をセット
                    await shiftBattleSeasonView.SetView(matchInfo, boardInfo);
                    
                }
                // シーズン戦期間中の表示(入れ替え戦前)
                else
                {
                    seasonBattleView.gameObject.SetActive(true);
                    shiftBattleSeasonView.gameObject.SetActive(false);
                    
                    await seasonBattleView.SetView(matchInfo, boardInfo);
                }

                // 入場ボタンの表示
                entryButton.Init(matchInfo, boardInfo, schedule);
                // 現在の自動配置設定のラベル表示
                SetAutoFormationSettingLabel();
                
                // 次に更新をかける時間をセット
                nextUpdateTime = schedule.NextUpdateTime;
            }
            
            // 残り時間を更新(これは一定時間毎に実行する)
            entryButton.UpdateRemainingTime();
            
            // 更新タイマーセット
            timer = updateTimer;
        }

        //// <summary> 対戦表情報を更新 </summary>
        private async UniTask UpdateBoardInfo()
        {
            // 対戦表のデータを取得
            ColosseumGetGroupLeagueBoardAPIResponse leagueBoardData = await LeagueMatchUtility.GetLeagueBoardInfo(matchInfo.SeasonData.SeasonId);
            boardInfo = leagueBoardData.boardInfo;
            groupStatusDetail = boardInfo.groupStatusDetailSelf;
        }

        //// <summary> 自動配置の設定ラベル表示 </summary>
        private void SetAutoFormationSettingLabel()
        {
            // 自動配置設定がされているか
            bool isSetAutoFormation = boardInfo.battleGameliftSetting.teamPlacement.spotIndexList.Length > 0;
            autoFormationSettingOn.SetActive(isSetAutoFormation);
            autoFormationSettingOff.SetActive(isSetAutoFormation == false);
        }

        //// <summary> メニューボタンクリック時の処理 </summary>
        public void OnClickMenuButton()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubRoyalMenu, new ClubRoyalMenuModalWindow.ClubRoyalInfo(param.MatchInfo, groupStatusDetail));
        }

        public void OnClickAutoFormationSettingModal()
        {
            OnClickAutoFormationSettingModalAsync().Forget();
        }
        
        //// <summary> 自動配置設定モーダルを開く </summary>
        private async UniTask OnClickAutoFormationSettingModalAsync()
        {
            // 現在、自動配置の変更が出来ない場合、配置を変更出来ない旨を表示する
            if (ClubRoyalManager.CanChangeAutoFormationSetting(param.MatchInfo) == false)
            {
                ClubRoyalManager.OpenCantChangeAutoFormationModal(param.MatchInfo).Forget();
                return;
            }

            CruFramework.Page.ModalWindow modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.ClubRoyalAutoFormation, new ClubRoyalAutoFormationModalWindow.Param(matchInfo, boardInfo));

            // 自動配置設定が更新されたか
            bool isUpdate = (bool)await modal.WaitCloseAsync();
            if (isUpdate)
            {
                // 自動配置が更新されたので対戦表の情報も更新
                await UpdateBoardInfo();
                // 強制更新をかける
                UpdateView(true).Forget();
            }
        }
        
        // 初回のみ遊び方を表示
        private async UniTask CheckShowHowToPlay()
        {
            // クラブロワイヤルのチュートリアル
            long tutorialId = (long)HowToPlayUtility.TutorialType.ClubRoyal;
            if (LocalSaveManager.saveData.tutorialIdConfirmList.Contains(tutorialId) == false)
            {
                CruFramework.Page.ModalWindow howToModal = await HowToPlayUtility.OpenHowToPlayModal(tutorialId, StringValueAssetLoader.Instance["club-royal.tutorial.title"]);
                await howToModal.WaitCloseAsync(this.GetCancellationTokenOnDestroy());
                LocalSaveManager.saveData.tutorialIdConfirmList.Add(tutorialId);
                LocalSaveManager.Instance.SaveData();
            }
            
            // アドバイザーチュートリアル
            // アドバイザーが解放されているかを確認
            if(UserDataManager.Instance.IsUnlockSystem((long)SystemUnlockDataManager.SystemUnlockNumber.TrainingAdviser) == false) return;
            tutorialId = (long)HowToPlayUtility.TutorialType.AdviserClubRoyalTop;
            if (LocalSaveManager.saveData.tutorialIdConfirmList.Contains(tutorialId) == false)
            {
                CruFramework.Page.ModalWindow howToModal = await HowToPlayUtility.OpenHowToPlayModal(tutorialId, StringValueAssetLoader.Instance["character.detail_modal.adviser.title"]);
                await howToModal.WaitCloseAsync(this.GetCancellationTokenOnDestroy());
                LocalSaveManager.saveData.tutorialIdConfirmList.Add(tutorialId);
                LocalSaveManager.Instance.SaveData();
            }
        }
    }
}
