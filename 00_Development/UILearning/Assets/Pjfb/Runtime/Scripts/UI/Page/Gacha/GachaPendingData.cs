using System;
using System.Collections;
using System.Collections.Generic;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.Gacha
{
    public class GachaPendingData
    {
        private long gachaSettingId = 0; 
        public long GachaSettingId { get { return gachaSettingId; } }
		private long gachaSettingType = 0;
        public long GachaSettingType { get { return gachaSettingType; } }
        private long storeMPointId = 0;
        public long StoreMPointId { get { return storeMPointId; } }
        private long gachaCategoryId = 0;
        public long GachaCategoryId { get { return gachaCategoryId; } }


        private long gachaResultPendingId = 0;
        /// <summary>保留Id</summary>
        public long GachaResultPendingId { get { return gachaResultPendingId; } }
        
        private DateTime expireAt = DateTime.MinValue;
        /// <summary>引き直し期間</summary>
        public DateTime ExpireAt { get { return expireAt; } }

        public GachaPendingData(GachaPendingInfo pendingInfo)
        {
            // 保留Id
            gachaResultPendingId = pendingInfo.uGachaResultPendingId;

            gachaSettingId = pendingInfo.mGachaSettingId;
            gachaSettingType = pendingInfo.gachaSettingType;
            storeMPointId =  pendingInfo.storeMPointId;
            gachaCategoryId = pendingInfo.mGachaCategoryId;
            // 引き直し期間
            if(DateTime.TryParse(pendingInfo.expireAt, out DateTime dateTime))
            {
                expireAt = dateTime;
            }
        }
    }
}
