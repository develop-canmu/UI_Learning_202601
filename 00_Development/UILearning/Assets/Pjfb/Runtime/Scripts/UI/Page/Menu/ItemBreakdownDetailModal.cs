using System;
using System.Linq;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;

namespace Pjfb.Menu
{
    public class ItemBreakdownDetailModal : ModalWindow
    {
        public class ItemData
        {
            private long itemId;
            public long ItemId => itemId;
            private NativeApiPointExpiry[] pointExpiryList;
            public NativeApiPointExpiry[] PointExpiryList => pointExpiryList;
            private NativeApiPointHistory[] pointHistoryList;
            public NativeApiPointHistory[] PointHistoryList => pointHistoryList;

            public ItemData(long itemId, NativeApiPointExpiry[] pointExpiryList, NativeApiPointHistory[] pointHistoryList)
            {
                this.itemId = itemId;
                this.pointExpiryList = pointExpiryList;
                this.pointHistoryList = pointHistoryList;
            }
        }

        [Serializable]
        private class BreakdownSheetData
        {
            [SerializeField] private BreakdownDetailItem breakdownDetailItem;
            public BreakdownDetailItem BreakdownDetailItem => breakdownDetailItem;
            [SerializeField] private Transform breakdownDetailItemRoot;
            public Transform BreakdownDetailItemRoot => breakdownDetailItemRoot;
            [SerializeField] private TextMeshProUGUI dataEmptyText;
            public TextMeshProUGUI DataEmptyText => dataEmptyText;
        }
        
        [SerializeField] private ItemIcon itemIcon;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemNumText;
        [SerializeField] private ItemBreakdownTabSheetManager tabSheetManager;
        [SerializeField] private BreakdownSheetData expiryDetailItem;
        [SerializeField] private BreakdownSheetData historyDetailItem;
        
        private ItemData data;
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            data = (ItemData) args;
            Init();
            return base.OnPreOpen(args, token);
        }

        private void Init()
        {
            itemIcon.SetTexture(data.ItemId);
            itemName.text = MasterManager.Instance.pointMaster.FindData(data.ItemId).name;
            itemNumText.text = UserDataManager.Instance.GetExpiryPointValue(data.ItemId).GetStringNumberWithComma();
            // 詳細情報を表示
            if(data.PointExpiryList == null || data.PointExpiryList.Length == 0)
            {
                // データがない時のテキスト表示
                expiryDetailItem.DataEmptyText.gameObject.SetActive(true);
                expiryDetailItem.BreakdownDetailItemRoot.gameObject.SetActive(false);
            }
            else
            {
                BreakdownDetailItem detailItem = null;
                expiryDetailItem.BreakdownDetailItemRoot.gameObject.SetActive(true);
                foreach (NativeApiPointExpiry pointExpiry in data.PointExpiryList)
                {
                    if(pointExpiry.mPointId != data.ItemId) continue;
                    detailItem = Instantiate(expiryDetailItem.BreakdownDetailItem, expiryDetailItem.BreakdownDetailItemRoot);
                    detailItem.gameObject.SetActive(true);
                    string dateTimeText = string.Format(StringValueAssetLoader.Instance["item.breakdown.end_at"], pointExpiry.expireAt);
                    BreakdownDetailItem.BreakdownDetailItemData detailData = new BreakdownDetailItem.BreakdownDetailItemData(dateTimeText,pointExpiry.value.GetStringNumberWithComma());
                    detailItem.SetItem(detailData);
                }
                // 最後の要素は下線を非表示にする
                if (detailItem != null)
                {
                    detailItem.HideLine();
                }
            }
            // 履歴表示
            if(data.PointHistoryList == null || data.PointHistoryList.Length == 0)
            {
                // データがない時のテキスト表示
                historyDetailItem.DataEmptyText.gameObject.SetActive(true);
                historyDetailItem.BreakdownDetailItemRoot.gameObject.SetActive(false);
            }
            else
            {
                historyDetailItem.BreakdownDetailItemRoot.gameObject.SetActive(true);
                // 履歴データを降順に並び替え
                IOrderedEnumerable<NativeApiPointHistory> histories = data.PointHistoryList.OrderByDescending(pointData => pointData.createdAt.TryConvertToDateTime());
                
                BreakdownDetailItem detailItem = null;
                foreach (NativeApiPointHistory pointHistory in histories)
                {
                    if(pointHistory.mPointId != data.ItemId) continue;
                    detailItem = Instantiate(historyDetailItem.BreakdownDetailItem, historyDetailItem.BreakdownDetailItemRoot);
                    detailItem.gameObject.SetActive(true);
                    string valueText = pointHistory.isIncome ? string.Format(StringValueAssetLoader.Instance["item.breakdown.history.income"],pointHistory.value.GetStringNumberWithComma()) : string.Format(StringValueAssetLoader.Instance["item.breakdown.history.outlay"],pointHistory.value.GetStringNumberWithComma());
                    BreakdownDetailItem.BreakdownDetailItemData detailData = new BreakdownDetailItem.BreakdownDetailItemData(pointHistory.createdAt,valueText,pointHistory.fluctuationRoute);
                    detailItem.SetItem(detailData,pointHistory.isIncome);
                }
                // 最後の要素は下線を非表示にする
                if (detailItem != null)
                {
                    detailItem.HideLine();
                }
            }
        }
    }
}