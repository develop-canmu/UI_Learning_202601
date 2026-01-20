using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb.Gacha
{
    public class GachaResultPageView : MonoBehaviour
    {
        [SerializeField]
        private GameObject content = null;
        public GameObject Content { get { return content; } }
        
        [SerializeField]
        private UIButton backButton = null;
        public UIButton BackButton { get { return backButton; } }
        
        [SerializeField]
        private UIButton onceButton = null;
        public UIButton OnceButton { get { return onceButton; } }
    }
}
