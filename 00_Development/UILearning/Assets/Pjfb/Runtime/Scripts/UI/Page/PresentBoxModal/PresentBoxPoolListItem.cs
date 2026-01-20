using System;
using System.Collections.Generic;
using System.Linq;
using Pjfb.UI;
using UnityEngine;

namespace Pjfb.PresentBox
{
    public class PresentBoxPoolListItem : PoolListItemBase
    {
        #region Params
        public class ItemParams : ItemParamsBase
        {
            public long id;
            public readonly List<PrizeJsonUtility.PrizeContainerData> prizeContainerDataList;
            public readonly string description;
            public readonly bool showReceiveButton;
            public readonly string dateText;
            public readonly Action<ItemParams> onClickReceiveButton;

            public ItemParams(long id, List<PrizeJsonUtility.PrizeContainerData> prizeContainerDataList, string description, bool showReceiveButton, string dateText, Action<ItemParams> onClickReceiveButton)
            {
                this.id = id;
                this.prizeContainerDataList = prizeContainerDataList;
                this.description = description;
                this.showReceiveButton = showReceiveButton;
                this.dateText = dateText;
                this.onClickReceiveButton = onClickReceiveButton;
            }
        }
        #endregion

        #region SerializeField
        [SerializeField] private TMPro.TMP_Text titleText;
        [SerializeField] private TMPro.TMP_Text descriptionText;
        [SerializeField] private TMPro.TMP_Text remainTimeText;
        [SerializeField] private PrizeJsonView prizeJsonView;
        [SerializeField] private GameObject receiveButtonGameObject;
        #endregion

        #region PrivateFields
        private ItemParams itemParams;
        #endregion

        #region OverrideMethods
        public override void Init(ItemParamsBase itemParamsBase)
        {
            itemParams = (ItemParams)itemParamsBase;
            UpdateDisplay(itemParams);
            base.Init(itemParamsBase);
        }
        #endregion

        #region PrivateMethods
        private void UpdateDisplay(ItemParams itemParams)
        {
            receiveButtonGameObject.SetActive(itemParams.showReceiveButton);
            descriptionText.rectTransform.sizeDelta = new Vector2(itemParams.showReceiveButton ? 510 : 710, descriptionText.rectTransform.sizeDelta.y);
            
            descriptionText.text = itemParams.description;

            if (!itemParams.prizeContainerDataList.Any())
            {
                titleText.text = string.Empty;
                prizeJsonView.gameObject.SetActive(false);
            }
            else
            {
                var prizeContainerData = itemParams.prizeContainerDataList[0];
                titleText.text = prizeContainerData.name;
                prizeJsonView.SetView(prizeContainerData.prizeJsonWrap);
                prizeJsonView.gameObject.SetActive(true);
            }

            remainTimeText.text = itemParams.dateText;
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
