using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using CruFramework.UI;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.Training;
using Pjfb.UserData;
using TMPro;

namespace Pjfb
{
    
    public class TrainingEventScrollItem : ScrollGridItem
    {

        [SerializeField] private TextMeshProUGUI tempEventName;


        private TrainingEventMasterObject trainingEvent;
        private int eventId;
        protected override void OnSetView(object value)
        {
            trainingEvent = MasterManager.Instance.trainingEventMaster.FindData((long)value);
            tempEventName.text = trainingEvent.name;
        }
        

        public void OnClickSkill()
        {
            CharacterSkillModal.Open(new CharacterSkillModal.WindowParams
            {
                onClosed = null,
            });
        }
    }
}