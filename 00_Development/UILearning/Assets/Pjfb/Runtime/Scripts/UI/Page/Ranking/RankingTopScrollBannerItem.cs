using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.Storage;
using UnityEngine.UI;

namespace Pjfb.Ranking
{
    public class RankingTopScrollBannerItem : ScrollGridItem
    {
        public class RankingBannerData
        {
            public class Param
            {
                private long id = new long();
                public long Id => id;
                private long imageId = new long();
                public long ImageId => imageId;

                public Param(long id, long imageId)
                {
                    this.id = id;
                    this.imageId = imageId;
                }
            }
            private List<Param> bannerParams = new List<Param>();
            public List<Param> BannerParams => bannerParams;
        }
        
        [SerializeField] RankingWebTexture bannerImage = null;
        [SerializeField] UIBadgeNotification badgeObject = null;
        protected override void OnSetView(object value)
        {
            RankingBannerData.Param data = (RankingBannerData.Param)value;
            bannerImage.SetTexture(data.ImageId);
            
            badgeObject.SetActive(LocalSaveManager.saveData.clientPreviewRankingIdConfirmList.Contains(data.Id) == false);
            
        }

        public void SetBannerData(RankingBannerData.Param data)
        {
            OnSetView(data);
        }
    }
}