using System;
using System.Linq;
using Pjfb.Club;
using UnityEngine;
using Pjfb.Common;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;

namespace Pjfb.LeagueMatch
{
    public class LeagueMatchBanner : LeagueMatchBannerBase
    {
        public enum State
        {
            /// <summary>シーズン開始前</summary>
            BeforeSeason,
            /// <summary>シーズンが開始した</summary>
            SeasonStarted,
            /// <summary>エントリ受付前</summary>
            BeforeEntry,
            /// <summary>エントリ受付中</summary>
            EntryStarted,
            /// <summary>エントリ受付終了</summary>
            EntryEnded,
            /// <summary>バトルが終わった</summary>
            BattleEnded,
            /// <summary>シーズン戦が終わった</summary>
            SeasonBattleEnded,
            /// <summary>シーズンが終わった</summary>
            SeasonEnded,
            /// <summary>マッチング失敗</summary>
            MatchingFailed
        }
        
        [SerializeField]
        private GameObject seasonBattleRoot = null;
        
        [SerializeField]
        private GameObject shiftBattleRoot = null;
        
        [SerializeField]
        private GameObject noClubRoot = null;
        
        [SerializeField]
        private TextMeshProUGUI messageText = null;
        
        [SerializeField]
        private TextMeshProUGUI termText = null;
        
        [SerializeField]
        private Material defaultTextMaterial = null;
        
        [SerializeField]
        private Material noClubTextMaterial = null;
        
        private LeagueMatchInfoSchedule nextSchedule = null;
        private bool isUpdateView = true;
        private State currentState = State.BeforeSeason;
        private DateTime nextStateStartAt = DateTime.MinValue;

        public override void SetView(LeagueMatchInfo leagueMatchInfo)
        {
            this.leagueMatchInfo = leagueMatchInfo;

            // テキストマテリアルセット
            messageText.fontMaterial = (leagueMatchInfo == null || !IsJoinedClub) ? noClubTextMaterial : defaultTextMaterial;
            isUpdateView = true;
            
            // リーグマッチが設定されてない
            if(leagueMatchInfo == null)
            {
                // 非表示
                gameObject.SetActive(false);
                // ビュー更新しない
                isUpdateView = false;
                return;
            }
            
            // マッチングしてない
            if(IsMatchingFailed)
            {
                // クラブに参加してない
                if(!IsJoinedClub)
                {
                    // ステート更新
                    currentState = State.MatchingFailed;
                    // ルートオブジェクト切り替え
                    seasonBattleRoot.SetActive(false);
                    shiftBattleRoot.SetActive(false);
                    noClubRoot.SetActive(true);
                    // メッセージ
                    messageText.text = StringValueAssetLoader.Instance["league.match.banner_message_10"];
                    // 開催期間非表示
                    termText.gameObject.SetActive(false);
                    // ビュー更新しない
                    isUpdateView = false;
                    return;
                }
                
                // シーズン中
                if(IsOnSeason)
                {
                    // ステート更新
                    currentState = State.MatchingFailed;
                    
                    // 今試合参加不可
                    if(leagueMatchInfo.MColosseumEvent.gradeShiftType == ColosseumGradeShiftType.ShiftBattle && DateTimeExtensions.IsWithinPeriod(AppTime.Now, leagueMatchInfo.ShiftBattleStartAt, leagueMatchInfo.ShiftBattleEndAt))
                    {
                        // ルートオブジェクト切り替え
                        seasonBattleRoot.SetActive(false);
                        shiftBattleRoot.SetActive(true);
                        noClubRoot.SetActive(false);
                        // メッセージ
                        messageText.text = StringValueAssetLoader.Instance["league.match.banner_message_11"];
                        // ビュー更新しない
                        isUpdateView = false;
                    }
                    // 今シーズン参加不可
                    else
                    {
                        // ルートオブジェクト切り替え
                        seasonBattleRoot.SetActive(true);
                        shiftBattleRoot.SetActive(false);
                        noClubRoot.SetActive(false);
                        // メッセージ
                        if(leagueMatchInfo.MColosseumEvent.clientHandlingType == ColosseumClientHandlingType.InstantTournament)
                        {
                            messageText.text = StringValueAssetLoader.Instance["instant_tournament.banner_message_8"];
                        }
                        else
                        {
                            messageText.text = StringValueAssetLoader.Instance["league.match.banner_message_8"];
                        }
                        // ビュー更新しない
                        isUpdateView = false;
                    }
                    // 開催期間非表示
                    termText.gameObject.SetActive(false);
                    return;
                }
            }
            
            // ルートオブジェクト切り替え
            bool canShiftBattle = leagueMatchInfo.CanShiftBattle;
            seasonBattleRoot.SetActive(!canShiftBattle);
            shiftBattleRoot.SetActive(canShiftBattle);
            noClubRoot.SetActive(false);
            
            // 期間表示
            string termFormat = "MM/dd HH:mm";
            if(IsOnSeason)
            {
                termText.text = string.Format(StringValueAssetLoader.Instance["league.match.banner_term"], leagueMatchInfo.SeasonStartAt.ToString(termFormat), leagueMatchInfo.SeasonEndAt.ToString(termFormat));
            }
            else
            {
                termText.text = string.Format(StringValueAssetLoader.Instance["league.match.banner_term"], leagueMatchInfo.NextSeasonStartAt.ToString(termFormat), leagueMatchInfo.NextSeasonEndAt.ToString(termFormat));
            }
            
            // ステート更新
            UpdateState();
        }

