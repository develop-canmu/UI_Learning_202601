using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Gacha
{
    public class GachaTopScrollBannerData
    {
        private long designNumber = 0;
        private Action<ScrollGridItem> onClickBanner = null;

        /// <summary>デザイン番号</summary>
        public long DesignNumber { get { return designNumber; } }
        public Action<ScrollGridItem> OnClickBanner { get { return onClickBanner; } }
        
        public GachaTopScrollBannerData(long designNumber, Action<ScrollGridItem> onClickBanner)
        {
            this.designNumber = designNumber;
            this.onClickBanner = onClickBanner;
        }

    }
}