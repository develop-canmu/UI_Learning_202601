using UnityEngine;
using CruFramework.UI;
using Pjfb.Networking.App.Request;

namespace Pjfb
{
    public class RewardGridItem : ScrollGridItem
    {
        #region Parameters
        public class Parameters
        {
            public PrizeJsonWrap prizeJsonWrap;
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private PrizeJsonView itemIcon;
        #endregion

        #region PrivateField
        private Parameters _parameter;
        #endregion
        
        protected override void OnSetView(object value)
        {
            _parameter = (Parameters)value;
            itemIcon.SetView(_parameter.prizeJsonWrap);
        }
    }
}