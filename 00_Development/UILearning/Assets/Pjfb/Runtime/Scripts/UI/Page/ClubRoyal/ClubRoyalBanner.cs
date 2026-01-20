using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pjfb.Club;
using Pjfb.Colosseum;
using Pjfb.Extensions;
using Pjfb.LeagueMatch;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.ClubRoyal
{
    public class ClubRoyalBanner : LeagueMatchBannerBase
    {
        /// <summary> バナー側の表示ステート </summary>
        private enum BannerState
        {
            //// <summary> 準備中 </summary>
            InPreparation,
            //// <summary> 試合準備中 </summary>
            BeforeEntry,
            //// <summary> 試合準備受付中 </summary>
            BattleEntry,
            //// <summary> 試合開催中 </summary>
            BattleStart,
            //// <summary> 本日の試合終了 </summary>
            BattleEnd,
            //// <summary> 全試合終了 </summary>
            AllBattleEnd,
            //// <summary> 参加不可 </summary>
            UnableParticipate,
            //// <summary> 入れ替え戦参加決定 </summary>
            ShiftBattleBeforeEntry,
            //// <summary> 入れ替え戦試合準備受付中 </summary>
            ShiftBattleEntry,
            //// <summary> 入れ替え戦試合開催中 </summary>
            ShiftBattleStart,
            //// <summary> 入れ替え戦終了 </summary>
            ShiftBattleEnd,
            //// <summary> 階級昇格 </summary>
            RankUp,
            //// <summary> 階級降格 </summary>
            RankDown,
            //// <summary> 階級維持 </summary>
            RankKeep,
        }

        //// <summary> 開催状況表示バッジ </summary>
        [Serializable]
        private class StateBadge
        {
            [SerializeField] private BannerState state;
            [SerializeField] private Image badgeImage;
            public BannerState State => state;
            public Image BadgeImage => badgeImage;
        }
        
        // バナー表示Image
        [SerializeField]
        private Image bannerBaseImage;

        // バナー活性化時のSprite画像
        [SerializeField] 
        private Sprite activateSprite;

        // バナー非活性時のSprite画像
        [SerializeField] 
        private Sprite deactivateSprite;

        // 開催状況毎に表示するバッジリスト
        [SerializeField] 
        private StateBadge[] stateBadges;
        
        // 期間表示テキスト
        [SerializeField] 
        private TMP_Text termText;

        // 次のスケジュールまでの残り時間テキスト
        [SerializeField] 
        private TMP_Text nextScheduleRemainingText;
        
        // 更新するかどうか
        private bool isUpdateView = true;
        // 現在のクラブロワイヤルのスケジュール
        private ClubRoyalManager.Schedule schedule;

        // 残り時間表示フォーマット
        private string remainingStringFormat = "";

        //// <summary> リーグマッチとマッチング失敗条件が違うので </summary>
        protected override bool IsMatchingFailed
        {
            get
            {
                // データがない場合はベース側の条件で吸収される       
                if (base.IsMatchingFailed)
                {
                    return true;
                }

                // シーズン情報に登録されているギルドIdと一致していない場合もマッチング失敗に
                if (leagueMatchInfo.SeasonData.UserSeasonStatus.groupSeasonStatus.groupId != UserDataManager.Instance.user.gMasterId)
                {
                    return true;
                }

                return false;
            }
        }

        //// <summary> 開催状況によって表示を設定 </summary>
        public override void SetView(LeagueMatchInfo matchInfo)
        {
            isUpdateView = true;
            
            leagueMatchInfo = matchInfo;
            
            // クラブロワイヤルが設定されていない
            if (matchInfo == null)
            {
                // 未開催でもmatchInfoは取得できるので取得出来なかった場合はエラーログを出す
                CruFramework.Logger.LogError("ClubRoyalの開催状況が見つかりません");
                return;
            }
            
            // 開催中の期間を表示
            if(IsOnSeason)
            {
                string startAt = leagueMatchInfo.SeasonStartAt.GetDateTimeString();
                string endAt = leagueMatchInfo.SeasonEndAt.GetDateTimeString();
                termText.gameObject.SetActive(true);
                termText.text = string.Format(StringValueAssetLoader.Instance["club-royal.banner.term"], startAt, endAt);
            }
            // 未開催の場合、期間は非表示
            else
            {
                termText.gameObject.SetActive(false);
            }
            
            // 開催状況を更新
            UpdateState();
        }

        //// <summary> 時間経過したときのバナーの更新 </summary>
        protected override void UpdateView()
        {
            // データがない
            if (leagueMatchInfo == null)
            {
                return;
            }
            // 更新しない
            if (isUpdateView == false)
            {
                return;
            }
            
            // Stateが変わる時間になったなら更新する
            if (schedule.NextUpdateTime.IsPast(AppTime.Now))
            {
                UpdateState();
            }
            
            // 残り時間を表示するなら
            if (nextScheduleRemainingText.gameObject.activeSelf)
            {
                // 次のスケジュールまでの残り時間
                string remainingString = schedule.NextUpdateTime.GetRemainingString(AppTime.Now);
                // 現在のフォーマットに合わして残り時間テキストをセット
                nextScheduleRemainingText.text = string.Format(remainingStringFormat, remainingString);
            }
        }

        private void UpdateState()
        {
            // 現在のスケジュールを取得
            schedule = ClubRoyalManager.GetSchedule(leagueMatchInfo);
            
            // シーズン中参加不可
            // マッチングに失敗時、クラブ未参加の時は参加不可
            if (IsOnSeason && (IsMatchingFailed || IsJoinedClub == false))
            {
                // 参加不可表示に
                SetStateBadge(BannerState.UnableParticipate);
                SetBannerBase(false);
                // 次のスケジュールまでの残り時間表示をオフ
                nextScheduleRemainingText.gameObject.SetActive(false);
                
                // クラブ未参加の時はビュー更新をしないように(時間経過で更新されることがないので)
                if (IsJoinedClub == false)
                {
                    isUpdateView = false;
                }
                return;
            }
            
            // 準備中以外はバナーをアクティブに
            SetBannerBase(schedule.State != ClubRoyalManager.State.InPreparation);
            // 準備中、試合開催中以外は残り時間を表示
            nextScheduleRemainingText.gameObject.SetActive(schedule.State != ClubRoyalManager.State.InPreparation && schedule.State != ClubRoyalManager.State.BattleStart);
            
            // Stateに応じてバナーの見た目を変える
            switch (schedule.State)
            {
                // 準備中
                case ClubRoyalManager.State.InPreparation:
                {
                    SetStateBadge(BannerState.InPreparation);
                    break;
                }
                // 試合準備中
                case ClubRoyalManager.State.BeforeEntry:
                {
                    // 入れ替え戦なら
                    if (IsOnShiftBattle)
                    {
                        SetStateBadge(BannerState.ShiftBattleBeforeEntry);
                    }
                    else
                    {
                        SetStateBadge(BannerState.BeforeEntry);
                    }

                    // 入場開始まで
                    remainingStringFormat = StringValueAssetLoader.Instance["club-royal.banner.remaining-time.entry"];
                    break;
                }
                // 試合準備受付中
                case ClubRoyalManager.State.BattleEntry:
                {
                    // 入れ替え戦なら
                    if (IsOnShiftBattle)
                    {
                        SetStateBadge(BannerState.ShiftBattleEntry);
                    }
                    else
                    {
                        SetStateBadge(BannerState.BattleEntry);
                    }

                    // 試合開始まで
                    remainingStringFormat = StringValueAssetLoader.Instance["club-royal.banner.remaining-time.battle_start"];
                    break;
                }
                // 試合開催中
                case ClubRoyalManager.State.BattleStart:
                {
                    // 入れ替え戦なら
                    if (IsOnShiftBattle)
                    {
                        SetStateBadge(BannerState.ShiftBattleStart);
                    }
                    else
                    {
                        SetStateBadge(BannerState.BattleStart);
                    }
                    
                    break;
                }
                // 本日の試合終了
                case ClubRoyalManager.State.BattleEnd:
                {
                    // 入れ替え戦なら
                    if (IsOnShiftBattle)
                    {
                        SetStateBadge(BannerState.ShiftBattleEnd);
                    }
                    else
                    {
                        SetStateBadge(BannerState.BattleEnd);
                    }
                    
                    // 次の対戦相手決定まで
                    remainingStringFormat = StringValueAssetLoader.Instance["club-royal.banner.remaining-time.next_battle_decision"];

                    break;
                }
                // 全試合終了
                case ClubRoyalManager.State.AllBattleEnd:
                {
                    ColosseumGroupSeasonStatus seasonStatus = leagueMatchInfo?.SeasonData?.UserSeasonStatus?.groupSeasonStatus;
                    // 成績があるか
                    bool hasResult = false;

                    // 入れ替え戦への参加があるなら入れ替え戦の結果が出ているかで判断する
                    if (leagueMatchInfo.CanShiftBattle)
                    {
                        long result = 0;
                        if (seasonStatus != null)
                        {
                            result = seasonStatus.shiftMatchInfo.result;
                        }
                        hasResult = ColosseumManager.HasResult(result);
                    }
                    // 入れ替え戦の参加がない場合はシーズン終了後のグレードに値が入ってきているかで判断する
                    else
                    {
                        hasResult = seasonStatus?.gradeAfter > 0;
                    }
                    
                    // 成績あり
                    if (hasResult)
                    {
                        // 階級昇格
                        if (seasonStatus.gradeAfter > seasonStatus.gradeBefore)
                        {
                            SetStateBadge(BannerState.RankUp);
                        }
                        // 階級降格
                        else if (seasonStatus.gradeAfter < seasonStatus.gradeBefore)
                        {
                            SetStateBadge(BannerState.RankDown);
                        }
                        // 階級維持
                        else
                        {
                            SetStateBadge(BannerState.RankKeep);
                        }
                    }
                    // まだ成績がない
                    else
                    {
                        // 入れ替え戦なら
                        if (IsOnShiftBattle)
                        {
                            // まだ成績が出てないなら入れ替え戦終了表示
                            SetStateBadge(BannerState.ShiftBattleEnd);
                        }
                        // 全試合終了
                        else
                        {
                            SetStateBadge(BannerState.AllBattleEnd);
                        }
                    }
                    
                    // 次回開催まで
                    remainingStringFormat = StringValueAssetLoader.Instance["club-royal.banner.remaining-time.next_season"];

                    break;
                }
            }
        }

        //// <summary> バナーStateと一致するImageを返す </summary>
        private Image GetBadgeStateImage(BannerState bannerState)
        {
            foreach (StateBadge badge in stateBadges)
            {
                if (badge.State == bannerState)
                {
                    return badge.BadgeImage;
                }
            }

            return null;
        }
        
        //// <summary> 開催状況表示用のバッジの表示 </summary>
        private void SetStateBadge(BannerState bannerState)
        {
            HideAllStateBadge();
            Image badgeStateImage = GetBadgeStateImage(bannerState);

            if (badgeStateImage != null)
            {
                // 新しく表示するバッジに切り替える
                badgeStateImage.gameObject.SetActive(true);
            }
        }

        //// <summary> 全開催状況バッジを非表示 </summary>
        private void HideAllStateBadge()
        {
            // 全てのバッジを非表示
            foreach (StateBadge badge in stateBadges)
            {
                badge.BadgeImage.gameObject.SetActive(false);
            }
        }

        //// <summary> バナー画像のアクティブを切り替え </summary>
        private void SetBannerBase(bool isActive)
        {
            if (isActive)
            {
                // バナー画像を活性時のものに
                bannerBaseImage.sprite = activateSprite;
            }
            else
            {
                // バナー画像を非活性時のものに
                bannerBaseImage.sprite = deactivateSprite;
            }
        }

        //// <summary> APIを実行して最新のデータを取得して更新 </summary>
        private async UniTask RefreshMatchInfo()
        {
            // API実行
            ColosseumGetHomeDataAPIRequest request = new ColosseumGetHomeDataAPIRequest();
            ColosseumGetHomeDataAPIPost post = new ColosseumGetHomeDataAPIPost();
            post.getTurn = 0;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            // ビュー更新
            SetView(LeagueMatchUtility.GetLeagueMatchInfo(leagueMatchInfo.MColosseumEvent.clientHandlingType));
        }
        
        //// <summary> バナークリック時の処理 </summary>
        public void OnClick()
        {
            // クラブロワイヤルが設定されていない
            if (leagueMatchInfo == null)
            {
                return;
            }

            // 同ページのクラブロワイヤルからは遷移できないように
            if (AppManager.Instance.UIManager.PageManager.CurrentPageType == PageType.ClubRoyal)
            {
                return;
            }

            // クラブに参加していない
            if (IsJoinedClub == false)
            {
                // クラブ機能が解放されてないクラブ未開放のモーダルを表示
                if(UserDataManager.Instance.IsUnlockSystem(ClubUtility.clubLockId) == false)
                {
                    ClubUtility.OpenClubLockModal();
                    return;
                }
                
                // 解放されているならクラブ所属を促すモーダルを表示
                // OKボタンはクラブ検索画面に飛ばす
                ConfirmModalButtonParams positiveButton = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["club.find"], (m) =>
                {
                    // 未所属なら自動でクラブ検索ページに遷移するので
                    AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Club, true, null);
                    m.Close();
                });
                ConfirmModalButtonParams negativeButton = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], (m)=>m.Close());
                string title = StringValueAssetLoader.Instance["common.join_condition"];
                string message = StringValueAssetLoader.Instance["club-royal.club-join_conditions_modal.message"];
                ConfirmModalData data = new ConfirmModalData(title, message, string.Empty, positiveButton, negativeButton);
            
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
                return;
            }

            // 準備期間中なら準備期間中である旨をモーダルで表示
            if (schedule.State == ClubRoyalManager.State.InPreparation)
            {
                ConfirmModalButtonParams negativeButton = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], (m)=>m.Close());
                string title = StringValueAssetLoader.Instance["club-royal.in-preparation_modal.title"];
                string message = StringValueAssetLoader.Instance["club-royal.in-preparation_modal.message"];
                ConfirmModalData data = new ConfirmModalData(title, message, string.Empty, negativeButton);
            
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
                return;
            }
            
            // シーズン中
            
            // シーズンが開始したならデータ更新をするモーダルを表示
            // シーズン中でシーズンデータがないならAPIから取得
            if (leagueMatchInfo.SeasonData == null)
            {
                ConfirmModalData modalData = new ConfirmModalData();
                modalData.Title = StringValueAssetLoader.Instance["common.update"];
                modalData.Message = StringValueAssetLoader.Instance["club-match.need_update_message"];
                modalData.NegativeButtonParams = new ConfirmModalButtonParams(
                    StringValueAssetLoader.Instance["common.executeUpdate"],
                    async window =>
                    {
                        // API実行
                        await RefreshMatchInfo();
                        // ウィンドウ閉じる
                        window.Close();
                    }
                );
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, modalData);
                return;
            }
            
            // マッチングに失敗した場合は次回参加するように促すモーダルを表示
            if (IsMatchingFailed)
            {
                ConfirmModalButtonParams positiveButton = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["character.deck_edit"], (m) =>
                {
                    AppManager.Instance.UIManager.PageManager.OpenPage(PageType.ClubRoyalDeck, true, null);
                    m.Close();
                });
                ConfirmModalButtonParams negativeButton = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], (m)=>m.Close());
                string title = StringValueAssetLoader.Instance["club-royal.participation_conditions_modal.title"];
                string message = StringValueAssetLoader.Instance["club-royal.participation_conditions_modal.message"];
                ConfirmModalData data = new ConfirmModalData(title, message, string.Empty, positiveButton, negativeButton);
            
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
                return;
            }
            
            ClubRoyalTopPage.Param param = new ClubRoyalTopPage.Param(leagueMatchInfo);
            // クラブロワイヤルページに移動
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.ClubRoyal, true, param);
        }
    }
}