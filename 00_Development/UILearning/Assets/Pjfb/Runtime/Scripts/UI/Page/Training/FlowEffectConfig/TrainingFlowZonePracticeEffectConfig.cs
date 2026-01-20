using UnityEngine;

namespace Pjfb.Training
{
    /// <summary> Flowゾーン中練習エフェクト設定 </summary>
    [CreateAssetMenu(fileName = "FlowZonePracticeConfig", menuName = "Pjfb/FlowConfig/FlowZonePracticeConfig")]
    public class TrainingFlowZonePracticeEffectConfig : ScriptableObject
    {
        [SerializeField]
        private Color[] smokeColors;
        public Color[] SmokeColors => smokeColors;
    }
}