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
    public class TrainingConcentrationZoneUpGradeEffect : TrainingConcentrationEffectBase<TrainingConcentrationZoneUpgradeConfig>
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

        [SerializeField]
        private Image glowImage = null;
        [SerializeField]
        private RawImage circleImage = null;
        [SerializeField]
        private Image arrowImage = null;
        [SerializeField]
        private UIGradient arrowLGradation = null;
        [SerializeField]
        private UIGradient arrowRGradation = null;
        
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
        
        protected override string ConfigResourceKey{get{return "TrainingConcentrationZoneUpgrade";}}

        
        
        private void SetArrowConfig(UIGradient arrow, TrainingConcentrationZoneUpgradeConfig.ArrowConfig config)
        {
            if(arrow.direction == UIGradient.Direction.Diagonal)
            {
                arrow.color1 = config.Gradation.DiagonalColor1;
                arrow.color2 = config.Gradation.DiagonalColor2;
                arrow.color3 = config.Gradation.DiagonalColor3;
                arrow.color4 = config.Gradation.DiagonalColor4;
            }
        }
        
        protected override void SetConfig(TrainingConcentrationZoneUpgradeConfig config)
        {
            
            // Glow
            glowImage.color = config.GrowColor;
            // Circle
            circleImage.color = config.CircleColor;
            // Arrow
            arrowImage.color = config.ArrowColor;
            // Border
            borderTop1Image.color = config.BorderTop1;
            borderTop2Image.color = config.BorderTop2;
            borderBottom1Image.color = config.BorderBottom1;
            borderBottom2Image.color = config.BorderBottom2;
            
            // Arrow
            SetArrowConfig(arrowLGradation, config.ArrowL);
            SetArrowConfig(arrowRGradation, config.ArrowR);
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
