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
    public abstract class TrainingConcentrationZoneBaseEffect<TConfig> :  TrainingConcentrationEffectBase<TConfig>  where TConfig : TrainingConcentrationZoneEffectConfig
    {
         [System.Serializable]
        private class StreamData
        {
            [SerializeField]
            public RawImage image = null;
            [SerializeField]
            public UIGradient gradient = null;
        }
        
        [SerializeField]
        private Image focusImage = null;
        [SerializeField]
        private UIGradient focusGradient = null;
        
        [SerializeField]
        private GameObject additionalEffectRoot = null;
        
        [SerializeField]
        private StreamData streamL1 = null;
        [SerializeField]
        private StreamData streamL2 = null;
        [SerializeField]
        private StreamData streamL3 = null;
        
        [SerializeField]
        private StreamData streamR1 = null;
        [SerializeField]
        private StreamData streamR2 = null;
        [SerializeField]
        private StreamData streamR3 = null;
        
        
        /// <summary>
        /// アニメーションから設定するようのパラメータ
        /// </summary>
        [SerializeField][HideInInspector]
        private Color focusImageColor = Color.white;
        
        // 追加で読み込んだエフェクト
        private List<GameObject> additionalEffects = new List<GameObject>();

        // 読み込み前に追加のエフェクトを削除
        protected override void OnPreLoadConfig()
        {
            // 追加エフェクトを削除
            foreach(GameObject effect in additionalEffects)
            {
                GameObject.Destroy(effect);
            }
            additionalEffects.Clear();
        }

        private void SetColor(StreamData streamData, Texture texture, TrainingConcentrationZoneEffectConfig.StreamConfig config)
        {
            streamData.image.texture = texture;
            streamData.image.material = config.ImageMaterial;
            streamData.image.color = config.ImageColor;
            streamData.gradient.color1 = config.GradientTop;
            streamData.gradient.color2 = config.GradientBottom;
        }
            
        protected override void SetConfig(TConfig config)
        {
            // Focus
            focusImage.color = config.FocusImageColor;
            
            if(focusGradient.direction == UIGradient.Direction.Diagonal)
            {
                focusGradient.color1 = config.FocusGradient.DiagonalColor1;
                focusGradient.color2 = config.FocusGradient.DiagonalColor2;
                focusGradient.color3 = config.FocusGradient.DiagonalColor3;
                focusGradient.color4 = config.FocusGradient.DiagonalColor4;
            }
            
            // StreamL
            SetColor(streamL1, config.StreamTexture, config.StreamL1);
            SetColor(streamL2, config.StreamTexture, config.StreamL2);
            SetColor(streamL3, config.StreamTexture, config.StreamL3);
            // StreamR
            SetColor(streamR1, config.StreamTexture, config.StreamR1);
            SetColor(streamR2, config.StreamTexture, config.StreamR2);
            SetColor(streamR3, config.StreamTexture, config.StreamR3);
            // 追加エフェクト
            foreach(TrainingConcentrationZoneEffectConfig.AdditionalEffectData prefab in config.AdditionalEffect)
            {
                GameObject effect = GameObject.Instantiate<GameObject>(prefab.Prefab, additionalEffectRoot.transform);
                additionalEffects.Add(effect);
            }
        }

        private void LateUpdate()
        {
            if(LoadedConfig != null)
            {
                // Focus
                focusImage.color = focusImageColor * LoadedConfig.FocusImageColor;
            }
        }
    }
    
    public class TrainingConcentrationZoneEffect : TrainingConcentrationZoneBaseEffect<TrainingConcentrationZoneEffectConfig>
    {
        /// <summary>パスのキー</summary>
        protected override string ConfigResourceKey{get{return "ConcentrationZoneConfig";}}
    }
}
