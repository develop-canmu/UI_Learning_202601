using UnityEngine;

namespace Pjfb.Training
{
    /// <summary> Flowゾーン突入演出 </summary>
    public class TrainingFlowZoneEnterEffect : TrainingConcentrationZoneEnterBaseEffect<TrainingFlowZoneEnterConfig>
    {
        protected override string ConfigResourceKey => "TrainingFlowZoneEnterEffectConfig";
        public override TrainingConcentrationEffectType EffectType => TrainingConcentrationEffectType.Flow;
    }
}