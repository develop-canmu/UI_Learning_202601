using System;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.InGame.ClubRoyal;
using Pjfb.LeagueMatch;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.ClubRoyal
{
    //// <summary> クラブロワイヤル入場ボタン </summary>
    public class ClubRoyalEntryButton : MonoBehaviour
    {
        // 入場ボタンのルートオブジェクト
        [SerializeField] private GameObject entryButtonRoot;
        // クラブロワイヤル入場イメージ表示オブジェクト
        [SerializeField] private Image entryBaseImage;
        // 入場画像
        [SerializeField] private Sprite entrySprite;
        // 対戦中画像
        [SerializeField] private Sprite inBattleSprite;
        // 残り時間表示テキスト
        [SerializeField] private TMP_Text entryLimitText;
        // アナウンスオブジェクト
        [SerializeField] private GameObject announceObjectRoot;
        // ボタン非活性時のアナウンステキスト
        [SerializeField] private TMP_Text announceText;
        
        // 次のスケジュール通知オブジェクト
        private TMP_Text nextScheduleAnnounceTextObject;

        private LeagueMatchInfo matchInfo;
        private GroupLeagueMatchBoardInfo boardInfo;
        private ClubRoyalManager.Schedule schedule;

        // 入場ボタンが表示されているか
        private bool isShowEntryButton = false;

        // 次のスケジュールまでの表示テキスト
        private string nextScheduleAnnounceText = string.Empty;

        // 次のスケジュールまでの時間
        private DateTime nextScheduleTime = DateTime.MaxValue;
        
        //// <summary> 表示を設定 </summary>
        public void Init(LeagueMatchInfo matchInfo, GroupLeagueMatchBoardInfo boardInfo, ClubRoyalManager.Schedule schedule)
        {
            // いったん条件洗い出し
            this.matchInfo = matchInfo;
            this.boardInfo = boardInfo;
            this.schedule = schedule;
            
            // 開催状況によってボタンの表示を変える
            SetButtonImage(schedule.State);
            
            // 次のスケジュールまでの時間表示に使う日時をセット
            nextScheduleTime = schedule.NextScheduleTime;
            
            switch (schedule.State)
            {
                // 入場前
                case ClubRoyalManager.State.BeforeEntry:
                {
                    // 入場可能まで・・
                    nextScheduleAnnounceText = StringValueAssetLoader.Instance["club-royal.announce.remaining-time.entry"];
                    break;
                }
                // 入場可能
                case ClubRoyalManager.State.BattleEntry:
                {
                    // 入れ替え戦の参加があるなら
                    if (matchInfo.CanShiftBattle)
                    {
                        // 入れ替え戦まで・・・
                        nextScheduleAnnounceText = StringValueAssetLoader.Instance["club-royal.announce.remaining-time.next_shift-battle-start"];
                    }
                    else
                    {
                        // 試合開始まであと・・・ 
                        nextScheduleAnnounceText = StringValueAssetLoader.Instance["club-royal.announce.remaining-time.battle_start"];
                    } 
                    break;
                }
                // 対戦中
                case ClubRoyalManager.State.BattleStart:
                {
                    //　試合終了まで・・・ 
                    nextScheduleAnnounceText = StringValueAssetLoader.Instance["club-royal.announce.remaining-time.battle_end"];
                    break;
                }
                // 本日の試合終了
                case ClubRoyalManager.State.BattleEnd:
                {
                    // 次の対戦相手決定まで・・・
                    nextScheduleAnnounceText = StringValueAssetLoader.Instance["club-royal.announce.remaining-time.next_battle_decision"];
                    break;
                }
                // 全試合終了(入れ替え戦以外が終了したときも含む)
                case ClubRoyalManager.State.AllBattleEnd:
                {
                    LeagueMatchInfoSchedule nextBattleSchedule = matchInfo.GetNextSchedule(LeagueMatchInfoSchedule.Schedule.Battle);
                    // 入れ替え戦の参加がある
                    if (matchInfo.CanShiftBattle && nextBattleSchedule != null)
                    {
                        // 入れ替え戦までの時間を表示するのでスケジュールの日時を変更
                        nextScheduleTime = matchInfo.GetNextSchedule(LeagueMatchInfoSchedule.Schedule.Battle).StartAt;
                        // 入れ替え戦まで・・・
                        nextScheduleAnnounceText = StringValueAssetLoader.Instance["club-royal.announce.remaining-time.next_shift-battle-start"];
                    }
                    // 次回クラブロワイヤルまで
                    else
                    {
                        nextScheduleAnnounceText = StringValueAssetLoader.Instance["club-royal.announce.remaining-time.next_season"];
                    }
                    break;
                }
                case ClubRoyalManager.State.InPreparation:
                {
                    // 次回クラブロワイヤルまで
                    nextScheduleAnnounceText = StringValueAssetLoader.Instance["club-royal.announce.remaining-time.next_season"];
                    break;
                }
            }
            
            // 残り時間を更新
            UpdateRemainingTime();
        }

        //// <summary> 残り時間を更新する </summary>
        public void UpdateRemainingTime()
        {
            // 残り時間
            string remainingString = nextScheduleTime.GetRemainingString(AppTime.Now);
            // 表示フォーマットと残り時間を組み合わせて文言を作成
            string text = string.Format(nextScheduleAnnounceText, remainingString);
            
            // 表示されているオブジェクトによって残り時間の表示先を変える
            if (isShowEntryButton)
            {
                entryLimitText.text = text;
            }
            else
            {
                announceText.text = text;
            }
        }

        //// <summary> 入場ボタンの表示 </summary>
        private void SetButtonImage(ClubRoyalManager.State state)
        {
            // 入場可能, 試合中の時は表示し、それ以外の時はアナウンスオブジェクトを表示する
            isShowEntryButton = state == ClubRoyalManager.State.BattleEntry || state == ClubRoyalManager.State.BattleStart;
            
            gameObject.SetActive(true);
            announceObjectRoot.SetActive(isShowEntryButton == false);
            entryButtonRoot.SetActive(isShowEntryButton);
            
            // 表示する画像を切り替え
            // 入場中
            if (state == ClubRoyalManager.State.BattleEntry)
            {
                entryBaseImage.sprite = entrySprite;
            }
            // 試合中
            else if (state == ClubRoyalManager.State.BattleStart)
            {
                entryBaseImage.sprite = inBattleSprite;
            }
        }

        //// <summary> インゲームへ遷移 </summary>
        private async UniTask MoveToClubRoyalInGame()
        {
            var request = new BattleGameliftJoinBattleAPIRequest();
            var post = new BattleGameliftJoinBattleAPIPost();
            post.id = boardInfo.todayMatch.inGameMatchId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            
            var inGameOpenArgs = new ClubRoyalInGameOpenArgs(
                response.connection.dnsName,
                response.connection.port,
                boardInfo.todayMatch.inGameMatchId);
            
            AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.ClubRoyalInGame, false, inGameOpenArgs).Forget();
        }

        //// <summary> 全試合終了時のモーダル表示 </summary>
        private void OpenAllBattleFinishModal()
        {
            ConfirmModalButtonParams positiveButton = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["character.deck_edit"], (m) =>
            {
                AppManager.Instance.UIManager.PageManager.OpenPage(PageType.ClubRoyalDeck, true, null);
                m.Close();
            });
            ConfirmModalButtonParams negativeButton = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], (m)=>m.Close());
            string title = StringValueAssetLoader.Instance["club-royal.name"];
            string message = "";

            // シーズン戦期間かつ入れ替え戦設定なら入れ替え戦前ポップアップの表示
            if (matchInfo.IsOnSeasonBattle && matchInfo.MColosseumEvent.gradeShiftType == ColosseumGradeShiftType.ShiftBattle)
            {
                message = StringValueAssetLoader.Instance["club-royal.announce.modal.all_finish_battle_before_shift_battle.message"];
            }
            // 全試合終了ポップアップの表示
            else
            {
                message = StringValueAssetLoader.Instance["club-royal.announce.modal.all_finish_battle.message"];
            }

            ConfirmModalData data = new ConfirmModalData(title, message, string.Empty, positiveButton, negativeButton);
            
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
        }

        //// <summary> まだ試合が開始していない時のモーダル表示 </summary>
        private void OpenNotBattleStartModal()
        {
            ConfirmModalButtonParams positiveButton = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["character.deck_edit"], (m) =>
            {
                AppManager.Instance.UIManager.PageManager.OpenPage(PageType.ClubRoyalDeck, true, null);
                m.Close();
            });
            ConfirmModalButtonParams negativeButton = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], (m)=>m.Close());
            string title = StringValueAssetLoader.Instance["club-royal.name"];
            string message = StringValueAssetLoader.Instance["club-royal.announce.modal.not_battle_start.message"];
            ConfirmModalData data = new ConfirmModalData(title, message, string.Empty, positiveButton, negativeButton);
            
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
        }
        
        //// <summary> 入場ボタンクリック時の処理 </summary>
        public void OnClickEntryButton()
        {
            // クリックしたときの開催状況に応じてモーダルを出す
            
            // 現在のクラブロワイヤルの開催状況を取得
            ClubRoyalManager.State state = ClubRoyalManager.GetClubRoyalState(matchInfo);

            // 全ての試合が終わっている
            if (state == ClubRoyalManager.State.AllBattleEnd)
            {
                OpenAllBattleFinishModal();
            }
            // 入場可能,バトル中はインゲームへ
            else if (state == ClubRoyalManager.State.BattleEntry || state == ClubRoyalManager.State.BattleStart)
            {
                MoveToClubRoyalInGame().Forget();
            }
            // 試合が開始されてない、試合がない
            else
            {
                OpenNotBattleStartModal();
            }
        }

        //// <summary> アナウンスボタンクリック時の処理 </summary>
        public void OnClickAnnounceButton()
        {
            // このボタンからはインゲームに遷移はできない
            
            // 開催状況に応じて表示するモーダルを変える
            // 全ての試合が終わっている
            if (schedule.State == ClubRoyalManager.State.AllBattleEnd)
            {
                OpenAllBattleFinishModal();
            }
            // 試合が開始されていない
            else
            {
                OpenNotBattleStartModal();
            }
        }
        
    }
}