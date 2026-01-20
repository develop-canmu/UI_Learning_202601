using CruFramework.UI;
using Pjfb.Master;
using TMPro;
using UnityEngine;

namespace Pjfb
{
    public class SupportEquipmentFilterPracticeSkillScrollData
    {
        public TrainingStatusTypeDetailMasterObject TrainingStatusTypeDetailMasterObject;
        public bool Toggle;

        public SupportEquipmentFilterPracticeSkillScrollData(TrainingStatusTypeDetailMasterObject trainingStatusTypeDetailMaster, bool toggle)
        {
            TrainingStatusTypeDetailMasterObject = trainingStatusTypeDetailMaster;
            Toggle = toggle;
        }
    }
    
    public class SupportEquipmentFilterPracticeSkillScrollItem: ScrollGridItem
    {
        [SerializeField]
        private SupportEquipmentFilterPracticeSkillSelectionModal modal;
        
        [SerializeField]
        private UIToggle toggle;
        
        [SerializeField]
        private TMP_Text practiceSkillName;

        private SupportEquipmentFilterPracticeSkillScrollData scrollData;

        protected override void OnSetView(object value)
        {
            scrollData = (SupportEquipmentFilterPracticeSkillScrollData)value;
            toggle.SetIsOnWithoutNotify(scrollData.Toggle);
            practiceSkillName.text = scrollData.TrainingStatusTypeDetailMasterObject.name;
        }

        /// <summary>トグルの値が変更された際の処理</summary>
        public void OnValueChanged(bool value)
        {
            if (value && !scrollData.Toggle && !modal.CanChangeToggle())
            {
                toggle.isOn = false;
            }
            else
            {
                scrollData.Toggle = value;
            }
        }
    }
}