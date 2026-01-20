namespace Pjfb.Training
{
    /// <summary> コンセントレーションゾーン演出Player </summary>
    public class TrainingConcentrationZoneEffectPlayer : TrainingConcentrationEffectBasePlayer
    {
        /// <summary> Cゾーン演出 </summary>
        private static readonly string ConcentrationZoneKey = "ConcentrationZoneEffect";
        /// <summary> Flowゾーン演出 </summary>
        private static readonly string FlowZoneKey = "FlowZoneEffect";
        
        /// <summary> 演出アセット読み込みキーを取得 </summary>
        protected override string GetEffectPathKey(TrainingConcentrationEffectType effectType)
        {
            string path = string.Empty;
            
            switch (effectType)
            {
                case TrainingConcentrationEffectType.Concentration:
                {
                    path = ConcentrationZoneKey;
                    break;
                }
                case TrainingConcentrationEffectType.Flow:
                {
                    path = FlowZoneKey;
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