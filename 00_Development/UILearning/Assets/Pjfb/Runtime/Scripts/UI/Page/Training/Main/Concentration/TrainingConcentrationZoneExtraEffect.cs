using System.Collections.Generic;
using System.Threading;
using Coffee.UIEffects;
using CruFramework;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Training
{
    public class TrainingConcentrationZoneExtraEffect : TrainingConcentrationEffectBase<TrainingConcentrationZoneProlongingConfig>
    {
        
        [SerializeField]
        private Image gradationBaseImage = null;
        
        [SerializeField]
        private Image borderTop1Image = null;
        [SerializeField]
        private Image borderTop2Image = null;
        
        [SerializeField]
        private Image borderBottom1Image = null;
        [SerializeField]
        private Image borderBottom2Image = null;
        
        
        /// <summary>アニメーションから設定するようのパラメータ</summary>
        [SerializeField][HideInInspector]
        private Color borderTop1Color = Color.white;
        
        /// <summary>アニメーションから設定するようのパラメータ</summary>
        [SerializeField][HideInInspector]
        private Color borderTop2Color = Color.white;
        
        /// <summary>アニメーションから設定するようのパラメータ</summary>
        [SerializeField][HideInInspector]
        private Color borderBottom1Color = Color.white;
        
        /// <summary>アニメーションから設定するようのパラメータ</summary>
        [SerializeField][HideInInspector]
        private Color borderBottom2Color = Color.white;
        
        /// <summary>アニメーションから設定するようのパラメータ</summary>
        [SerializeField][HideInInspector]
        private float configColor = 0;
        
        protected override string ConfigResourceKey{get{return "TrainingConcentrationZoneProlonging";}}

        protected override void SetConfig(TrainingConcentrationZoneProlongingConfig config)
        {
            // Border
            borderTop1Image.color = config.BorderTop1;
            borderTop2Image.color = config.BorderTop2;
            borderBottom1Image.color = config.BorderBottom1;
            borderBottom2Image.color = config.BorderBottom2;
            
            // Gradation
            gradationBaseImage.sprite = config.GradationBase;
        }
        
        private void LateUpdate()
        {
            if(LoadedConfig != null)
            {
                float invColor = 1.0f - configColor;
                // Border
                borderTop1Image.color    = (borderTop1Color * invColor) + (LoadedConfig.BorderTop1 * configColor);
                borderTop2Image.color    = (borderTop2Color * invColor) + (LoadedConfig.BorderTop2 * configColor);
                borderBottom1Image.color = (borderBottom1Color * invColor) + (LoadedConfig.BorderBottom1 * configColor);
                borderBottom2Image.color = (borderBottom2Color * invColor) + (LoadedConfig.BorderBottom2 * configColor);
            }
        }
    }
}
