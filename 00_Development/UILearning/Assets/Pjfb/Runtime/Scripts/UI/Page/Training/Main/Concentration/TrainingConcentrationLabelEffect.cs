using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace Pjfb.Training
{
    public abstract class TrainingConcentrationLabelBaseEffect<TConfig> : TrainingConcentrationEffectBase<TConfig> where TConfig : TrainingConcentrationLabelEffectConfig
    {
        [SerializeField]
        protected Image baseGrade = null; 
        
        protected override void SetConfig(TConfig config)
        {
            baseGrade.sprite = config.BaseGrade;
        }
    }
    
    public class TrainingConcentrationLabelEffect : TrainingConcentrationLabelBaseEffect<TrainingConcentrationLabelEffectConfig>
    {
        /// <summary>リソースのキー</summary>
        protected override string ConfigResourceKey{get{return "ConcentrationLabel";}}
        
    }
}