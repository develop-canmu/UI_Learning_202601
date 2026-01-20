using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Training
{
    [CreateAssetMenu(fileName = "ConcentrationLabelConfig", menuName = "Pjfb/ConcentrationConfig/ConcentrationLabelConfig")]
    public class TrainingConcentrationLabelEffectConfig : ScriptableObject
    {
        [SerializeField]
        private Sprite baseGrade = null;
        /// <summary>BaseGrade</summary>
        public Sprite BaseGrade{get{return baseGrade;}}
    }
}