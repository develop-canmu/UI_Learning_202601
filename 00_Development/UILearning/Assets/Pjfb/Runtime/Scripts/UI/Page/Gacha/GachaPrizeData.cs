using System.Collections;
using System.Collections.Generic;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.Gacha
{
    public class GachaPrizeData
    {
        private PrizeJsonViewData[] contentList = null;
        /// <summary>報酬の一覧</summary>
        public PrizeJsonViewData[] ContentList { get { return contentList; } }
        
        private GachaPendingFrameData pendingData = null;
        /// <summary>この枠の保留情報</summary>
        public GachaPendingFrameData PendingData { get { return pendingData; } }
        
        public GachaPrizeData(PrizeJsonWrap[] prizeList)
        {
            SetContentListData(prizeList);
        }
        
        /// <summary>報酬一覧セット</summary>
        public void SetContentListData(PrizeJsonWrap[] prizeList)
        {
            // メモリ確保
            contentList = new PrizeJsonViewData[prizeList.Length];
            // データセット
            for(int i = 0; i < contentList.Length; i++)
            {
                contentList[i] = new PrizeJsonViewData(prizeList[i]);
            }
        }
        
        /// <summary>保留情報のセット</summary>
        public void SetPendingData(GachaPendingFrame pendingFrame)
        {
            pendingData = new GachaPendingFrameData(pendingFrame);
        }

        public void ClearPendingData()
        {
            pendingData = null;
        }
    }
}
