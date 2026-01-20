using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Training
{
    public class TrainingStatusUpView : MonoBehaviour
    {
        [SerializeField]
        private  TMPro.TMP_Text valueText = null;
        [SerializeField]
        private OmissionTextSetter omissionTextSetter = null;
        
        public void SetValue(BigValue value)
        {
            
            valueText.text = value.ToDisplayString(omissionTextSetter.GetOmissionData());
        }
    }
}