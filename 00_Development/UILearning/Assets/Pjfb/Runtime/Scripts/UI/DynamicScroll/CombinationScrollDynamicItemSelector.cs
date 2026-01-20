using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public class CombinationScrollDynamicItemSelector : ScrollDynamicItemSelector
    {
        [SerializeField] private ScrollDynamicItem item;
        
        public override ScrollDynamicItem GetItem(object _)
        {
            return item;
        }
    }
}