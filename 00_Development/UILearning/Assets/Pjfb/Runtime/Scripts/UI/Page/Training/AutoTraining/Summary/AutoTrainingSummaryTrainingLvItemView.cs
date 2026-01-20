using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using TMPro;

namespace Pjfb.Training
{
    public class AutoTrainingSummaryTrainingLvItemView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text nameText = null;
        [SerializeField]
        private TMP_Text valueText = null;
        
        public void SetName(string name)
        {
            nameText.text = name;
        }
        
        public void SetValue(long value)
        {
            valueText.text = value.ToString();
        }
    }
}