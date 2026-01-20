using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Training
{
    public class TrainingPracticeSkillDynamicSelector : ScrollDynamicItemSelector
    {
        [SerializeField] 
        private TrainingPracticeSkillDetailView item;

        public override ScrollDynamicItem GetItem(object item)
        {
            return this.item;
        }
    }
}