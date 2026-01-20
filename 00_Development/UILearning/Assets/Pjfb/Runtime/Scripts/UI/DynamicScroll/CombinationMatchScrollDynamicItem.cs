using System;
using System.Collections.Generic;
using CruFramework.UI;
using Pjfb.Combination;
using Pjfb.Master;
using UnityEngine;

namespace Pjfb
{ 
    public class CombinationMatchScrollData
    {
        public CombinationMatchScrollData(CombinationManager.CombinationMatch combinationMatch)
        {
            CombinationMatch = combinationMatch;
        }
        
        public CombinationManager.CombinationMatch CombinationMatch;
        
    }
    
    public class CombinationMatchScrollDynamicItem : ScrollDynamicItem
    {
        [SerializeField] private CombinationMatchView combinationMatchView;
        
        private CombinationMatchScrollData scrollData;


        protected override void OnSetView(object value)
        {
            scrollData = (CombinationMatchScrollData)value;
            
            combinationMatchView.Initialize(scrollData.CombinationMatch);
            Canvas.ForceUpdateCanvases();
            RecalculateSize();
        }
        
    }
}
