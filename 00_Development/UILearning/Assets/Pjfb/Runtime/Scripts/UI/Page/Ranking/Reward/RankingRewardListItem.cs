using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.Ranking
{
    /// <summary>
    /// 報酬画面における縦並びのリストの各アイテムの振る舞いを定義するクラス
    /// </summary>
    public class RankingRewardListItem : ScrollGridItem
    {
        /// <summary>任意の順位における必要なパラメータ（報酬リスト含める）を定義するクラス</summary>
        public class Param
        {
            /// <summary>任意の報酬におけるランキング上限</summary>
            public long UpperRanking { get; private set; }
            
            /// <summary>任意の報酬におけるランキング下限</summary>
            public long LowerRanking { get; private set; }
            
            /// <summary>ランキング上限、下限における報酬内容</summary>
            public PrizeJsonWrap[] PrizeJsonWrap { get; private set; }
            
            /// <summary>ランクインのフレームを表示するか</summary>
            public bool IsFrameRankInShown { get; private set; }
            
            public Param(long upperRanking, long lowerRanking, PrizeJsonWrap[] prizeJsonWrap, bool isFrameRankInShown)
            {
                UpperRanking = upperRanking;
                LowerRanking = lowerRanking;
                PrizeJsonWrap = prizeJsonWrap;
                IsFrameRankInShown = isFrameRankInShown;
            }
        }
        
        /// <summary>横並びの報酬リストの参照を持つ</summary>
        [SerializeField]
        private ScrollGrid nestedListScrollGrid = null;
        
        /// <summary>ランクアイコンを貼り付けるImageオブジェクト</summary>
        [SerializeField]
        private Image rankIconImage = null;
        
        /// <summary>使用するランクアイコンの画像</summary>
        [SerializeField]
        private Sprite[] rankIconSprites = null;

        /// <summary>ランクアイコンとテキスト時にアクティブを切り替える</summary>
        [SerializeField]
        private GameObject parentRankText = null;
        
        /// <summary>範囲でランクを表示する際の上限、または、単一の順位の表示</summary>
        [SerializeField]
        private TextMeshProUGUI rankTopText = null;
        
        /// <summary>範囲でランクを表示する際の「～」</summary>
        [SerializeField]
        private TextMeshProUGUI rankMiddleText = null;

        /// <summary>範囲でランクを表示する際の下限</summary>
        [SerializeField]
        private TextMeshProUGUI rankBottomText = null;
        
        /// <summary>ランクインを表すフレーム</summary>
        [SerializeField]
        private GameObject frameRankIn = null;
        
        // 各リストアイテムにおける横並びの報酬リストのデータをセット
        protected override void OnSetView(object value)
        {
            // 任意の順位における報酬リストのデータとしてキャストする
            Param param = (Param)value;
            
            // 報酬部分を除くRank部分のViewをセットする
            SetRankView(param.UpperRanking, param.LowerRanking, param.IsFrameRankInShown);
            
            // 任意の順位における報酬リストの各アイテムのデータをセットする
            SetRankingRewardPrize(param.PrizeJsonWrap);
        }

        /// <summary>任意の順位におけるランク部分（ランクアイコン、テキスト、フレーム切替）のViewをセットする</summary>
        private void SetRankView(long upperRanking, long lowerRanking, bool isFrameRankInShown)
        {
            // 単一のランクか範囲のランクかを判定する
            if (upperRanking == lowerRanking)
            {
                // 単一のランクセット
                SetSingleRank(upperRanking);
            }
            else
            {
                // 範囲のランクセット
                SetRangeRank(upperRanking, lowerRanking);
            }
            
            // ランクインのフレームの表示切替
            frameRankIn.SetActive(isFrameRankInShown);
        }
        
        /// <summary>単一のランクを表示する</summary>
        private void SetSingleRank(long upperRanking)
        {
            // ランクアイコンが存在しているかを判定する
            if (upperRanking <= rankIconSprites.Length)
            {
                // ランクテキストを非表示にする
                parentRankText.SetActive(false);
                
                // 用意されているランクアイコンを使用する
                rankIconImage.gameObject.SetActive(true);
                rankIconImage.sprite = rankIconSprites[(int)upperRanking - 1];
            }
            else
            {
                // ランクテキストを表示する
                parentRankText.SetActive(true);
                    
                // ランクアイコンを非表示にする
                rankIconImage.gameObject.SetActive(false);
                    
                // 範囲のランク表示で使用するものは非アクティブにする
                rankMiddleText.gameObject.SetActive(false);
                rankBottomText.gameObject.SetActive(false);
                    
                // 単一のランクを表示する
                rankTopText.text = upperRanking.ToString();
                rankTopText.gameObject.SetActive(true);
            }
        }
        
        /// <summary>範囲のランクを表示する</summary>
        private void SetRangeRank(long upperRanking, long lowerRanking)
        {
            // ランクテキストを表示する
            parentRankText.SetActive(true);
            
            // ランクアイコンを非表示にする
            rankIconImage.gameObject.SetActive(false);
            
            // 上限のセット
            rankTopText.text = upperRanking.ToString();
            rankTopText.gameObject.SetActive(true);
                
            // 既に範囲を示す「～」がセットされているのでアクティブのみ切り替える
            rankMiddleText.gameObject.SetActive(true);
                
            // 下限のセット
            rankBottomText.text = lowerRanking.ToString();
            rankBottomText.gameObject.SetActive(true);
        }

        /// <summary>任意の順位における報酬リストの各アイテムのデータをセットする</summary>
        private void SetRankingRewardPrize(PrizeJsonWrap[] prizeJsonWraps)
        {
            // 報酬アイテムを横並びに配置するScrollGridに渡すデータを作成する
            List<RankingRewardNestedListItem.Param> paramList = new List<RankingRewardNestedListItem.Param>();
            foreach (PrizeJsonWrap prizeJsonWrap in prizeJsonWraps)
            {
                paramList.Add(new RankingRewardNestedListItem.Param(prizeJsonWrap));
            }
            
            // 報酬アイテムを横並びに配置するScrollGridにデータをセットする
            nestedListScrollGrid.SetItems(paramList);
        }
    }
}