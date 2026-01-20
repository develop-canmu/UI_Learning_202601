using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Networking.App.Request;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;

namespace Pjfb.LeagueMatch
{
    public class LeagueMatchSeasonEndPage : Page
    {
        public class Data : PageData
        {
            public Data(LeagueMatchInfo leagueMatchInfo) : base(leagueMatchInfo)
            {
                targetPage = LeagueMatchPageType.LeagueMatchSeasonEnd;
                this.seasonData = leagueMatchInfo.SeasonData;
            }
        }
        
        [SerializeField] private LeagueMatchHeader leagueMatchHeader;
        [SerializeField] private ScrollGrid listScroll;
        [SerializeField] private UIBadgeNotification menuBadge;
        [SerializeField] private LeagueMatchHelp help;
        
        private Data currentArgs;
        private GroupLeagueMatchMatchHistoryInfo matchHistoryInfo;
        private CancellationTokenSource cancellationTokenSource;
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            currentArgs = (Data)args;
            LeagueMatchUtility.UpdateMenuNotification(menuBadge);
            help.SetHelp(currentArgs.leagueMatchInfo);
            var response = await LeagueMatchUtility.GetMatchHistory(currentArgs.seasonData.SeasonId);
            matchHistoryInfo = response.matchHistoryInfo;
            InitUI();
            await base.OnPreOpen(args, token);
        }
        
        protected override void OnOpened(object args)
        {
            base.OnOpened(args);
            // チュートリアルポップアップの表示
            AppManager.Instance.TutorialManager.OpenLeagueMatchTutorialAsync().Forget();
        }

        
        protected override UniTask<bool> OnPreLeave(CancellationToken token)
        {
            cancellationTokenSource?.Cancel();
            return base.OnPreLeave(token);
        }
        
        private void InitUI(bool isUpdateInterval = true)
        {
            if (isUpdateInterval)
            {
                cancellationTokenSource?.Cancel();
                cancellationTokenSource = new CancellationTokenSource();
            }
            leagueMatchHeader.SetupForSeasonEnd(currentArgs.seasonData, matchHistoryInfo.groupStatusDetailSelf, currentArgs.leagueMatchInfo).Forget();
            // スクロールデータ
            List<LeagueMatchResultScrollGridItem.Arguments> listScrollDatas = new List<LeagueMatchResultScrollGridItem.Arguments>();
            for(int i = 0; i < matchHistoryInfo.matchHistoryList.Length; i++)
            {
                listScrollDatas.Add(new LeagueMatchResultScrollGridItem.Arguments(matchHistoryInfo.matchHistoryList[i], currentArgs.seasonData.MColosseumEvent)); 
            }
            // スクロールにセット
            listScroll.SetItems(listScrollDatas);
        }
        
        public void OnClickBack(UIButton sender)
        {
            cancellationTokenSource.Cancel();
            if (currentArgs.isStackedPage)
            {
                sender.BackPage();
            }
            else
            {
                AppManager.Instance.UIManager.PageManager.OpenPage(currentArgs.callerPage, false,null);
            }
        }
        
        public void OnClickLeagueMatchMenu()
        {
            LeagueMatchManager.OnClickLeagueMatchMenu(currentArgs.leagueMatchInfo);
        }
    }
}