        protected override void UpdateView()
        {
            // データがない
            if(leagueMatchInfo == null)
            {
                return;
            }
            
            // ビュー更新しない
            if(!isUpdateView)
            {
                return;
            }
            
            // 更新しないステート
            if(currentState == State.SeasonStarted || currentState == State.SeasonBattleEnded)
            {
                return;
            }
            
            // ステート更新
            if(nextStateStartAt.IsPast(AppTime.Now))
            {
                UpdateState();
            }
            
            // 残り時間文字列
            string remainingString = nextStateStartAt.GetRemainingString(AppTime.Now);
            // メッセージ更新
            switch (currentState)
            {
                case State.BeforeSeason:
                    messageText.text = string.Format(StringValueAssetLoader.Instance["league.match.banner_message_1"], remainingString);
                    break;
                case State.EntryStarted:
                    messageText.text = string.Format(StringValueAssetLoader.Instance["league.match.banner_message_2"], remainingString);
                    break;
                case State.EntryEnded:
                    messageText.text = string.Format(StringValueAssetLoader.Instance["league.match.banner_message_3"], nextSchedule.RoundNumber, remainingString);
                    break;
                case State.BattleEnded:
                    messageText.text = string.Format(StringValueAssetLoader.Instance["league.match.banner_message_4"], nextSchedule.RoundNumber - 1, remainingString);
                    break;
                case State.BeforeEntry:
                    if(shiftBattleRoot.activeInHierarchy)
                    {
                        messageText.text = string.Format(StringValueAssetLoader.Instance["league.match.banner_message_12"], remainingString);
                    }
                    else
                    {
                        messageText.text = string.Format(StringValueAssetLoader.Instance["league.match.banner_message_6"], remainingString);
                    }
                    break;
                case State.SeasonEnded:
                    messageText.text = string.Format(StringValueAssetLoader.Instance["league.match.banner_message_7"], remainingString);
                    break;
            }
        }
        
