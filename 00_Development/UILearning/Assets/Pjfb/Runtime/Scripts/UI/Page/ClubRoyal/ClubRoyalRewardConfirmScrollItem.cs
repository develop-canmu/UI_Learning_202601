using CruFramework.UI;
using Pjfb.Master;
using UnityEngine;

namespace Pjfb.ClubRoyal
{ 
    public class ClubRoyalRewardConfirmScrollItem : ScrollGridItem
    {
        public class Param
        {
            private long upperRanking = 0;
            /// <summary>
            /// ランキング上限
            /// </summary>
            public long UpperRanking
            {
                get { return upperRanking; }
            }
            
            private long lowerRanking = 0;
            /// <summary>
            /// ランキング下限
            /// </summary>
            public long LowerRanking
            {
                get { return lowerRanking; }
            }
            
            private PrizeJsonWrap[] prizeJsonWrap = null;
            /// <summary>
            /// 報酬内容
            /// </summary>
            public PrizeJsonWrap[] PrizeJsonWrap
            {
                get { return prizeJsonWrap; }
            }
            
            private bool isFrameRankInShown = false;
            /// <summary>
            /// ランクインのフレームを表示するか
            /// </summary>
            public bool IsFrameRankInShown
            {
                get { return isFrameRankInShown; }
            }
            
            public Param(long upperRanking, long lowerRanking, PrizeJsonWrap[] prizeJsonWrap, bool isFrameRankInShown)
            {
                this.upperRanking = upperRanking;
                this.lowerRanking = lowerRanking;
                this.prizeJsonWrap = prizeJsonWrap;
                this.isFrameRankInShown = isFrameRankInShown;
            }
        }
        [SerializeField]
        private ClubRoyalRewardInfoView rewardInfoView = null;

        protected override void OnSetView(object value)
        {
            Param param = (Param)value;
            
            rewardInfoView.SetRank(param.UpperRanking, param.LowerRanking, param.IsFrameRankInShown);
            rewardInfoView.SetRewardPrize(param.PrizeJsonWrap);
        }
    }
}