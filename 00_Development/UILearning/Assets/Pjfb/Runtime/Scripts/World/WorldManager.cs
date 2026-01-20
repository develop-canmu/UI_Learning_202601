using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb
{
    public class WorldManager : MonoBehaviour
    {
        [SerializeField]
        private Camera worldCamera = null;
        /// <summary>カメラ</summary>
        public Camera WorldCamera { get { return worldCamera; } }
        
        [SerializeField]
        private Camera threeDWorldCamera = null;
        /// <summary>カメラ</summary>
        public Camera ThreeDWorldCamera { get { return threeDWorldCamera; } }
        
        [SerializeField]
        private Transform rootTransform = null;
        /// <summary>ルート</summary>
        public Transform RootTransform { get { return rootTransform; } }
    }
}
