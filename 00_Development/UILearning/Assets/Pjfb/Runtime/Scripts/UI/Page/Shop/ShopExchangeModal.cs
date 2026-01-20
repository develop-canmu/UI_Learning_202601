using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using CruFramework.UI;
using Pjfb.Master;
using TMPro;
using UniRx;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Common;
using Pjfb.UserData;

namespace Pjfb.Shop
{
    // Todo : refactor
    public class ShopExchangeModal : ModalWindow
    {
        
        [SerializeField] private ScrollGrid exchangeScrollGrid;
        [SerializeField] private UIButton updateButton;
        [SerializeField] private UIButton exchangeAllButton;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text updateCountText;
        [SerializeField] private TMP_Text updateLimitText;
        [SerializeField] private PossessionItemUi possessionItemBase;
        [SerializeField] private Transform possessionRoot;
        [SerializeField] private GameObject dailyUiRoot;
        [SerializeField] private GameObject updateCountRoot;

        private class Data
        {
            public readonly CommonStoreCategoryMasterObject commonStoreCategory;
            public readonly Action onExchangeItem;
            public Data(CommonStoreCategoryMasterObject commonStoreCategory, Action onExchangeItem = null)
            {
                this.commonStoreCategory = commonStoreCategory;
                this.onExchangeItem = onExchangeItem;
            }
        }
        
        
        private Dictionary<long,StoreBuyingInfo> storeBuyingInfoDictionary;
        private List<CommonStoreMasterObject> commonStoreList;
        private long[] dailyCommonStoreIdList;
        private ShopBulkExchangeConfirmModal.BulkExchangeData bulkExchangeData;
        private Dictionary<long, long> possessionItem;  // 所持アイテム(costMPointId, 所持アイテム数)
        
        private List<PossessionItemUi> possessionItemUiList = new List<PossessionItemUi>();
        public Action OnUpdateLimit;
        private long updateCount;
        private DateTime endDate;
        private IDisposable updateExitTimeDisposable;
        private bool getShopUpdateInfo;
        private Data data;
        private bool refreshScroller = true;
        private bool reloadScroller = true;
        private List<ShopExchangeItem.Data> exchangeStoreDataList = new List<ShopExchangeItem.Data>();
        private bool isDailyStore;
        private bool readdExitTimeEvent;
        
        public static void Open(long commonStoreCategoryId, Action onExchangeItem = null)
        {
            var commonStoreCategory = MasterManager.Instance.commonStoreCategoryMaster.FindData(commonStoreCategoryId);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ShopExchange, new Data(commonStoreCategory, onExchangeItem));
        }
        
