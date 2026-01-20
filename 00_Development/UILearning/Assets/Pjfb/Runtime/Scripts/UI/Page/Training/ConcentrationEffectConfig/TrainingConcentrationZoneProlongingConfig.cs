using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Training
{
    [CreateAssetMenu(fileName = "ConcentrationProlongingConfig", menuName = "Pjfb/ConcentrationConfig/ConcentrationProlongingConfig")]
    public class TrainingConcentrationZoneProlongingConfig : ScriptableObject
    {
        
        [SerializeField]
        private Sprite gradationBase = null;
        /// <summary>GradationBase</summary>
        public Sprite GradationBase{get{return gradationBase;}}
        
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