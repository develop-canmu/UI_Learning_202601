using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public class TrainingDeckEnhanceScrollDynamicSelector : ScrollDynamicItemSelector
    {
        [SerializeField] private ScrollDynamicItem item;
        
        public override ScrollDynamicItem GetItem(object item)
        {
            return this.item;
        }
    }
}