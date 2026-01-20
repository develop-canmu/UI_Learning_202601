using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Club;
using Pjfb.Ranking;

namespace Pjfb.Ranking
{
    public class RankingClubScrollItem : ScrollGridItem
    {
        public class Param
        {
            private long emblemId = 0;
            /// <summary>
            /// エンブレムID
            /// </summary>
            public long EmblemId
            {
                get { return emblemId; }
            }
            
            private long clubId = 0;
            /// <summary>
            /// クラブID
            /// </summary>
            public long ClubId
            {
                get { return clubId; }
            }

            private long rank = 0;
            /// <summary>
            /// 順位
            /// </summary>
            public long Rank
            {
                get { return rank; }
            }
            
            private string clubName = string.Empty;
            /// <summary>
            /// クラブ名
            /// </summary>
            public string ClubName
            {
                get { return clubName; }
            }
            
            private BigValue value = BigValue.Zero;
            /// <summary>
            /// ランキングで比較する値
            /// </summary>
            public BigValue Value
            {
                get { return value; }
            }
            
            private bool isCurrent = false;
            /// <summary>
            /// 自分かどうか
            /// </summary>
            public bool IsCurrent
            {
                get { return isCurrent; }
            }
            
            private bool isPointOmission = false;
            /// <summary>
            /// ポイントを省略表記するかどうか
            /// </summary>
            public bool IsPointOmission
            {
                get { return isPointOmission; }
            }
            
            public Param(long emblemId, long clubId, long rank, string clubName, BigValue value, bool isCurrent, bool isPointOmission)
            {
                this.emblemId = emblemId;
                this.clubId = clubId;
                this.rank = rank;
                this.clubName = clubName;
                this.value = value;
                this.isCurrent = isCurrent;
                this.isPointOmission = isPointOmission;
            }
        }

        [SerializeField]
        private RankingClubInfoView clubInfoView = null;
        
        protected override void OnSetView(object value)
        {
            Param param = (Param)value;
            
            // 各情報のセット
            clubInfoView.SetRank(param.Rank, param.IsCurrent);
            clubInfoView.SetClubId(param.ClubId);
            clubInfoView.SetEmblem(param.EmblemId);
            clubInfoView.SetName(param.ClubName);
            clubInfoView.SetPoint(param.Value, param.IsPointOmission);
        }
    }
}