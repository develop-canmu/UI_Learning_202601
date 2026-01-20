using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Menu
{
    public class HelpScrollDynamaticSelector : ScrollDynamicItemSelector
    {
        [SerializeField] private ScrollDynamicItem item;
        
        public override ScrollDynamicItem GetItem(object _)
        {
            return item;
        }
    }
}