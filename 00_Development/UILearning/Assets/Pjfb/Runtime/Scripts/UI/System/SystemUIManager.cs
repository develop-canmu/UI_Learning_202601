using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb
{
    public class SystemUIManager : MonoBehaviour
    {
        [SerializeField]
        private UILoading loading = null;
        /// <summary>ローディング</summary>
        public UILoading Loading { get { return  loading; } }
        
        [SerializeField]
        private UILoadingProgress progress = null;
        /// <summary>プログレスバー</summary>
        public UILoadingProgress Progress { get { return progress; } }
        
        [SerializeField]
        private UITouchGuard touchGuard = null;
        /// <summary>タッチガード</summary>
        public UITouchGuard TouchGuard { get { return touchGuard; } }

        [SerializeField]
        private UITouchGuard addressableTouchGuard = null;
        /// <summary>タッチガード</summary>
        public UITouchGuard AddressableTouchGuard { get { return addressableTouchGuard; } }
        
        [SerializeField] private UINotification uiNotification = null;
        //// <summary> 通知表示 </summary>
        public UINotification UINotification {get {return uiNotification;} }
    }
}
