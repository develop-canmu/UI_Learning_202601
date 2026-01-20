using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.ClubMatch;
using UnityEngine;
using Pjfb.Networking.App.Request;
using Pjfb.Extensions;
using Pjfb.UserData;
using TMPro;
using UniRx;

namespace Pjfb.LeagueMatch
{
    public enum BattleResult
    {
        Win = 1,
        Lose = 2,
        Draw = 3,
        None = 98,
        Unprocessed = 99,
    }
    
    public class LeagueMatchBoardPage : Page
    {
        public class Data : PageData
        {
            public Data(LeagueMatchInfo leagueMatchInfo) : base(leagueMatchInfo)
            {
                this.seasonData = leagueMatchInfo.SeasonData;
                targetPage = LeagueMatchPageType.LeagueMatchBoard;
            }
        }
        
        private Data currentArgs;
        private GroupLeagueMatchBoardInfo boardInfo;
        private CancellationTokenSource cancellationTokenSource;
        
        [SerializeField] private ClubRankImage _rank = null;
        [SerializeField] private GameObject dummyRank = null;
        [SerializeField] private TextMeshProUGUI leagueMatchTitle;
        [SerializeField] private TextMeshProUGUI leagueTerm;
        [SerializeField] private LeagueMatchBoardUI leagueMatchBoardUI;
        [SerializeField] private LeagueMatchHeader leagueMatchHeader;
        
        #region next battle info
        [SerializeField] private RectTransform nextOpponentRoot;
        [SerializeField] private TextMeshProUGUI opponentTitle;
        [SerializeField] private TextMeshProUGUI team2ClubName;
        [SerializeField] private ClubEmblemImage team2ClubIcon;
        [SerializeField] private TextMeshProUGUI nextBattleStartData;
        #endregion
        
        [SerializeField] private TextMeshProUGUI transitionButtonText;
        [SerializeField] private TextMeshProUGUI extraTextLine;
        
        [SerializeField] private UIBadgeNotification menuBadge;
        
        [SerializeField] private UIButton backButton;

        [SerializeField] private LeagueMatchHelp help;
        
        private GroupLeagueMatchBoardRow todayMatch;
        private IDisposable _disposeScheduleCountDown;
        private LeagueMatchInfoSchedule _nextSchedule;
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            currentArgs = (Data)args;
            LeagueMatchUtility.UpdateMenuNotification(menuBadge);
            // ヘルプの設定
            help.SetHelp(currentArgs.leagueMatchInfo);
            await OnPreOpenExecution();
            _nextSchedule = currentArgs.leagueMatchInfo.GetNextSchedule();
            SetCounter(_nextSchedule, ()=> { OnPreOpenExecution().Forget();});
            await base.OnPreOpen(args, token);
        }
        
        protected override void OnOpened(object args)
        {
            base.OnOpened(args);
            // ポップアップの表示
            AppManager.Instance.TutorialManager.OpenLeagueMatchTutorialAsync().Forget();
        }

        protected override UniTask<bool> OnPreLeave(CancellationToken token)
        {
            cancellationTokenSource?.Cancel();
            _disposeScheduleCountDown?.Dispose();
            return base.OnPreLeave(token);
        }
        
