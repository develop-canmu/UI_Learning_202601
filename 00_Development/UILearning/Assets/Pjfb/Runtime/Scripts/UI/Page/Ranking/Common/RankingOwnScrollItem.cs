using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Ranking
{
    public class RankingOwnScrollItem : ScrollGridItem
    {
        public class Param
        {
            private long userId = 0;
            /// <summary>
            /// ユーザーID
            /// </summary>
            public long UserId
            {
                get { return userId; }
            }
            
            private long rank = 0;
            /// <summary>
            /// 順位
            /// </summary>
            public long Rank
            {
                get { return rank; }
            }
            
            private long userIconId = 0;
            /// <summary>
            /// アイコンのID
            /// </summary>
            public long UserIconId
            {
                get { return userIconId; }
            }
            
            private string userName = string.Empty;
            /// <summary>
            /// ユーザー名
            /// </summary>
            public string UserName
            {
                get { return userName; }
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
            
            public Param(long userId, long rank, long userIconId, string userName, BigValue value, bool isCurrent, bool isPointOmission)
            {
                this.userId = userId;
                this.rank = rank;
                this.userIconId = userIconId;
                this.userName = userName;
                this.value = value;
                this.isCurrent = isCurrent;
                this.isPointOmission = isPointOmission;
            }
        }
        
        [SerializeField]
        private RankingOwnInfoView ownInfoView = null;
        
        protected override void OnSetView(object value)
        {
            Param param = (Param)value;
            
            // 各情報をセット
            ownInfoView.SetRank(param.Rank, param.IsCurrent);
            ownInfoView.SetUserIcon(param.UserIconId);
            ownInfoView.SetName(param.UserName);
            ownInfoView.SetPoint(param.Value, param.IsPointOmission);
            ownInfoView.SetUserId(param.UserId);
        }
    }
}