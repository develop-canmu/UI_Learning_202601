using CruFramework.UI;
using UnityEngine;

namespace Pjfb.Community
{
    public class ChatItemSelector : ScrollDynamicItemSelector
    {
        [SerializeField]
        private ScrollDynamicItem item;
        
        public override ScrollDynamicItem GetItem(object item)
        {
            return this.item;
        }
    }
}