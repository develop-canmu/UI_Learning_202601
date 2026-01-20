using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb.Shop
{
    public class SecretBannerItem : ScrollGridItem
    {
        public class BannerData
        {
            private string imagePath = string.Empty;
            public string ImagePath => imagePath;
            private bool isRemind = false;
            public bool IsRemind => isRemind;
            public BannerData(string imagePath, bool isRemind)
            {
                this.imagePath = imagePath;
                this.isRemind = isRemind;
            }
        }
        
        
        [SerializeField] 
        private CancellableWebTexture campaignBannerImage;

        [SerializeField]
        private GameObject label;
        
        protected override void OnSetView(object value)
        {
            BannerData bannerData = (BannerData)value;
            SetBanner(bannerData.ImagePath).Forget();
            // リマインドラベルの表示非表示
            label.SetActive(bannerData.IsRemind);
        }
        
        private async UniTask SetBanner(string imagePath)
        {
            await campaignBannerImage.SetTextureAsync($"{AppEnvironment.AssetBrowserURL}/{imagePath}");
        }
    }
}