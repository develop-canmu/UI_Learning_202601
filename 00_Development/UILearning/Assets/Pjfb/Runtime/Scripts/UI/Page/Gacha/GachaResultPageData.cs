using System.Collections;
using System.Collections.Generic;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.Gacha
{
    public class GachaResultPageData
    {
        private GachaPrizeData[] prizeList = null;
        /// <summary>ガチャ報酬一覧</summary>
        public GachaPrizeData[] PrizeList { get { return prizeList; } }

        private PrizeJsonViewData[] autoGotList = null;
        /// <summary>自動売却結果</summary>
        public PrizeJsonViewData[] AutoGotList { get { return autoGotList; } }
        
        private bool canShowReview = false;
        /// <summary>レビューの強制表示を行うか</summary>
        public bool CanShowReview { get { return canShowReview; } }
        
        private GachaPendingData pendingData = null;
        /// <summary>保留情報</summary>
        public GachaPendingData PendingData { get { return pendingData; } }
        
        private GachaCategoryData gachaCategoryData = null;
        /// <summary>実行データ</summary>
        public GachaCategoryData GachaCategoryData { get { return gachaCategoryData; } }

        private long exchangePointId = 0;
        /// <summary>交換ポイントId</summary>
        public long ExchangePointId{ get { return exchangePointId; } }

        private long beforePoint = 0;
        /// <summary>実行前の交換ポイント所持数</summary>
        public long BeforePoint{ get { return beforePoint; } }

        private long afterPoint = 0;
        /// <summary>実行後の交換ポイント所持数</summary>
        public long AfterPoint{ get { return afterPoint; } }


        private List<GachaRushData> rushDataList = null;
        /// <summary>ガチャラッシュのデータリスト</summary>
        public List<GachaRushData> RushDataList{ get { return rushDataList; } }

        private long[] effectDatas = null;
        /// <summary>演出データ</summary>
        public long[] EffectDatas { get { return effectDatas; } }

        /// <summary>保留画面に戻ってきたか</summary>
        public bool IsComebackPage { get;set; } = false;
        
        
        public GachaResultPageData(GachaPendingInfo pendingInfo, GachaCategoryData gachaCategoryData, long[] effectDatas, RushRushCategoryInfo[] rushCategoryInfoList)
        {
            // 保留情報
            pendingData = new GachaPendingData(pendingInfo);
            // ガチャ報酬
            SetPrizeList(pendingInfo.contentListList);
            // 各枠の保留状況
            for(int i = 0; i < pendingInfo.frameList.Length; i++)
            {
                int prizeListIndex = (int)pendingInfo.frameList[i].index;
                prizeList[prizeListIndex].SetPendingData(pendingInfo.frameList[i]);
            }
            // 実行データ
            this.gachaCategoryData = gachaCategoryData;
            if( rushCategoryInfoList != null && rushCategoryInfoList.Length > 0 ) {
                rushDataList = new List<GachaRushData>();
                foreach( var rushCategory in rushCategoryInfoList ){
                    var rushData = new GachaRushData(rushCategory);
                    if (rushData.IsHiddenRush()) continue;
                    rushDataList.Add(rushData);
                }
            }
            // 演出データ
            this.effectDatas = effectDatas;
        }
        
        public GachaResultPageData(WrapperPrizeList[] contentListList, NativeApiAutoSell autoSell, bool canShowReview, RushRushCategoryInfo[] rushCategoryInfoList, GachaCategoryData gachaCategoryData, long[] effectDatas)
        {
            // 保留情報
            pendingData = null;
            // ガチャ報酬
            SetPrizeList(contentListList);
            // 自動売却結果
            SetAutoGotList(autoSell.prizeListGot);
            // レビューするかどうか
            this.canShowReview = canShowReview;
            // 実行データ
            this.gachaCategoryData = gachaCategoryData;
            if( rushCategoryInfoList != null && rushCategoryInfoList.Length > 0 ) {
                rushDataList = new List<GachaRushData>();
                foreach( var rushCategory in rushCategoryInfoList ){
                    var rushData = new GachaRushData(rushCategory);
                    if (rushData.IsHiddenRush()) continue;
                    rushDataList.Add(rushData);
                }
            }

            // 演出データ
            this.effectDatas = effectDatas;
        }
        
        /// <summary>保留状態の枠の引き直しをした</summary>
        public void UpdateByPendingRetry(int index, GachaPendingRetryAPIResponse response)
        {
            prizeList[index].SetContentListData(response.contentList);
            prizeList[index].SetPendingData(response.frame);
        }
        
        /// <summary>保留を確定した</summary>
        public void UpdateByPendingFix(GachaPendingFixAPIResponse response)
        {
            // レビュー表示フラグ更新
            canShowReview = response.canShowReview;
            gachaCategoryData.SettingData.UpdateEnable(response.enabled);
            // 自動売却情報更新
            SetAutoGotList(response.autoSell.prizeListGot);
        }

        /// <summary>ポイントデータ設定</summary>
        public void SetPointData(long exchangePointId, long before, long after )
        {
            this.exchangePointId = exchangePointId;
            this.beforePoint = before;
            this.afterPoint = after; 
        }

        /// <summary>ポイントデータ設定</summary>
        public void SetPointData(long after )
        {
            this.afterPoint = after; 
        }

        public void ClearPendingData()
        {
            pendingData = null;
            // 各枠の保留状況
            for(int i = 0; i < prizeList.Length; i++)
            {
                prizeList[i].ClearPendingData();
            }
        }

        
        
        /// <summary>報酬情報をセット</summary>
        private void SetPrizeList(WrapperPrizeList[] contentListList)
        {
            // メモリ確保
            prizeList = new GachaPrizeData[contentListList.Length];
            // データセット
            for(int i = 0; i < prizeList.Length; i++)
            {
                prizeList[i] = new GachaPrizeData(contentListList[i].l);
            }
        }
        
        /// <summary>自動売却情報をセット</summary>
        private void SetAutoGotList(PrizeJsonWrap[] autoGotList)
        {
            if( autoGotList == null ) {
                return;
            }
            
            // メモリ確保
            this.autoGotList = new PrizeJsonViewData[autoGotList.Length];
            // データセット
            for(int i = 0; i < autoGotList.Length; i++)
            {
                this.autoGotList[i] = new PrizeJsonViewData(autoGotList[i]);
            }
        }
    }
}
