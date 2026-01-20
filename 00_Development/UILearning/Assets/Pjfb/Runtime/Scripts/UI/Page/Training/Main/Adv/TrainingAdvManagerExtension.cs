using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Adv;
using Pjfb.Training;

namespace Pjfb
{
    public class TrainingAdvManagerExtension : MonoBehaviour
    {
        [SerializeField]
        private TrainingMain trainingMain = null;
        /// <summary></summary>
        public TrainingMain TrainingMainPage{get{return trainingMain;}}
        
    }
}