using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using UnityEngine.UI;

namespace Pjfb.Training
{
    [CreateAssetMenu(fileName = "CZoneConfig", menuName = "Pjfb/ConcentrationConfig/CZoneConfig")]
    public class TrainingConcentrationZoneEffectConfig : ScriptableObject
    {
        [System.Serializable]
        public class StreamConfig
        {
            
            [SerializeField]
            private Material imageMaterial = null;
            /// <summary>マテリアル</summary>
            public Material ImageMaterial{get{return imageMaterial;}}
            
            [SerializeField]
            private Color imageColor = Color.white;
            /// <summary>イメージカラー</summary>
            public Color ImageColor{get{return imageColor;}}
                
            [SerializeField]
            private Color gradientTop = Color.white;
            /// <summary>グラデーション（上）</summary>
            public Color GradientTop{get{return gradientTop;}}
            
            [SerializeField]
            private Color gradientBottom = Color.white;
            /// <summary>グラデーション（下）</summary>
            public Color GradientBottom{get{return gradientBottom;}}
        }
        
        [System.Serializable]
        public class GradientConfig
        {
            [SerializeField]
            private Color diagonalColor1 = Color.white;
            /// <summary>DiagonalColor</summary>
            public Color DiagonalColor1{get{return diagonalColor1;}}
        
            [SerializeField]
            private Color diagonalColor2 = Color.white;
            /// <summary>DiagonalColor</summary>
            public Color DiagonalColor2{get{return diagonalColor2;}}
        
            [SerializeField]
            private Color diagonalColor3 = Color.white;
            /// <summary>DiagonalColor</summary>
            public Color DiagonalColor3{get{return diagonalColor3;}}
        
            [SerializeField]
            private Color diagonalColor4 = Color.white;
            /// <summary>DiagonalColor</summary>
            public Color DiagonalColor4{get{return diagonalColor4;}}
        }
        
        [System.Serializable]
        public class AdditionalEffectData
        {
            [SerializeField]
            private GameObject prefab = null;
            /// <summary> プレハブ</summary>
            public GameObject Prefab{get{return prefab;}}
        }
        
#if UNITY_EDITOR
        [SerializeField][TextArea]
        private string summary = string.Empty;
        /// <summary>エディタ上で確認する用のテキストデータ</summary>
        public string Summary{get{return summary;}}
#endif
        
        [SerializeField]
        private Color focusImageColor = Color.white;
        /// <summary>FxFocus</summary>
        public Color FocusImageColor{get{return focusImageColor;}}
        

        [SerializeField]
        private GradientConfig focusGradient = new GradientConfig();
        /// <summary>Focus</summary>
        public GradientConfig FocusGradient{get{return focusGradient;}}
        
        
        [SerializeField]
        private Texture streamTexture = null;
        /// <summary>StreamImage</summary>
        public Texture StreamTexture{get{return streamTexture;}}
        
        [SerializeField]
        private StreamConfig streamL1 = new StreamConfig();
        /// <summary>FxStreamL1</summary>
        public StreamConfig StreamL1{get{return streamL1;}}
        
        [SerializeField]
        private StreamConfig streamL2 = new StreamConfig();
        /// <summary>FxStreamL2</summary>
        public StreamConfig StreamL2{get{return streamL2;}}
        
        [SerializeField]
        private StreamConfig streamL3 = new StreamConfig();
        /// <summary>FxStreamL3</summary>
        public StreamConfig StreamL3{get{return streamL3;}}
        
        [SerializeField]
        private StreamConfig streamR1 = new StreamConfig();
        /// <summary>FxStreamR1</summary>
        public StreamConfig StreamR1{get{return streamR1;}}
        
        [SerializeField]
        private StreamConfig streamR2 = new StreamConfig();
        /// <summary>FxStreamR2</summary>
        public StreamConfig StreamR2{get{return streamR2;}}
        
        [SerializeField]
        private StreamConfig streamR3 = new StreamConfig();
        /// <summary>FxStreamR3</summary>
        public StreamConfig StreamR3{get{return streamR3;}}
        
        [SerializeField]
        private AdditionalEffectData[] additionalEffects = null;
        /// <summary>追加エフェクト</summary>
        public AdditionalEffectData[] AdditionalEffect{get{return additionalEffects;}}

    }
}