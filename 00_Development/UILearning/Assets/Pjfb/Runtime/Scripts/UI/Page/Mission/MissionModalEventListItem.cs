using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using Pjfb.Master;
using UnityEngine;
using Pjfb.UI;
using UnityEngine.UI;


namespace Pjfb.Common
{
    public class MissionModalEventListItem : PoolListItemBase
    {
        #region Params
        public class ItemParams : ItemParamsBase
        {
            public DailyMissionCategoryMasterObject missionEventData;
            public List<MissionProgressPairData> missionProgressPairDataList;
            public Action<ItemParams> onClickEventButton;

            public DateTime endAt;
            public DateTime receiveEndAt;
            public ItemParams(DailyMissionCategoryMasterObject missionEventData, List<MissionProgressPairData> missionProgressPairDataList, Action<ItemParams> onClickEventButton)
            {
                this.missionEventData = missionEventData;
                this.missionProgressPairDataList = missionProgressPairDataList;
                this.onClickEventButton = onClickEventButton;

                endAt = missionEventData.endAt.TryConvertToDateTime();
                receiveEndAt = missionEventData.receiveEndAt.TryConvertToDateTime();
            }
        }

        #endregion

        #region SerializeField
        [SerializeField] private TMPro.TMP_Text titleText;
        [SerializeField] private TMPro.TMP_Text countText;
        [SerializeField] private TMPro.TMP_Text remainTimeText;
        [SerializeField] private GameObject remainTimeBalloonParent, closedGameObject, completedMaskGameObject;
        [SerializeField] private UIProgress progress;
        [SerializeField] private Image eventLogo;
        [SerializeField] private UIBadgeNotification badgeNotification;
        #endregion

        #region PrivateFields
        private ItemParams itemParams;
        private CancellationTokenSource source;
        private const string ExpireDateTimeFormat = "あと<size=34>{0}</size>";
        private const string ReceiveExpireDateTimeFormat = "<size=34>受け取り期間中</size>　あと<size=34>{0}</size>";
        #endregion

        #region OverrideMethods
        public override void Init(ItemParamsBase itemParams)
        {
            this.itemParams = (ItemParams)itemParams;
            UpdateDisplay(this.itemParams);
            base.Init(itemParams);
        }
        #endregion

        #region PrivateMethods
        private void UpdateDisplay(ItemParams itemParams)
        {
            titleText.text = itemParams.missionEventData.name;

            var maxValue = itemParams.missionProgressPairDataList.Count;
            var currentValue = itemParams.missionProgressPairDataList
                .Count(aData => aData.nullableMissionProgressData is { progressStatus: (int)MissionProgressStatus.End});
            var currentReceivingRewardCount = itemParams.missionProgressPairDataList
                .Count(aData => aData.nullableMissionProgressData is { progressStatus: (int)MissionProgressStatus.ReceivingReward});
            progress.SetProgress(min: 0, max: maxValue, value: currentValue);
            countText.text = $"{currentValue}/{maxValue}";
            badgeNotification.SetActive(currentReceivingRewardCount > 0);
            closedGameObject.SetActive(itemParams.endAt.IsPast(AppTime.Now));
            completedMaskGameObject.SetActive(itemParams.missionEventData.displayType == (long)MissionCategoryDisplayType.HideCompleted && currentValue == maxValue);
            SetRemainTimeText();

            if (source != null)
            {
                source.Cancel();
                source.Dispose();
                source = null;
            }
            source = new CancellationTokenSource();
            eventLogo.gameObject.SetActive(false);
            //symbolName定義あり場合は画像はsymbolNameを使う
            string imagePath = (itemParams.missionEventData.symbolName.StartsWith("event_logo_"))
                ? $"Images/MissionEventLogo/{itemParams.missionEventData.symbolName}.png" 
                : $"Images/MissionEventLogo/event_logo_{itemParams.missionEventData.id}.png";
            PageResourceLoadUtility.LoadAssetAsync<Sprite>(imagePath,
                callback: sprite =>
                {
                    eventLogo.sprite = sprite;
                    eventLogo.SetNativeSize();
                    eventLogo.gameObject.SetActive(true);
                }, token: source.Token).Forget();
        }

        private void SetRemainTimeText()
        {
            var now = AppTime.Now;
            var remainString = 
                itemParams.endAt.IsFuture(now) && (itemParams.endAt - now).Days < 100 ? string.Format(ExpireDateTimeFormat, itemParams.endAt.GetRemainingString(now)) : 
                itemParams.receiveEndAt.IsFuture(now) && (itemParams.receiveEndAt - now).Days < 100 ? string.Format(ReceiveExpireDateTimeFormat, itemParams.receiveEndAt.GetRemainingString(now)) :
                string.Empty;
            if (string.IsNullOrEmpty(remainString)) remainTimeBalloonParent.SetActive(false);
            else {
                remainTimeBalloonParent.SetActive(true);
                remainTimeText.text = remainString;
            }
        }
        #endregion

        #region EventListeners
        public void OnClickEventButton()
        {
            itemParams?.onClickEventButton?.Invoke(itemParams);
        }
        #endregion
    }
}