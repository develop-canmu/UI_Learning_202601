using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using CruFramework.UI;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using TMPro;

namespace Pjfb.LeagueMatch
{
    public class LeagueMatchRankingModal : ModalWindow
    {
        public class Data
        {
            private long seasonId;
            public long SeasonId
            {
                get { return seasonId; }
                set { seasonId = value; }
            }
        }

        [SerializeField] private TextMeshProUGUI textSeasonTitle;
        [SerializeField] TextMeshProUGUI textSeasonPeriod;
        [SerializeField] ScrollGrid scrollGrid;
        [SerializeField] GameObject emptyText;
        private Data data;
        private ColosseumGetGroupBattleRankingAPIResponse response;
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            bool hasRankingData = false;
            data = (Data)args;
            await GetRankingData();
            // 開催期間と何日目かの表示
            DateTime startTime = response.userSeasonStatus.startAt.TryConvertToDateTime();
            DateTime endTime = response.userSeasonStatus.endAt.TryConvertToDateTime();
            textSeasonPeriod.text = LeagueMatchManager.GetDateTimeString(startTime, endTime);
            // シーズンが終了していたら分岐
            if (AppTime.Now > endTime)
            {
                textSeasonTitle.text = StringValueAssetLoader.Instance["club.season.end_title"];
            }
            else
            {
                DateTime displayTimeAdjustment = startTime.AddDays(-1);
                TimeSpan dayCount = AppTime.Now.Date - displayTimeAdjustment.Date;
                textSeasonTitle.text = string.Format(StringValueAssetLoader.Instance["league.match.day_count"], dayCount.Days);
            }

            // ランキング集計前かどうかを各値が全て0かどうかで判定
            for (int i = 0; i < response.rankingGroupList.Length; i++)
            {
                if(response.rankingGroupList[i].winCount != 0 && 
                   response.rankingGroupList[i].score != 0 && 
                   response.rankingGroupList[i].winCountSub != 0)
                {
                    hasRankingData = true;
                    break;
                }
            }
            if (hasRankingData)
            {
                scrollGrid.gameObject.SetActive(true);
                emptyText.SetActive(false);
                SetItem();
            }
            else
            {
                scrollGrid.gameObject.SetActive(false);
                emptyText.SetActive(true);
            }
            
            await base.OnPreOpen(args, token);
        }
        
        // ランキングデータを取得
        private async UniTask GetRankingData()
        {
            ColosseumGetGroupBattleRankingAPIPost post = new ColosseumGetGroupBattleRankingAPIPost();
            post.sColosseumEventId = data.SeasonId;
            post.getTurn = 1;
            ColosseumGetGroupBattleRankingAPIRequest request = new ColosseumGetGroupBattleRankingAPIRequest();
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            response = request.GetResponseData();
        }

        // ランキングデータをセット
        private void SetItem()
        {
            var paramList = new List<LeagueMatchRankingItem.Param>();
            
            if( response.rankingGroupList != null ) {
                foreach( var group in response.rankingGroupList){
                    var param = new LeagueMatchRankingItem.Param();
                    
                    param.IsAffiliated = group.groupId == response.userSeasonStatus.groupSeasonStatus.groupId;
                    
                    param.Ranking = group.ranking;
                    param.MGuildEmblemId = group.mGuildEmblemId;
                    param.Name = group.name;
                    param.WinCount = group.winCount;
                    param.Score = group.score;
                    param.WinCountSub = group.winCountSub;
                    
                    paramList.Add(param);
                }
                scrollGrid.SetItems(paramList);
            }
        }
        
        public void OnClickCloseButton()
        {
            Close();
        }
    }
}