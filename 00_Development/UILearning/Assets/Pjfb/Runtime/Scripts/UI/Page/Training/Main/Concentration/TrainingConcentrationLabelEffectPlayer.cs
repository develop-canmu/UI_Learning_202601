using System;
using System.Threading;
using CruFramework;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pjfb.Training
{
    /// <summary> コンセントレーションのラベル演出Player </summary>
    public class TrainingConcentrationLabelEffectPlayer : TrainingConcentrationEffectBasePlayer
    {
        /// <summary> コンセントレーションのラベル演出 </summary>
        private static readonly string ConcentrationLabelKey = "ConcentrationLabelEffect";
        /// <summary> Flowのラベル演出 </summary>
        private static readonly string FlowLabelKey = "FlowLabelEffect";

        /// <summary> 演出アセット読み込みキーを取得 </summary>
        protected override string GetEffectPathKey(TrainingConcentrationEffectType effectType)
        {
            string path = string.Empty;
            
            switch (effectType)
            {
                case TrainingConcentrationEffectType.Concentration:
                {
                    path = ConcentrationLabelKey;
                    break;
                }
                case TrainingConcentrationEffectType.Flow:
                {
                    path = FlowLabelKey;
                    break;
                }
                default:
                {
                    CruFramework.Logger.LogError($"Not Find TrainingConcentrationEffectType : {effectType}");
                    break;
                }
            }

            return path;
        }
    }
}