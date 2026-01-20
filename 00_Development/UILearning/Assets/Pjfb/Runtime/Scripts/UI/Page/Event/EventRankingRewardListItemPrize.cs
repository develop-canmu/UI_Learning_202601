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
    public class EventRankingRewardListItemPrize : ScrollGridItem {
        public class Param {
            public PrizeJsonWrap prize = null; // 報酬内容
        }

        [SerializeField]
        PrizeJsonView _prizeView = null;

        protected override void OnSetView(object value){
            var param = (Param)value;
            _prizeView.SetView(param.prize);
        }
    }
}