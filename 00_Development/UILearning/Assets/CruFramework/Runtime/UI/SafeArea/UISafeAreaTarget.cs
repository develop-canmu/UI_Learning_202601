using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.UI
{
    public class UISafeAreaTarget : MonoBehaviour
    {
        [SerializeField]
        private RectTransform safeArea = null;
        /// <summary>セーフエリア</summary>
        public RectTransform SafeArea { get { return safeArea; } }
        
        [SerializeField]
        private RectTransform fixedArea = null;
        /// <summary>解像度を固定するエリア</summary>
        public RectTransform FixedArea { get { return fixedArea; } }
    }
}