        /// <summary>ステート更新</summary>
        private void UpdateState()
        {
            // シーズン中
            if(IsOnSeason)
            {
                // 次のスケジュール更新
                nextSchedule = leagueMatchInfo.GetNextSchedule();
                
                // 次のスケジュールがない
                if(nextSchedule == null)
                {
                    // シーズン戦期間
                    if(IsOnSeasonBattle)
                    {
                        // シーズン戦終了表示
                        OnSeasonBattleEnded();
                    }
                    else
                    {
                        // シーズン終了表示
                        OnSeasonEnded();
                    }
                }
                else if(nextSchedule.CurrentSchedule == LeagueMatchInfoSchedule.Schedule.FormationLock)
                {
                    if(leagueMatchInfo.SeasonData == null)
                    {
                        OnSeasonStarted();
                    }
                    else if(DateTimeExtensions.IsSameDay(AppTime.Now, nextSchedule.StartAt))
                    {
                        // エントリ受付中
                        OnEntryStarted(nextSchedule);
                    }
                    else
                    {
                        // エントリ開始まで
                        OnBeforeEntry();
                    }
                }
                else if(nextSchedule.CurrentSchedule == LeagueMatchInfoSchedule.Schedule.Battle)
                {
                    if(nextSchedule.RoundNumber == 1)
                    {
                        // エントリ受付終了
                        OnEntryEnded(nextSchedule);       
                    }
                    else
                    {
                        // バトル終了
                        OnBattleEnded(nextSchedule);
                    }
                }
            }
            else
            {
                // シーズン外
                OnBeforeSeason();
            }
        }

        /// <summary>シーズン開始前通知</summary>
        private void OnBeforeSeason()
        {
            if(leagueMatchInfo.NextSeasonStartAt.IsFuture(AppTime.Now))
            {
                currentState = State.BeforeSeason;
                nextStateStartAt = leagueMatchInfo.NextSeasonStartAt;
            }
            else
            {
                // 次回開催期間が過去の場合はシーズン開始の表示
                OnSeasonStarted();
            }
        }
        
        /// <summary>シーズン開始通知</summary>
        private void OnSeasonStarted()
        {
            currentState = State.SeasonStarted;
            // メッセージ
            messageText.text = StringValueAssetLoader.Instance["league.match.banner_message_9"];
        }

        /// <summary>シーズン開始通知</summary>
        private void OnEntryStarted(LeagueMatchInfoSchedule nextSchedule)
        {
            // エントリ受付中
            currentState = State.EntryStarted;
            nextStateStartAt = nextSchedule.StartAt;
        }
  
        /// <summary>エントリ終了通知</summary>
        private void OnEntryEnded(LeagueMatchInfoSchedule nextSchedule)
        {
            currentState = State.EntryEnded;
            nextStateStartAt = nextSchedule.StartAt;
        }

        /// <summary>バトル終了通知</summary>
        private void OnBattleEnded(LeagueMatchInfoSchedule nextSchedule)
        {
            currentState = State.BattleEnded;
            nextStateStartAt = nextSchedule.StartAt;
        }
        
        /// <summary>シーズンバトル終了通知</summary>
        private void OnSeasonBattleEnded()
        {
            currentState = State.SeasonBattleEnded;
            messageText.text = StringValueAssetLoader.Instance["league.match.banner_message_5"];
        }
        
        /// <summary>エントリ受付前</summary>
        private void OnBeforeEntry()
        {
            currentState = State.BeforeEntry;
            // 受付開始時間を次の更新時間にする
            nextStateStartAt = leagueMatchInfo.GetTeamEntryStartTime();
        }

        /// <summary>シーズン終了通知</summary>
        private void OnSeasonEnded()
        {
            // 次ステート
            currentState = State.SeasonEnded;
            // 次ステートの開始時間
            nextStateStartAt = leagueMatchInfo.NextSeasonStartAt;
        }

        public void OnClick()
        {
            // リーグマッチが設定されてない
            if(leagueMatchInfo == null)
            {
                return;
            }
            
            // クラブに参加してない
            if(!IsJoinedClub)
            {
                // クラブ機能が解放されてない
                if(!UserDataManager.Instance.IsUnlockSystem(ClubUtility.clubLockId))
                {
                    ClubUtility.OpenClubLockModal();
                    return;
                }
                // マッチングしてない
                if (IsMatchingFailed)
                {
                    // クラブ側でクラブを探すページへ遷移する
                    AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Club, true, null);
                    return;
                }
            }
            
