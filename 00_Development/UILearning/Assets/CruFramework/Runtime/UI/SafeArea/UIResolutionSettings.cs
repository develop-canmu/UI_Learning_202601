using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CruFramework.UI
{
    public class UIResolutionSettings : ScriptableObject
    {
        [SerializeField]
        private Vector2Int referenceResolution = new Vector2Int(1920, 1080);
        /// <summary>基準となるUI解像度</summary>
        public Vector2Int ReferenceResolution { get { return referenceResolution; } }

        [SerializeField]
        private Vector2Int maxResolution = new Vector2Int(0, 0);
        /// <summary>最大解像度</summary>
        public Vector2Int MaxResolution { get { return maxResolution; } }
    }
}
