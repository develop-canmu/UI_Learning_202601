using UnityEngine;
using CruFramework.UI;
using Pjfb.Networking.App.Request;

namespace Pjfb.Ranking
{
    /// <summary>任意の順位で獲得できる報酬リストの各アイテム（アイテムを横並びで配置）の振る舞いを定義するクラス</summary>
    public class RankingRewardNestedListItem : ScrollGridItem
    {
        /// <summary>任意の順位で獲得できる報酬リストの各アイテムを表示するために必要なデータを定義するクラス</summary>
        public class Param
        {
            /// <summary>特定の順位における報酬内容</summary>
            public PrizeJsonWrap PrizeJsonWrap { get; private set; } 
            
            public Param(PrizeJsonWrap prizeJsonWrap)
            {
                PrizeJsonWrap = prizeJsonWrap;
            }
        }
        
        /// <summary>報酬の内容を表示するView</summary>
        [SerializeField]
        private PrizeJsonView prizeJsonView = null;
        
        // TODO: Viewにデータをセットする処理を実装する
        protected override void OnSetView(object value)
        {
            Param param = (Param)value;
            
            // 任意の順位で獲得できる報酬リストは専用のViewクラスで処理する
            prizeJsonView.SetView(param.PrizeJsonWrap);
        }
    }
}