        void SetCounter(LeagueMatchInfoSchedule waitForSchedule, Action action)
        {
            this._nextSchedule = waitForSchedule;
            _disposeScheduleCountDown?.Dispose();
            _disposeScheduleCountDown = 
                Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1)).Subscribe(
                    (_) =>
                    {
                        Refresh();
                        var schedule = currentArgs.leagueMatchInfo.GetNextSchedule();
                        if (this._nextSchedule != schedule)
                        {
                            action.Invoke();
                            SetCounter(schedule, action);
                        }
                    }
                ).AddTo(gameObject);
        }

        private async UniTask OnPreOpenExecution()
        {
            var leagueBoardData = await LeagueMatchUtility.GetLeagueBoardInfo(currentArgs.leagueMatchInfo.SeasonData.SeasonId);
            boardInfo = leagueBoardData.boardInfo;
            todayMatch = boardInfo.rowList.FirstOrDefault(x=> x.sColosseumGroupStatusId == boardInfo.todayMatch.sColosseumGroupStatusId);
            await InitUI();
        }
        
        private async UniTask InitUI()
        {
            leagueTerm.text = LeagueMatchManager.GetDateTimeString(currentArgs.seasonData.SeasonHome.startAt.TryConvertToDateTime(), currentArgs.seasonData.SeasonHome.endAt.TryConvertToDateTime());

            if(currentArgs.seasonData.IsInstantTournament)
            {
                // タイトルテキスト
                leagueMatchTitle.text = currentArgs.leagueMatchInfo.MColosseumEvent.name;
                // ダミーアイコン表示
                dummyRank.SetActive(true);
                // ランクアイコン非表示
                _rank.gameObject.SetActive(false);
                // 戻るボタン表示
                backButton.gameObject.SetActive(true);
            }
            else
            {
                // タイトルテキスト
                leagueMatchTitle.text = StringValueAssetLoader.Instance["league.type.season"];
                // ダミーアイコン非表示
                dummyRank.SetActive(false);
                // ランクアイコン表示
                _rank.gameObject.SetActive(true);
                long gradeNumber = UserDataManager.Instance.ColosseumGradeData.GetGradeNumber(currentArgs.seasonData.MColosseumEvent.mColosseumGradeGroupId, currentArgs.seasonData.UserSeasonStatus.groupSeasonStatus.groupId);
                await _rank.SetTextureAsync(gradeNumber);
                // 戻るボタン非表示
                backButton.gameObject.SetActive(false);
            }
            var battleEndTimeOfToday = currentArgs.leagueMatchInfo.GetBattleEndTimeOfToday();
            bool showOpponent = AppTime.Now < battleEndTimeOfToday;
            nextOpponentRoot.gameObject.SetActive(showOpponent);
            if (showOpponent)
            {
                opponentTitle.text = StringValueAssetLoader.Instance["league.match.match_board.current_opponent_club"];
                team2ClubName.text = todayMatch != null ? todayMatch.name : String.Empty;
                var battleStartDate = currentArgs.leagueMatchInfo.GetNextOrCurrentBattleStartDateTime();
                nextBattleStartData.text = string.Format(StringValueAssetLoader.Instance["league.match_board.battle_start_date"], battleStartDate.Year, battleStartDate.Month, battleStartDate.Day, battleStartDate.Hour, battleStartDate.Minute, battleStartDate.Second );
                if (todayMatch != null)
                {
                    await team2ClubIcon.SetTextureAsync(todayMatch.mGuildEmblemId);
                }
            }
            
            leagueMatchBoardUI.InitUI(boardInfo.rowList, boardInfo.groupStatusDetailSelf.sColosseumGroupStatusId, todayMatch != null ? todayMatch.sColosseumGroupStatusId : default);
            leagueMatchHeader.Setup(currentArgs.seasonData, boardInfo, currentArgs.leagueMatchInfo).Forget();
        }

        void Refresh()
        {
            extraTextLine.text = currentArgs.leagueMatchInfo.GetBoardUnderText(boardInfo.groupStatusDetailSelf.ranking);
            transitionButtonText.text = currentArgs.leagueMatchInfo.GetMatchBoardTransitionButtonText();
        }
        
        public void Transition()
        {
            var aggregatingEndTimeOfToday = currentArgs.leagueMatchInfo.GetAggregatingEndTimeOfToday();
            // 入れ替え戦に参加できるかどうか
            MatchType matchType = currentArgs.leagueMatchInfo.CanShiftBattle ? MatchType.Shift : MatchType.Season;
            // シーズン戦最終日かつ最終試合から一定時間経過後の場合は結果ページへ
            if (DateTimeExtensions.IsPast(aggregatingEndTimeOfToday, AppTime.Now) &&
                DateTimeExtensions.IsSameDay(currentArgs.leagueMatchInfo.SeasonBattleEndAt, AppTime.Now))
            {
                LeagueMatchPage.OpenPage(true, new LeagueMatchSeasonEndPage.Data(currentArgs.leagueMatchInfo) { callerPage = PageType.Home });
            }
            // 違う場合はトップページへ
            else
            {
                LeagueMatchPage.OpenPage(true,new LeagueMatchTopPage.Data(matchType, currentArgs.leagueMatchInfo, boardInfo.todayMatch.inGameMatchId) { callerPage = PageType.Home });
            }
        }
        
        public void OnClickLeagueMatchMenu()
        {
            LeagueMatchManager.OnClickLeagueMatchMenu(currentArgs.leagueMatchInfo);
        }
        
        /// <summary>
        /// uGUI
        /// </summary>
        public void OnLongTapOwnClub()
        {
            LeagueMatchUtility.OpenClubInfo(boardInfo.groupStatusDetailSelf.groupId, boardInfo.groupStatusDetailSelf.groupType).Forget();
        }
        
        /// <summary>
        /// uGUI
        /// </summary>
        public void OnLongTapNextOpponentClub()
        {
            LeagueMatchUtility.OpenClubInfo(boardInfo.todayMatch.groupId, boardInfo.todayMatch.groupType).Forget();
        }
        
        /// <summary>戻るボタン</summary>
        public void OnClickLeagueMatchTournamentBackButton()
        {
            AppManager.Instance.UIManager.PageManager.RemovePageStack(PageType.LeagueMatch);
            // 大会ページを開く
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.LeagueMatchTournament, false, null);    
        }
    }
}