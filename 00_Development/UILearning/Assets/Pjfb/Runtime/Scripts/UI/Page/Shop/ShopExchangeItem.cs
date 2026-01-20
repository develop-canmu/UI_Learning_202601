using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.Common;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UniRx;

namespace Pjfb.Shop
{

    public class ShopExchangeItem : ScrollGridItem
    {
        //無期限に設定するendAt
        private static readonly DateTime indefinitePeriodDateTime = new DateTime(2099,1,1,0,0,0);


        [SerializeField] private TMPro.TMP_Text nameText;
        [SerializeField] private TMPro.TMP_Text closeDateText;
        [SerializeField] private TMPro.TMP_Text buyLimitText;
        [SerializeField] private PossessionItemUi costUi;
        [SerializeField] private PrizeJsonView prizeJsonView;
        [SerializeField] private UIButton exchangeButton;
        [SerializeField] private GameObject soldOutCover;
        
        private DateTime closedDateTime;
        private IDisposable updatePackClosedTimeDisposable;
        
        public class Data
        {
            public CommonStoreMasterObject commonStore;
            public long exchangedCount;
            public readonly Action<List<StoreBuyingInfo>> OnExchangedItem;
            public readonly Action OnTimeUp;

            public Data(CommonStoreMasterObject item, long count, Action<List<StoreBuyingInfo>> onExchangedItem, Action onTimeUp)
            {
                this.commonStore = item;
                this.exchangedCount = count;
                this.OnExchangedItem = onExchangedItem;
                OnTimeUp = onTimeUp;
            }
        }

        private Data data = null;
        
        protected override void OnSetView(object value)
        {
            data = (Data) value;
            nameText.text = data.commonStore.name;
            prizeJsonView.SetView(data.commonStore.prizeJson.FirstOrDefault());
            costUi.SetCount(data.commonStore.costMPointId,data.commonStore.costValue);
            
            var buyLimit = data.commonStore.buyLimit;
            var remainCount = Mathf.Max(0, buyLimit - data.exchangedCount);
            buyLimitText.text = buyLimit == 0 ? 
                StringValueAssetLoader.Instance["shop.exchange.unlimited"] : 
                string.Format(StringValueAssetLoader.Instance["common.ratio_value"], remainCount, buyLimit);

            bool hasRemainItem = buyLimit == 0 || remainCount > 0;
            soldOutCover.SetActive(!hasRemainItem);
            exchangeButton.interactable = (hasRemainItem) && 
                (UserDataManager.Instance.point.Find(data.commonStore.costMPointId)?.value ?? 0) >= data.commonStore.costValue;


            closedDateTime = DateTime.Parse(data.commonStore.closedDatetime);
            bool isIndefinitePeriod = IsIndefinitePeriod(closedDateTime);
            
            
            DisposePackClosedTimeEvent();
            closeDateText.gameObject.SetActive(!isIndefinitePeriod); 
            if (!isIndefinitePeriod)
            {
                AddPackClosedTimeDisposable();
                UpdateCloseDateText();
            }
        }
        

        public void OnClickExchangeButton()
        {
            // サポート器具上限チェック
            if (prizeJsonView.IconType == ItemIconType.SupportEquipment) 
            {
                if (SupportEquipmentManager.ShowOverLimitModal()) return;
            }
           ShopExchangeConfirmModal.Open(data.commonStore,data.exchangedCount, data.OnExchangedItem);
        }
        

        //期限のテキスト表示
        private void UpdateCloseDateText()
        {  
            var span = closedDateTime - AppTime.Now;
            
            if (span.TotalSeconds <= 0)
            {
                data.OnTimeUp?.Invoke();
                DisposePackClosedTimeEvent();
                return;
            }
            
            if( span.Days > 0 ) 
            {
                //日数表示
                closeDateText.text = string.Format(StringValueAssetLoader.Instance["shop.day_close_at"], span.Days );
            }
            else if( span.Hours > 0 ) 
            {
                //時間表示
                closeDateText.text = string.Format(StringValueAssetLoader.Instance["shop.hour_close_at"], span.Hours );
            } 
            else if(span.Minutes > 0)
            {
                //分数表示
                closeDateText.text = string.Format(StringValueAssetLoader.Instance["shop.minutes_close_at"], span.Minutes );
            }
            else
            {
                closeDateText.text = string.Format(StringValueAssetLoader.Instance["shop.second_close_at"], span.Seconds );
            }

            
        }

        //無期限かどうか
        private bool IsIndefinitePeriod(DateTime endAt)
        {
            return endAt >= indefinitePeriodDateTime;
        }
        
        private void AddPackClosedTimeDisposable()
        {
            DisposePackClosedTimeEvent();
            updatePackClosedTimeDisposable =  Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
            { 
                UpdateCloseDateText();
            }).AddTo(gameObject);
        }
        
        private void DisposePackClosedTimeEvent()
        {
            if (updatePackClosedTimeDisposable != null)
            {
                updatePackClosedTimeDisposable.Dispose();
                updatePackClosedTimeDisposable = null;
            }
        }
    }
}