using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Encyclopedia
{
    public class EncyclopediaScrollDynamicItemSelector : ScrollDynamicItemSelector
    {
        [SerializeField] 
        private VoiceScrollItem item;

        public override ScrollDynamicItem GetItem(object _)
        {
            return item;
        }
    }
}