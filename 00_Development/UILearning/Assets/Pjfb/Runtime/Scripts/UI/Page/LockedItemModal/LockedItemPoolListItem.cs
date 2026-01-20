using System;
using System.Collections.Generic;
using System.Linq;
using Pjfb.Extensions;
using Pjfb.UI;
using UnityEngine;

namespace Pjfb.LockedItem
{
    public class LockedItemPoolListItem : PoolListItemBase
    {
        #region Params
        public class ItemParams : ItemParamsBase
        {
            public long id;
            public readonly List<PrizeJsonUtility.PrizeContainerData> prizeContainerDataList;
            public readonly string description;
            public readonly DateTime startDateTime;
            public readonly DateTime endDateTime;
            public readonly bool isReceivable;
            public readonly Action<ItemParams> onClickReceiveButton;

            public ShownReleaseAnimationItemDataContainer ShownReleaseAnimationItemDataContainer;
            public void UpdateShownReleaseAnimationItemData(ShownReleaseAnimationItemDataContainer shownReleaseAnimationItemDataContainer) => this.ShownReleaseAnimationItemDataContainer = shownReleaseAnimationItemDataContainer;
            
            /// <summary>
            /// 1秒度に更新されます
            /// </summary>
            public Status realtimeStatus;
            public void UpdateStatus(DateTime now) {
                realtimeStatus = 
                    endDateTime.IsPast(now) ? Status.Expired :
                    startDateTime.IsFuture(now) ? Status.Locked :
                    Status.Unlocked;
            }
            
            public ItemParams(long id, List<PrizeJsonUtility.PrizeContainerData> prizeContainerDataList, string description, DateTime startDateTime, DateTime endDateTime, bool isReceivable, Action<ItemParams> onClickReceiveButton)
            {
                this.id = id;
                this.prizeContainerDataList = prizeContainerDataList;
                this.description = description;
                this.startDateTime = startDateTime;
                this.endDateTime = endDateTime;
                this.isReceivable = isReceivable;
                this.onClickReceiveButton = onClickReceiveButton;
            }
        }
        #endregion

        #region SerializeField
        [SerializeField] private TMPro.TMP_Text titleText;
        [SerializeField] private TMPro.TMP_Text descriptionText;
        [SerializeField] private TMPro.TMP_Text remainTimeText;
        [SerializeField] private TMPro.TMP_Text unlockTimeText;
        [SerializeField] private GameObject unlockTimeParent;
        [SerializeField] private PrizeJsonView prizeJsonView;
        [SerializeField] private GameObject prizeLockIcon;
        [SerializeField] private GameObject receiveButtonGameObject;
        [SerializeField] private GameObject expiredMaskGameObject;
        [SerializeField] private AnimatorController animatorController;
        #endregion

        #region PrivateFields
        private ItemParams itemParams { get; set; }
        private PrizeJsonUtility.PrizeContainerData showingPrizeData = null;
        private string ReleaseRemainingTextFormat => StringValueAssetLoader.Instance["locked_item.item.release_remain_format"];
        private string ExpireRemainingTextFormat => StringValueAssetLoader.Instance["locked_item.item.expire_remain_format"];
        #endregion

        #region enum
        public enum Status
        {
            Locked,
            Unlocked,
            Expired
        }
        
        #endregion

        #region OverrideMethods
        public override void Init(ItemParamsBase itemParamsBase)
        {
            itemParams = (ItemParams)itemParamsBase;
            animatorController.Init();
            InitDisplay();
            base.Init(itemParamsBase);
        }
        
        public override void Pooled()
        {
            base.Pooled();
            itemParams = null;
        }
        #endregion

        #region PrivateMethods
        private void InitDisplay()
        {
            if (!itemParams.prizeContainerDataList.Any()) {
                showingPrizeData = null;
                titleText.text = string.Empty;
                prizeJsonView.gameObject.SetActive(false);
            } else {
                showingPrizeData = itemParams.prizeContainerDataList[0];
                titleText.text = showingPrizeData.name;
                prizeJsonView.SetView(showingPrizeData.prizeJsonWrap);
                prizeJsonView.gameObject.SetActive(true);
            }
            
            UpdateDisplay(AppTime.Now, itemParams.ShownReleaseAnimationItemDataContainer);
        }
        #endregion
        
        #region PublicMethods
        public void UpdateDisplay(DateTime now, ShownReleaseAnimationItemDataContainer shownReleaseAnimationItemDataContainer)
        {
            if (itemParams == null) return;
            
            var status = itemParams.realtimeStatus;
            unlockTimeText.text = status == Status.Locked ? itemParams.startDateTime.GetPreciseRemainingString(now, textFormat: ReleaseRemainingTextFormat) : string.Empty;
            remainTimeText.text = status == Status.Unlocked ? itemParams.endDateTime.GetPreciseRemainingString(now, textFormat: ExpireRemainingTextFormat) : string.Empty;
            descriptionText.text = itemParams.description;
            if (showingPrizeData == null) titleText.text = string.Empty;
            else {
                var title = showingPrizeData.name;
                if (status == Status.Locked) title = $"{PrizeJsonUtility.LockedText}{title}";
                titleText.text = title;
            }
            
            if (animatorController.isAnimating) return;
            
            receiveButtonGameObject.SetActive(status == Status.Unlocked);
            unlockTimeParent.SetActive(status == Status.Locked);
            prizeLockIcon.SetActive(status == Status.Locked);
            expiredMaskGameObject.SetActive(status == Status.Expired);

            if (status == Status.Unlocked && (shownReleaseAnimationItemDataContainer?.ShouldShowReleaseAnimation(itemParams.id) ?? false)) {
                receiveButtonGameObject.SetActive(false);
                PlayReleaseAnimation(itemParams.id, shownReleaseAnimationItemDataContainer);
            }
        }

        private async void PlayReleaseAnimation(long id, ShownReleaseAnimationItemDataContainer shownReleaseAnimationItemDataContainer)
        {
            await animatorController.Play("Release").SuppressCancellationThrow();
            shownReleaseAnimationItemDataContainer.OnFinishReleaseAnimation(id);
        }
        #endregion

        #region EventListeners
        public void OnClickReceiveButton()
        {
            itemParams?.onClickReceiveButton?.Invoke(itemParams);
        }
        #endregion
    }
}
