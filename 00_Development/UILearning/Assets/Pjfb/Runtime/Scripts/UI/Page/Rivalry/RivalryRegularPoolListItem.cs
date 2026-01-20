using UnityEngine;
using TMPro;
using System;
using Pjfb.UI;
using Pjfb.Extensions;
using Pjfb.Master;

namespace Pjfb.Rivalry
{
    public class RivalryRegularPoolListItem : PoolListItemBase
    {
        #region InnerClass
        public class ItemParams : ItemParamsBase
        {
            public HuntDifficultyMasterObject huntDifficultyMasterObject;
            public HuntMasterObject huntMasterObject;
            public HuntTimetableMasterObject huntTimetableMasterObject;
            public Action<ItemParams> onClickItemParams;
            
            public ItemParams(HuntDifficultyMasterObject huntDifficultyMasterObject, HuntMasterObject huntMasterObject, HuntTimetableMasterObject huntTimetableMasterObject, Action<ItemParams> onClickItemParams)
            {
                this.huntDifficultyMasterObject = huntDifficultyMasterObject;
                this.huntMasterObject = huntMasterObject;
                this.huntTimetableMasterObject = huntTimetableMasterObject;
                this.onClickItemParams = onClickItemParams;
            }
        }
        #endregion

        #region SerializeFields
        [SerializeField] private TMP_Text powerText, titleText;
        [SerializeField] private OmissionTextSetter omissionTextSetter;
        [SerializeField] private RivalryRegularBannerImage bannerImage;
        #endregion

        private ItemParams _itemParams;

        public override void Init(ItemParamsBase value)
        {
            _itemParams = (ItemParams) value;
            bannerImage.SetTexture(_itemParams.huntDifficultyMasterObject.id);
            titleText.text = _itemParams.huntDifficultyMasterObject.name;
            powerText.text = new BigValue(_itemParams.huntDifficultyMasterObject.combatPowerRecommended).ToDisplayString(omissionTextSetter.GetOmissionData());
        }

        #region EventListener
        public void OnClickListItem()
        {
            _itemParams?.onClickItemParams?.Invoke(_itemParams);
        }
        #endregion
    }
}