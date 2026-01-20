using System;
using System.Collections.Generic;
using UnityEngine;
using Pjfb.Master;
using Pjfb.UI;
using Cysharp.Threading.Tasks;
using Logger = CruFramework.Logger;

namespace Pjfb.Shop
{

    public class ShopExchangeItemGroup : ListItemBase
    {
        public class Data : ItemParamsBase
        {
            public ExchangeStoreType exchangeStoreType;
            public List<CommonStoreCategoryMasterObject> storeList;
        }
        
        [SerializeField] private TMPro.TMP_Text titleText = null;
        [SerializeField] private Transform categoryRoot;
        [SerializeField] private ShopExchangeBanner bannerPrefab;

        private List<ShopExchangeBanner> storeBannerUiList = new List<ShopExchangeBanner>();
            
        private Data data;
        private bool isInitializeing;

        public override void Init(ItemParamsBase value)
        {
            if(isInitializeing) return;
            data = (Data)value;
            switch (data.exchangeStoreType)
            {
                case ExchangeStoreType.Limited:
                    titleText.text = StringValueAssetLoader.Instance["shop.exchange.limited"];
                    break;
                case ExchangeStoreType.Daily:
                    titleText.text = StringValueAssetLoader.Instance["shop.exchange.daily"];
                    break;
                case ExchangeStoreType.Item:
                    titleText.text = StringValueAssetLoader.Instance["shop.exchange.item"];
                    break;
            }
            SetShopExchangeBannerUiAsync(data.storeList).Forget();
        }

        private async UniTask SetShopExchangeBannerUiAsync(List<CommonStoreCategoryMasterObject> storeList)
        {
            isInitializeing = true;
            storeBannerUiList.ForEach(banner => banner.gameObject.SetActive(false));
            for(var i = 0; i < storeList.Count; i++)
            {
                if (storeBannerUiList.Count <= i)
                {
                    var obj = Instantiate(bannerPrefab.gameObject,categoryRoot);
                    var storeBannerUi = obj.GetComponent<ShopExchangeBanner>();
                    storeBannerUi.gameObject.SetActive(true);
                    storeBannerUiList.Add(storeBannerUi);
                }
                await storeBannerUiList[i].SetData(storeList[i]);
                if(this == null || this.gameObject == null)  return;
            }
            isInitializeing = false;
        }
        
    }
}