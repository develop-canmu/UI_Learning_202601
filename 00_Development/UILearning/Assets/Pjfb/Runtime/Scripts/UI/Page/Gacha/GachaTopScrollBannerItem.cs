using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework.UI;
using UnityEngine;

namespace Pjfb.Gacha
{
    public class GachaTopScrollBannerItem : ScrollGridItem
    {
        [SerializeField]
        private GachaTopSmallBannerWebTexture smallBannerTexture = null;

        GachaTopScrollBannerData _data = null;

        protected override void OnSetView(object value)
        {
            _data = (GachaTopScrollBannerData)value;
            // 画像セット
            smallBannerTexture.SetTexture(_data.DesignNumber);
        }

        public void OnClickBanner() {
            _data.OnClickBanner?.Invoke(this);
        }
    }
}
