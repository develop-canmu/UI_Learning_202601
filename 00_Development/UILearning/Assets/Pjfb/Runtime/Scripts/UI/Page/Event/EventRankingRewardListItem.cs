using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using TMPro;
using CruFramework.UI;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Club;

namespace Pjfb.EventRanking {
    public class EventRankingRewardListItem : EventRankingListItem {
        [SerializeField]
        ScrollGrid _scrollGrid = null;


        List<PrizeJsonView> _prizeItemList = new List<PrizeJsonView>();

        public class Param{
            public long upperRanking = 0; // ランキング上限
            public long lowerRanking = 0; // ランキング下限
            public PrizeJsonWrap[] prizeList = null; // 報酬内容
            public bool isCurrent = false;
        }

        public void UpdateView(Param param){
            if( param.upperRanking == param.lowerRanking ) {
                UpdateRankView( param.upperRanking, param.isCurrent );
            } else {
                UpdateRewardRankView(param);
            }
            
            CreatePrize(param);
        }

        protected override void OnSetView(object value){
            var param = value as Param;
            UpdateView(param);
        }
        
        protected void CreatePrize( Param param ){
             if( param.prizeList != null ) {
                var paramList = new List<EventRankingRewardListItemPrize.Param>();
                foreach( var prize in param.prizeList ){
                    var prizeParam = new EventRankingRewardListItemPrize.Param();
                    prizeParam.prize = prize;
                    paramList.Add(prizeParam);
                }
                _scrollGrid.SetItems(paramList);
            }
        }

        protected void UpdateRewardRankView( Param param ){
            _rankText.gameObject.SetActive(true);
            _rankImage.gameObject.SetActive(false);
            _currentRankFrame.gameObject.SetActive(param.isCurrent);
            _rankText.text = string.Format( StringValueAssetLoader.Instance["ranking.reword_rank"], param.upperRanking, param.lowerRanking );
        }
        
    }
}