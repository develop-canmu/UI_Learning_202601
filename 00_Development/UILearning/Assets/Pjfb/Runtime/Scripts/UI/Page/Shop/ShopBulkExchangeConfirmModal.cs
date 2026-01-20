using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using PrizeJsonWrap = Pjfb.Master.PrizeJsonWrap;

namespace Pjfb.Shop
{
    public class ShopBulkExchangeConfirmModal : ModalWindow
    {

        public class ObtainItemData
        {
            private long itemNum;
            public long ItemNum
            {
                get { return itemNum;}
                set { itemNum = value; }
            }
            
            private PrizeJsonWrap prizeJsonWrap;
            public PrizeJsonWrap PrizeJsonWrap
            {
                get { return prizeJsonWrap; }
                set { prizeJsonWrap = value; }
            }
        }
        
        public class BulkExchangeData
        {
            private Dictionary<long, ObtainItemData> obtainItem;    // 入手アイテム(Id, ObtainExchangeData)
            public Dictionary<long,ObtainItemData> ObtainItem 
            {
                get { return obtainItem; }
                set { obtainItem = value; }
            }
            private Dictionary<long,long> consumptionItem;  // 消費アイテム(costMPointId, 消費アイテム数)
            public Dictionary<long, long> ConsumptionItem
            {
                get { return consumptionItem; }
                set { consumptionItem = value; }
            }
            private Action<List<StoreBuyingInfo>> onExchangedItem;
            public Action<List<StoreBuyingInfo>> OnExchangedItem
            {
                get { return onExchangedItem; }
                set { onExchangedItem = value; }
            }
            public BulkExchangeData()
            {
                obtainItem = new Dictionary<long, ObtainItemData>();
                consumptionItem = new Dictionary<long, long>();
            }
        }

        [SerializeField] private ScrollGrid consumptionItemScrollGrid;
        [SerializeField] private ScrollGrid obtainItemItemScrollGrid;
        
        private BulkExchangeData bulkExchangeData;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            bulkExchangeData = (BulkExchangeData)args;
            InitializeUI();
            return base.OnPreOpen(args, token);
        }

        public void OnClickPositiveButton()
        {
            RequestShopBuyStoreApi().Forget();
        }

        // 交換確認の各種ItemIconの初期化
        private void InitializeUI()
        {
            // ScrollGridにアイテムをセット
            List<ItemIconGridItem.Data> consumptionItemIconGridItemDataList = new List<ItemIconGridItem.Data>();
            List<ItemIconGridItem.Data> obtainItemIconGridItemDataList = new List<ItemIconGridItem.Data>();
            foreach (var consumptionItem in bulkExchangeData.ConsumptionItem.OrderBy(consumptionItemData => consumptionItemData.Key))
            {
                ItemIconGridItem.Data itemIconGridItemData = new ItemIconGridItem.Data(consumptionItem.Key, consumptionItem.Value);
                consumptionItemIconGridItemDataList.Add(itemIconGridItemData);
            }
            foreach (var obtainItemValue in bulkExchangeData.ObtainItem.Values.OrderBy(value => value.PrizeJsonWrap.args.mPointId))
            {
                ItemIconGridItem.Data itemIconGridItemData = new ItemIconGridItem.Data(obtainItemValue.PrizeJsonWrap.args.mPointId, obtainItemValue.ItemNum);
                obtainItemIconGridItemDataList.Add(itemIconGridItemData);
            }
            consumptionItemScrollGrid.SetItems(consumptionItemIconGridItemDataList);
            obtainItemItemScrollGrid.SetItems(obtainItemIconGridItemDataList);
        }
        
        // ShopBuyStoreAPIを叩く
        private async UniTask RequestShopBuyStoreApi()
        {
            var request = new ShopBuyStoreAPIRequest();
            var post = new ShopBuyStoreAPIPost
            { 
                idList = bulkExchangeData.ObtainItem.Keys.ToArray(),
                countList = bulkExchangeData.ObtainItem.Values.Select(x => x.ItemNum).ToArray(),
            };
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);

            List<StoreBuyingInfo> storeBuyingInfoList = await RequestGetShopGetStoreBuyingInfoAPI();
            bulkExchangeData.OnExchangedItem?.Invoke(storeBuyingInfoList);
            
            // prizeJsonに交換した数をセット
            List<PrizeJsonWrap> exchangeDataList = new List<PrizeJsonWrap>();
            foreach (var value in bulkExchangeData.ObtainItem.Values.OrderBy(obtainItemData => obtainItemData.PrizeJsonWrap.args.mPointId))
            {
                PrizeJsonWrap data = new PrizeJsonWrap(value.PrizeJsonWrap);
                data.args.value = value.ItemNum;
                exchangeDataList.Add(data);
            }
            // 交換成功画面を開く
            ShopExchangeSuccessConfirmModal.Parameters parameters = new ShopExchangeSuccessConfirmModal.Parameters(
                exchangeDataList,
                () => AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop((window) =>
                    window is not ShopExchangeModal));
            ShopExchangeSuccessConfirmModal.Open(parameters);
        }

        private UniTask<List<StoreBuyingInfo>> RequestGetShopGetStoreBuyingInfoAPI()
        {
            return  ShopPage.GetShopGetStoreBuyingInfoAPI();
        }
        
        public void OnClickClose()
        {
            Close();
        }
        
    }
}