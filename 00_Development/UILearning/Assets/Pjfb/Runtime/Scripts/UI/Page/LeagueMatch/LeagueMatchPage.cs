using System.Threading;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using Pjfb.Colosseum;
using Pjfb.Extensions;
using UnityEngine;
using Logger = CruFramework.Logger;

namespace Pjfb.LeagueMatch
{
    public enum LeagueMatchPageType
    {
        Divergence,
        LeagueMatchTop,
        LeagueMatchBoard,
        LeagueMatchSeasonEnd,
    }

    public enum MatchType
    {
        None = 0,
        Season = 1,
        Shift = 2
    }
    
    public class PageData
    {
        public LeagueMatchPageType targetPage = LeagueMatchPageType.LeagueMatchBoard;
        public PageType callerPage = PageType.Home;
        public ColosseumSeasonData seasonData;
        public bool isStackedPage = true;
        public LeagueMatchInfo leagueMatchInfo = null; 
        
        public PageData(LeagueMatchInfo leagueMatchInfo)
        {
            this.leagueMatchInfo = leagueMatchInfo;
        }
    }
    
    public class LeagueMatchPage : PageManager<LeagueMatchPageType>
    {
        public class Data : PageData
        {
            public Data(LeagueMatchInfo leagueMatchInfo) : base(leagueMatchInfo)
            {
                targetPage = LeagueMatchPageType.Divergence;
            }
        }
        
        protected override string GetAddress(LeagueMatchPageType matchPage)
        {
            return $"Prefabs/UI/Page/LeagueMatch/{matchPage}Page.prefab";
        }
        
        /// <summary>
        /// シーズン戦でデッキ編成からリッグマッチトップ画面へ戻る際にgetMatchInfoを叩くため引数を一時保存
        /// </summary>
        public long InGameMatchId { get; set; } = 0;
        
        public static void OpenPage(bool stack, object args = null)
        {
            if(AppManager.Instance.TutorialManager.OpenScenarioTutorial(PageType.LeagueMatch, stack, args))
            {
                return;
            }
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.LeagueMatch, stack, args);
        }
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            var pageData = ((PageData)args);
            LeagueMatchInfo leagueMatchInfo = pageData.leagueMatchInfo;
            // 戻るで遷移してきた
            if(TransitionType == PageTransitionType.Back)
            {
                if(CurrentPageType == LeagueMatchPageType.LeagueMatchTop)
                {
                    if(leagueMatchInfo.SeasonStartAt.IsFuture(AppTime.Now))
                    {
                        await OpenPageAsync(LeagueMatchPageType.LeagueMatchTop, true, new LeagueMatchTopPage.Data(MatchType.Season, leagueMatchInfo, -1) { callerPage = PageType.Home });
                        return;
                    }
                    else if (leagueMatchInfo.CanShiftBattle && AppTime.Now >= leagueMatchInfo.ShiftBattleStartAt)
                    {
                        await OpenPageAsync(LeagueMatchPageType.LeagueMatchTop, true, new LeagueMatchTopPage.Data(MatchType.Shift, leagueMatchInfo));
                        return;
                    }
                    else
                    {
                        if(InGameMatchId > 0)
                        {
                            await OpenPageAsync(LeagueMatchPageType.LeagueMatchTop, true, new LeagueMatchTopPage.Data(MatchType.Season, leagueMatchInfo, InGameMatchId));
                        }
                        else
                        {
                            // 準備中の画面に戻す
                            await OpenPageAsync(LeagueMatchPageType.LeagueMatchTop, true, new LeagueMatchTopPage.Data(MatchType.Season, leagueMatchInfo, -1) { callerPage = PageType.Home });
                        }
                        return;
                    }
                }
                else if(CurrentPageType == LeagueMatchPageType.LeagueMatchSeasonEnd)
                {
                    // シーズン結果画面
                    await OpenPageAsync(LeagueMatchPageType.LeagueMatchSeasonEnd, true, new LeagueMatchSeasonEndPage.Data(leagueMatchInfo) { callerPage = PageType.Home });
                    return;
                }
                else if(CurrentPageType == LeagueMatchPageType.LeagueMatchBoard)
                {
                    // リーグマッチ対戦表画面へ遷移
                    await OpenPageAsync(LeagueMatchPageType.LeagueMatchBoard, true, new LeagueMatchBoardPage.Data(leagueMatchInfo));
                    return;
                }
            }
            else
            {
                // シーズン期間外なら直近のシーズンIDをキャッシュ
                if (leagueMatchInfo.SeasonData == null)
                {
                    LeagueMatchManager.UpdateRecentSeasonId(leagueMatchInfo.MColosseumEvent.id);
                }
                
                if (pageData.targetPage == LeagueMatchPageType.Divergence)
                {
                    if (leagueMatchInfo.CanShiftBattle && AppTime.Now >= leagueMatchInfo.ShiftBattleStartAt)
                    {
                        await OpenPageAsync(LeagueMatchPageType.LeagueMatchTop, true, new LeagueMatchTopPage.Data(MatchType.Shift, leagueMatchInfo));
                        return;
                    }
                    else
                    {
                        // リーグマッチ対戦表画面へ遷移
                        await OpenPageAsync(LeagueMatchPageType.LeagueMatchBoard, true, new LeagueMatchBoardPage.Data(leagueMatchInfo));
                        return;
                    }
                }
            }
            
            await OpenPageAsync(pageData.targetPage, true, args);
        }
    }
}