            // シーズンが開始した
            if(currentState == State.SeasonStarted)
            {
                ConfirmModalData modalData = new ConfirmModalData();
                modalData.Title = StringValueAssetLoader.Instance["league.match.need_update_title_1"];
                modalData.Message = StringValueAssetLoader.Instance["league.match.need_update_message_1"];
                modalData.NegativeButtonParams = new ConfirmModalButtonParams(
                    StringValueAssetLoader.Instance["common.executeUpdate"],
                    async window =>
                    {
                        // API実行
                        ColosseumGetHomeDataAPIRequest request = new ColosseumGetHomeDataAPIRequest();
                        ColosseumGetHomeDataAPIPost post = new ColosseumGetHomeDataAPIPost();
                        post.getTurn = 0;
                        request.SetPostData(post);
                        await APIManager.Instance.Connect(request);
                        // ビュー更新
                        SetView(LeagueMatchUtility.GetLeagueMatchInfo(leagueMatchInfo.MColosseumEvent.clientHandlingType));
                        // ウィンドウ閉じる
                        window.Close();
                    }
                );
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, modalData);
                return;
            }

            // シーズン開催中
            if(IsOnSeason)
            {
                // マッチングしてない
                if(IsMatchingFailed)
                {
                    ParticipationConditionsModal.Params param = new ParticipationConditionsModal.Params();
                    if(leagueMatchInfo.MColosseumEvent.clientHandlingType == ColosseumClientHandlingType.InstantTournament)
                    {
                        param.title = StringValueAssetLoader.Instance["instant_tournament.participation_conditions_modal.title"];
                        param.message = StringValueAssetLoader.Instance["instant_tournament.participation_conditions_modal.message"];
                    }
                    else
                    {
                        param.title = StringValueAssetLoader.Instance["league.match.participation_conditions_modal.title"];
                        param.message = StringValueAssetLoader.Instance["league.match.participation_conditions_modal.message"];
                    }
                    param.colosseumEventMaster = leagueMatchInfo.MColosseumEvent;
                    AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ParticipationConditions, param);
                    return;
                }
                
                switch (currentState)
                {
                    case State.BeforeSeason:
                        LeagueMatchPage.OpenPage(true, new LeagueMatchTopPage.Data(MatchType.Season, leagueMatchInfo, -1) { callerPage = PageType.Home });
                        break;
                    case State.BeforeEntry:
                    case State.EntryStarted:
                    case State.EntryEnded:
                    case State.BattleEnded:
                    case State.SeasonBattleEnded:
                        // リーグマッチページへ遷移
                        LeagueMatchPage.OpenPage(true, new LeagueMatchPage.Data(leagueMatchInfo) { callerPage = PageType.Home });
                        break;
                    /// <summary>シーズンが終わった</summary>
                    case State.SeasonEnded:
                        if (leagueMatchInfo.CanShiftBattle && AppTime.Now >= leagueMatchInfo.ShiftBattleStartAt)
                        {
                            LeagueMatchPage.OpenPage(true, new LeagueMatchPage.Data(leagueMatchInfo) { callerPage = PageType.Home });
                        }
                        else
                        {
                            LeagueMatchPage.OpenPage(true, new LeagueMatchSeasonEndPage.Data(leagueMatchInfo) { callerPage = PageType.Home });
                        }
                        break;
                    default:
                        throw new NotImplementedException($"Current state is {currentState}");
                }
            }
            else
            {
                // シーズン外
                LeagueMatchPage.OpenPage(true, new LeagueMatchTopPage.Data(MatchType.Season, leagueMatchInfo, -1) { callerPage = PageType.Home });
            }
        }
    }
}