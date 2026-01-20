using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Networking.App.Request;
using TMPro;

namespace Pjfb.Training
{
    public class AutoTrainingSummaryStatusItemView : MonoBehaviour
    {

        [SerializeField]
        private TMP_Text nameText = null;
        [SerializeField]
        private TMP_Text valueText = null;
  

        /// <summary>名前の表示</summary>
        public void SetName(string name)
        {
            nameText.text = name;
        }
        
        /// <summary>直の表示</summary>
        public void SetValue(long value)
        {
            valueText.text = value.ToString();
        }
    }
}