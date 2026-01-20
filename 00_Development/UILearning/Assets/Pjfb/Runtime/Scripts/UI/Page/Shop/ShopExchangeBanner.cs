using CruFramework;
using Pjfb.Master;
using Pjfb.Shop;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    public class ShopExchangeBanner : CancellableWebTexture
    {
        private CommonStoreCategoryMasterObject commonStoreCategory;
        public async UniTask SetData(CommonStoreCategoryMasterObject store)
        {
            commonStoreCategory = store;
            var key = ShopManager.GetStoreBannerPath(store.id);
            await SetTextureAsync(key);
        }
        public void OnClickBanner()
        {
            ShopExchangeModal.Open(commonStoreCategory);
        }
    }
}