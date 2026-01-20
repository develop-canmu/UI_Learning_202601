using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Training
{
    [CreateAssetMenu(fileName = "ConcentrationUpgradeConfig", menuName = "Pjfb/ConcentrationConfig/ConcentrationUpgradeConfig")]
    public class TrainingConcentrationZoneUpgradeConfig : ScriptableObject
    {
        
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
        public class ArrowConfig
        {
            [SerializeField]
            private GradientConfig gradation = new GradientConfig();
            /// <summary>Gradation</summary>
            public GradientConfig Gradation{get{return gradation;}}
        }
        
        [SerializeField]
        private Sprite gradationBase = null;
        /// <summary>GradationBase</summary>
        public Sprite GradationBase{get{return gradationBase;}}
        
        [SerializeField]
        private ArrowConfig arrowL = new ArrowConfig();
        /// <summary>ArrowL</summary>
        public ArrowConfig ArrowL{get{return arrowL;}}
        
        [SerializeField]
        private ArrowConfig arrowR = new ArrowConfig();
        /// <summary>ArrowL</summary>
        public ArrowConfig ArrowR{get{return arrowR;}}
        
        [SerializeField]
        private Color growColor = Color.white;
        /// <summary>Grow</summary>
        public Color GrowColor{get{return growColor;}}
        
        [SerializeField]
        private Color circleColor = Color.white;
        /// <summary>Circle</summary>
        public Color CircleColor{get{return circleColor;}}
        
        [SerializeField]
        private Color arrowColor = Color.white;
        /// <summary>Arrow</summary>
        public Color ArrowColor{get{return arrowColor;}}
        
        [SerializeField]
        private Color borderTop1 = Color.white;
        /// <summary>BorderTop1</summary>
        public Color BorderTop1{get{return borderTop1;}}
        
        [SerializeField]
        private Color borderTop2 = Color.white;
        /// <summary>BorderTop2</summary>
        public Color BorderTop2{get{return borderTop2;}}
        
        [SerializeField]
        private Color borderBottom1 = Color.white;
        /// <summary>BorderBottom1</summary>
        public Color BorderBottom1{get{return borderBottom1;}}
        
        [SerializeField]
        private Color borderBottom2 = Color.white;
        /// <summary>BorderBottom2</summary>
        public Color BorderBottom2{get{return borderBottom2;}}
    }
}