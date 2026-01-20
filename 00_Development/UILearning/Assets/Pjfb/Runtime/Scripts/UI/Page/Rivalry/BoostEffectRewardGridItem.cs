using System;
using System.Collections.Generic;
using System.Linq;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.UI;
using UnityEngine;

namespace Pjfb
{
    public class BoostEffectRewardGridItem : ScrollGridItem
    {
        #region Params
        public class ItemParams
        {
            public PrizeJsonViewData prizeJsonViewData;
        }
        #endregion

        #region SerializeField
        [SerializeField] private TMPro.TMP_Text titleText;
        [SerializeField] private TMPro.TMP_Text descriptionText;
        [SerializeField] private PrizeJsonView prizeJsonView;
        #endregion

        #region PrivateFields
        private ItemParams itemParams;
        #endregion

        #region OverrideMethods
        protected override void OnSetView(object value)
        {
            itemParams = (ItemParams)value;
            UpdateDisplay(itemParams);
        }
        #endregion

        #region PrivateMethods
        private void UpdateDisplay(ItemParams itemParams)
        {   
            PointDescriptionMasterObject description = MasterManager.Instance.pointDescriptionMaster.FindByMPointId(itemParams.prizeJsonViewData.Id);
            descriptionText.text = description?.description;
            titleText.text = itemParams.prizeJsonViewData.Name;
            prizeJsonView.SetView(itemParams.prizeJsonViewData);
            prizeJsonView.gameObject.SetActive(true);
        }
        #endregion
    }
}
