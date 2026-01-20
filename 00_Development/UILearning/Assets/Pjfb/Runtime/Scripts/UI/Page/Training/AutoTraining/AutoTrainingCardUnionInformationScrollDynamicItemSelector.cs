using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Training
{
    public class AutoTrainingCardUnionInformationScrollDynamicItemSelector : TrainingCardUnionInformationScrollDynamicItemSelector
    {
        
        [SerializeField]
        private AutoTrainingCardUnionStatusResultItem autoTrainingCardUnionStatusResultItem = null;
        
        public override ScrollDynamicItem GetItem(object item)
        {
            if (item is AutoTrainingCardUnionStatusResultItem.Argument)
            {
                return autoTrainingCardUnionStatusResultItem;
            }
            return base.GetItem(item);
        }
    }
}