        public static void Open(CommonStoreCategoryMasterObject commonStoreCategory, Action onExchangeItem = null)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ShopExchange,new Data(commonStoreCategory, onExchangeItem));
        }
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            getShopUpdateInfo = true;
            data = (Data)args;
            isDailyStore =  data.commonStoreCategory.mCommonStoreLotteryCostCategoryId != 0;
            refreshScroller = false;
            reloadScroller = true;
            readdExitTimeEvent = false;
            await InitializeUi();
            
            await base.OnPreOpen(args, token);
        }

        
        protected override void OnOpened()
        {
            base.OnOpened();
            if (reloadScroller)
            {
                SetExchangeItemListUi();
                reloadScroller = false;
                refreshScroller = false;
            }
            else if (refreshScroller)
            {
                exchangeScrollGrid.RefreshItemView();
                refreshScroller = false;
            }

            if (readdExitTimeEvent) AddExitTimeEvent();
       
        }

        protected override void OnClosed()
        {
            if (isDailyStore)
            {
                DisposeExitTimeEvent(); 
                readdExitTimeEvent = true;   
            }
        }

        private async UniTask InitializeUi(List<StoreBuyingInfo> infoList = null)
        {
            bool refreshExchangeItemList = false;
            if (infoList is null)
            {
                infoList = await ShopPage.GetShopGetStoreBuyingInfoAPI();
            }
            else
            {
                refreshExchangeItemList = true;
                refreshScroller = true;
            }
            storeBuyingInfoDictionary = infoList.ToDictionary(x => x.mCommonStoreId, x=> x);
            
            var storeItems = MasterManager.Instance.commonStoreMaster.GetCommonStoreByCategory(data.commonStoreCategory.id);
     
            if (isDailyStore)
            {
                if (getShopUpdateInfo)
                {
                    await GetShopGetLotteryStoreInfoAPI();
                }
                else
                {
                    refreshExchangeItemList = true;
                }
                storeItems = storeItems.Where(item => dailyCommonStoreIdList.Contains(item.id));
            }
            SetDailyStoreUi(isDailyStore);
            
            commonStoreList = storeItems.ToList();
            SortCommonStoreList(storeBuyingInfoDictionary);

            if(refreshExchangeItemList)
                RefreshExchangeItemListUi();
            
            titleText.text = data.commonStoreCategory.name;
            
            exchangeAllButton.gameObject.SetActive(data.commonStoreCategory.bulkButtonVisibleFlg);
            
            await SetPossessionItemUiAsync(commonStoreList);
            
            // 一括交換が有効の場合
            if (data.commonStoreCategory.bulkButtonVisibleFlg)
            {
                InitializeBulkExchangeData();
                BulkExchangeItemCalculation();
                bulkExchangeData.OnExchangedItem = OnExchangedItem;
            }
        }

        
        

        private void SetExchangeItemListUi()
        {
            exchangeStoreDataList = new List<ShopExchangeItem.Data>();

            SortCommonStoreList(storeBuyingInfoDictionary);
            
            commonStoreList.ForEach(x =>
            {
                var exchangedCount = storeBuyingInfoDictionary.GetValueOrDefault(x.id)?.boughtCount ?? 0;
                var storeData = new ShopExchangeItem.Data(x, exchangedCount, OnExchangedItem, OnPackTimeOut);
                exchangeStoreDataList.Add(storeData);
            });
            exchangeScrollGrid.SetItems(exchangeStoreDataList);
        }

        private void SortCommonStoreList(Dictionary<long,StoreBuyingInfo> storeBuyingInfoDictionary)
        {
            commonStoreList = commonStoreList.OrderBy(x =>
            {
                var exchangedCount = storeBuyingInfoDictionary.GetValueOrDefault(x.id)?.boughtCount ?? 0;
                return x.buyLimit > 0 && x.buyLimit <= exchangedCount;
            }).ThenByDescending(x => x.sortPriority).ToList();
        }
        
        
        private void RefreshExchangeItemListUi()
        {
            if (commonStoreList.Count != exchangeStoreDataList.Count)
            {
                reloadScroller = true;
                return;
            }

            int itemCount = exchangeStoreDataList.Count;
            for (int i = 0; i < itemCount; i++)
            {
                if (commonStoreList[i].mCommonStoreCategoryId == exchangeStoreDataList[i].commonStore.id) continue;
                reloadScroller = true;
                return;
            }
            
            var commonStoreDictionary = commonStoreList.ToDictionary(x => x.id, x => x);
            foreach (var info in storeBuyingInfoDictionary.Values)
            {
                var cellData = exchangeStoreDataList.FirstOrDefault(x => x.commonStore.id == info.mCommonStoreId);
                if(cellData is null) continue;
                cellData.commonStore = commonStoreDictionary.GetValueOrDefault(cellData.commonStore.id);
                cellData.exchangedCount = info.boughtCount;
            }
        }


        private void SetDailyStoreUi(bool isDailyStore)
        {
            dailyUiRoot.SetActive(isDailyStore);
            if (isDailyStore)
            {
                var updateLimit =
                    MasterManager.Instance.commonStoreLotteryCostMaster.GetCommonStoreUpdateLimitByLotteryGroupId(
                        data.commonStoreCategory.mCommonStoreLotteryCostCategoryId);
                var remainUpdateCount = Mathf.Max(0, updateLimit - updateCount);
                updateCountText.text = string.Format(StringValueAssetLoader.Instance["shop.exchange.badge"], remainUpdateCount,updateLimit);
                updateButton.interactable = updateLimit == 0 || updateCount < updateLimit;
                updateCountRoot.SetActive(updateLimit != 0);
                endDate = AppTime.Now.Date.AddDays(1);
                AddExitTimeEvent();
            }
        }
        
        private void AddExitTimeEvent()
        {
            DisposeExitTimeEvent();
            SetExitTimeText();
            updateExitTimeDisposable = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
            {
                SetExitTimeText();
            }).AddTo(gameObject);
            readdExitTimeEvent = false;
        }
        
        private void SetExitTimeText()
        {
            var remainTimeSpan = endDate - AppTime.Now;
            var hours = remainTimeSpan.Hours > 0 ? remainTimeSpan.Hours : 0;
            var minutes = remainTimeSpan.Minutes > 0 ? remainTimeSpan.Minutes : 0;
            var seconds = remainTimeSpan.Seconds > 0 ? remainTimeSpan.Seconds : 0;
            var text = StringValueAssetLoader.Instance["shop.exchange.remain_time"];
            text += hours > 0
                ? $"{hours}{StringValueAssetLoader.Instance["common.hours"]}{minutes}{StringValueAssetLoader.Instance["common.minutes"]}"
                : $"{minutes}{StringValueAssetLoader.Instance["common.minutes"]}{seconds}{StringValueAssetLoader.Instance["common.seconds"]}";
            updateLimitText.text = text;
            if (IsUpdateLimit())
            {
                DisposeExitTimeEvent();
                // TODO 日を跨いだ場合どうするのか確認(ドラスマではタイトルに戻す)
                OnUpdateLimit?.Invoke();
            }
        }
        
        private bool IsUpdateLimit()
        {
            // DateTime.Nowだと端末時間に依存するためAppTime.Nowでサーバーの時間を使用するようにする
            var remainTimeSpan = endDate - AppTime.Now;
            return remainTimeSpan <= TimeSpan.Zero;
        }

        private void DisposeExitTimeEvent()
        {
            if (updateExitTimeDisposable != null)
            {
                updateExitTimeDisposable.Dispose();
                updateExitTimeDisposable = null;
            }
        }
       
        private async UniTask SetPossessionItemUiAsync(List<CommonStoreMasterObject> storesItemList)
        {
            var possessionItemIdList = new List<long>();
            foreach (var item in storesItemList.OrderBy(itemData => itemData.id))
            {
                var pointId = item.costMPointId;
                if (possessionItemIdList.Contains(pointId))
                {
                    continue;
                }
                possessionItemIdList.Add(pointId);
            }

            possessionItemUiList.ForEach(possessionItem => possessionItem.gameObject.SetActive(false));
            for(var i = 0; i < possessionItemIdList.Count; i++)
            {
                if (possessionItemUiList.Count <= i)
                {
                    var obj = Instantiate(possessionItemBase.gameObject,possessionRoot);
                    var possessionItemUi = obj.GetComponent<PossessionItemUi>();
                    possessionItemUiList.Add(possessionItemUi);
                }
                possessionItemUiList[i].SetPossessionUi(possessionItemIdList[i]);
                possessionItemUiList[i].gameObject.SetActive(true);
                await UniTask.DelayFrame(1);
            }
        }
        
        // 一括交換の際に必要な各データの取得
        private void InitializeBulkExchangeData()
        {
            bool enoughItem = false;
            possessionItem = new Dictionary<long, long>();
            foreach (var storeItem in commonStoreList)
            {
                long possessionItemNum;
                // 所持アイテムの集計を行っているか
                if (possessionItem.ContainsKey(storeItem.costMPointId) == false)
                {
                    possessionItemNum = UserDataManager.Instance.point.Find(storeItem.costMPointId)?.value ?? 0;
                    possessionItem.Add(storeItem.costMPointId,possessionItemNum);
                }
                else
                {
                    possessionItemNum = possessionItem[storeItem.costMPointId];
                }
                
                // 在庫確認
                if (storeItem.buyLimit > 0)
                {
                    long boughtNum = storeBuyingInfoDictionary.TryGetValue(storeItem.id, out var storeBuyingInfo)
                        ? storeBuyingInfo.boughtCount : 0;
                    long stockNum = storeItem.buyLimit - boughtNum;
                    if (stockNum <= 0) continue;
                }
                
                // 交換可能か判定
                if(storeItem.costValue <= possessionItemNum)
                {
                    enoughItem = true;
                }
            }
            exchangeAllButton.interactable = enoughItem;
        }
        
        // 一括交換の計算処理
        private void BulkExchangeItemCalculation()
        {
            bulkExchangeData = new ShopBulkExchangeConfirmModal.BulkExchangeData();
            // ここで個数の計算処理をする
            foreach (var storeItem in commonStoreList.OrderByDescending(commonMasterObjectData => commonMasterObjectData.priority))
            {
                // 所持アイテム数が0なら計算しない
                if(possessionItem[storeItem.costMPointId] <= 0) continue;
                long limit = storeItem.buyLimit;
                long exchangeItemNum = possessionItem[storeItem.costMPointId] / storeItem.costValue;
                // アイテム交換上限数が設定されている場合
                if (storeItem.buyLimit > 0)
                {
                    // 交換履歴があった場合上限を変更
                    if (storeBuyingInfoDictionary.ContainsKey(storeItem.id))
                    {
                        limit = storeItem.buyLimit - storeBuyingInfoDictionary[storeItem.id].boughtCount;
                    }
                    // 交換上限を超えている場合
                    if (exchangeItemNum > limit)
                    {
                        exchangeItemNum = limit;
                    }
                }

                // 交換できない場合次へ
                if(exchangeItemNum <= 0) continue;  
                
                long consumptionItemNum = exchangeItemNum * storeItem.costValue;
                // 所持アイテム量の更新
                possessionItem[storeItem.costMPointId] -= consumptionItemNum;
                // 各種データの更新
                ShopBulkExchangeConfirmModal.ObtainItemData obtainItemData = new ShopBulkExchangeConfirmModal.ObtainItemData();
                obtainItemData.PrizeJsonWrap = storeItem.prizeJson.FirstOrDefault();
                obtainItemData.ItemNum = exchangeItemNum;
                bulkExchangeData.ObtainItem.Add(storeItem.id, obtainItemData);
                if(bulkExchangeData.ConsumptionItem.ContainsKey(storeItem.costMPointId))
                {
                    bulkExchangeData.ConsumptionItem[storeItem.costMPointId] += consumptionItemNum;
                }
                else
                {
                    bulkExchangeData.ConsumptionItem.Add(storeItem.costMPointId, consumptionItemNum);
                }
            }
        }
        
        
        #region EventListeners
        public void OnClickClose()
        {
            Close();
        }
        public void OnClickExchangeUpdateButton()
        {
            ShopExchangeUpdateModal.Open(data.commonStoreCategory, updateCount, OnUpdateDailyStore);
        }
        // 全て交換を押したとき
        public void OnClickExchangeAllButton()
        {
            // 一括交換確認モーダルを開く
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ShopBulkExchangeConfirm,bulkExchangeData);
        }

        private async UniTask OnUpdateDailyStore(ShopLotStoreAPIResponse response)
        {
            updateCount = response.lottedCount;
            dailyCommonStoreIdList = response.mCommonStoreIdList;
            refreshScroller = true;
            readdExitTimeEvent = false;
            await InitializeUi();
        }

        private void OnExchangedItem(List<StoreBuyingInfo> infoList)
        {
            InitializeUi(infoList).Forget();
            data.onExchangeItem?.Invoke();
        }
        
        private void OnPackTimeOut()
        {
            // DateTime.Nowだと端末時間に依存するためAppTime.Nowでサーバーの時間を使用するようにする
            commonStoreList.RemoveAll(x =>
                !ShopManager.IsAvailableDateTime(x.releaseDatetime, x.closedDatetime, AppTime.Now));
            SetExchangeItemListUi();
        }
        #endregion
        
        #region API
        
        private async UniTask GetShopGetLotteryStoreInfoAPI()
        {
            ShopGetLotteryStoreInfoAPIRequest request = new ShopGetLotteryStoreInfoAPIRequest();
            
            var post = new ShopGetLotteryStoreInfoAPIPost();
            post.mCommonStoreCategoryId = data.commonStoreCategory.id;
            request.SetPostData(post);
            try
            {
                await APIManager.Instance.Connect(request);
                ShopGetLotteryStoreInfoAPIResponse response = request.GetResponseData();
                updateCount = response.lottedCount;
                dailyCommonStoreIdList = response.mCommonStoreIdList;
                getShopUpdateInfo = false;
            }
            catch (APIException)
            {
                // Apiエラーだった場合はログを出して後の処理を行わない
                CruFramework.Logger.LogError("ShopGetLotteryStoreInfoAPI error");
            }
        }
        

        #endregion

    }
   
}
