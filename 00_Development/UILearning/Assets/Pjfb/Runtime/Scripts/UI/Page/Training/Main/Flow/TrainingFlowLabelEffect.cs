using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Training
{
    /// <summary> Flowラベル </summary>
    public class TrainingFlowLabelEffect : TrainingConcentrationLabelBaseEffect<TrainingFlowLabelEffectConfig>
    {
        protected override string ConfigResourceKey => "FlowLabelConfig";

        public override TrainingConcentrationEffectType EffectType => TrainingConcentrationEffectType.Flow;
    }
}