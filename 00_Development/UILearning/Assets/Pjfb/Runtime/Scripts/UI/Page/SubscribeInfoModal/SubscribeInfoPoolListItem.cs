using Pjfb.UI;
using UnityEngine;

namespace Pjfb.SubscribeInfo
{
    public class SubscribeInfoPoolListItem : PoolListItemBase
    {
        #region Params
        public class ItemParams : ItemParamsBase
        {
            public SubscribeInfoDescriptionView.ViewParams nullableDescriptionViewParams;
            public SubscribeInfoExpireDateView.ViewParams nullableExpireDateViewParams;
            
            public ItemParams(SubscribeInfoExpireDateView.ViewParams expireDateViewParams)
            {
                nullableExpireDateViewParams = expireDateViewParams;
                nullableDescriptionViewParams = null;
            }

            public ItemParams(SubscribeInfoDescriptionView.ViewParams descriptionViewParams)
            {
                nullableDescriptionViewParams = descriptionViewParams;
                nullableExpireDateViewParams = null;
            }
        }
        #endregion

        #region SerializeField
        [SerializeField] private SubscribeInfoDescriptionView descriptionView;
        [SerializeField] private SubscribeInfoExpireDateView expireDateView;
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

        public override int GetItemHeight(int prefabHeight, ItemParamsBase itemParamsBase)
        {
            var itemParams = (ItemParams)itemParamsBase;
            var preferredHeight = (float)prefabHeight;

            if (itemParams.nullableExpireDateViewParams != null) preferredHeight = expireDateView.GetPreferredHeight(itemParams.nullableExpireDateViewParams);
            else if (itemParams.nullableDescriptionViewParams != null) preferredHeight = descriptionView.GetPreferredHeight(itemParams.nullableDescriptionViewParams);
            return Mathf.CeilToInt(preferredHeight);
        }
        #endregion

        #region PrivateMethods
        private void UpdateDisplay(ItemParams itemParams)
        {
            descriptionView.SetActive(isActive: false);
            expireDateView.SetActive(isActive: false);
            if (itemParams.nullableDescriptionViewParams != null) descriptionView.SetDisplay(itemParams.nullableDescriptionViewParams);
            else if (itemParams.nullableExpireDateViewParams != null) expireDateView.SetDisplay(itemParams.nullableExpireDateViewParams);
        }
        #endregion
    }
}
