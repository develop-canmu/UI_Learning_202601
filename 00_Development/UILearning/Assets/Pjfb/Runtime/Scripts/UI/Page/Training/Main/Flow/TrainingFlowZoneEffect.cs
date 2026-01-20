namespace Pjfb.Training
{
    public class TrainingFlowZoneEffect : TrainingConcentrationZoneBaseEffect<TrainingFlowZoneEffectConfig>
    {
        protected override string ConfigResourceKey => "FlowZoneConfig";

        public override TrainingConcentrationEffectType EffectType => TrainingConcentrationEffectType.Flow;
    }
}