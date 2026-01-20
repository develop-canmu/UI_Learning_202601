using System;
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
    /// <summary> 突入演出のベースクラス </summary>
    public abstract class TrainingConcentrationZoneEnterBaseEffect<TConfig> : TrainingConcentrationEffectBase<TConfig> where TConfig : TrainingConcentrationZoneEnteringConfig
    {
        [SerializeField]
        protected Image glowImage = null;
        [SerializeField]
        protected Image gradationBaseImage = null;
        [SerializeField]
        protected Image borderTop1Image = null;
        [SerializeField]
        protected Image borderTop2Image = null;
        [SerializeField]
        protected Image borderBottom1Image = null;
        [SerializeField]
        protected Image borderBottom2Image = null;
        
        /// <summary>アニメーションから設定するようのパラメータ</summary>
        [SerializeField][HideInInspector]
        protected Color borderTop1Color = Color.white;
        
        /// <summary>アニメーションから設定するようのパラメータ</summary>
        [SerializeField][HideInInspector]
        protected Color borderTop2Color = Color.white;
        
        /// <summary>アニメーションから設定するようのパラメータ</summary>
        [SerializeField][HideInInspector]
        protected Color borderBottom1Color = Color.white;
        
        /// <summary>アニメーションから設定するようのパラメータ</summary>
        [SerializeField][HideInInspector]
        protected Color borderBottom2Color = Color.white;
        
        /// <summary>アニメーションから設定するようのパラメータ</summary>
        [SerializeField][HideInInspector]
        protected float configColor = 0;

        protected override void SetConfig(TConfig config)
        {
            // GlowImageColor
            glowImage.color = config.GlowImageColor;
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
    
    public class TrainingConcentrationZoneEnterEffect : TrainingConcentrationZoneEnterBaseEffect<TrainingConcentrationZoneEnteringConfig>
    {
        protected override string ConfigResourceKey{get{return "TrainingConcentrationZoneEntering";}}
    }
}
