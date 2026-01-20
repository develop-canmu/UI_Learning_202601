using UnityEngine;
using CruFramework.UI;
using Pjfb.UI;

namespace Pjfb
{
    public class DeckPanelScrollGridItem : ScrollGridItem
    {
        #region Parameters
        public class Parameters
        {
            public DeckPanelView.ViewParams viewParams;
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private DeckPanelView deckPanelView;
        #endregion

        #region PrivateFields
        private Parameters parameters;
        #endregion
        
        #region OverrideMethods
        protected override void OnSetView(object value)
        {
            parameters = (Parameters) value;
            deckPanelView.Init();
            deckPanelView.SetDisplay(parameters.viewParams);
        }
        #endregion
    }
}