using System.Collections;
using System.Collections.Generic;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.Gacha
{
    public class GachaPendingFrameData
    {

        /// <summary>リトライできるか</summary>
        public bool CanRetry
        {
            get
            {
                // -1の場合は無限にできる
                if(retryLimit == -1) return true;
                return retryCount < retryLimit;
            }
        }


        public long Index => index;
        public long RetryId => retryId;
        public long RetryCount => retryCount;
        public long RetryLimit => retryLimit;
        public long RetryPointId => retryPointId;
        public long RetryPrice => retryPrice;

        private long index = 0; // ガチャ枠そのものindexを保持
		private long retryId = 0; // リトライ設定に対応するm_gacha_retry.id
        private long retryLimit = -1;
        private long retryCount = 0;
        private long retryPointId = 0;
        private long retryPrice = 0;
            
        
        public GachaPendingFrameData(GachaPendingFrame pendingFrame)
        {
            UpdateData(pendingFrame);
        }

        public void UpdateData(GachaPendingFrame pendingFrame)
        {
            index = pendingFrame.index;
            retryId = pendingFrame.retryId;
            retryCount = pendingFrame.count;
            retryLimit = MasterManager.Instance.gachaRetryMaster.FindData(pendingFrame.retryId).retryLimit;
            var priceMaster = MasterManager.Instance.gachaRetryPriceMaster.FindDateByRetryIdAndCount(pendingFrame.retryId, pendingFrame.count);
            if( priceMaster == null ) {
                return;
            }
            retryPointId = priceMaster.mPointId;
            retryPrice = priceMaster.value;
        }
    }
}
