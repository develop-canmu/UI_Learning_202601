using UnityEngine;

namespace Pjfb.Training
{
    /// <summary> FlowZone練習エフェクト </summary>
    public class TrainingFlowZonePracticeEffect : TrainingConcentrationEffectBase<TrainingFlowZonePracticeEffectConfig>
    {
        // 対象パーティクル
        [SerializeField]
        private ParticleSystem[] fxParticles = null;
        
        public override TrainingConcentrationEffectType EffectType => TrainingConcentrationEffectType.Flow;
        
        protected override string ConfigResourceKey {get{return "TrainingFlowZonePracticeEffectConfig";}}
        
        protected override void SetConfig(TrainingFlowZonePracticeEffectConfig config)
        {
            for(int i = 0; i < fxParticles.Length; i++)
            {
                // パーティクルの色指定
                ParticleSystem.MainModule main = fxParticles[i].main;
                main.startColor = config.SmokeColors[i];
            }
        }
    }
}