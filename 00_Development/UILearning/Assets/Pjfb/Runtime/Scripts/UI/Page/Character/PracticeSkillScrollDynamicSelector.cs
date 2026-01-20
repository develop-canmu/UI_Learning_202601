using CruFramework.UI;
using UnityEngine;

namespace Pjfb.Character
{ 
    public class PracticeSkillScrollDynamicSelector : ScrollDynamicItemSelector
    {
        [SerializeField] private ScrollDynamicItem item;
        
        public override ScrollDynamicItem GetItem(object item)
        {
            return this.item;
        }
    }
}