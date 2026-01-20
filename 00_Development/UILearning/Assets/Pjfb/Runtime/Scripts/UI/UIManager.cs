using System.Collections;
using System.Collections.Generic;
using Pjfb.UI;
using UnityEngine;

namespace Pjfb
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private Camera uiCamera = null;
        /// <summary>カメラ</summary>
        public Camera UICamera { get { return uiCamera; } }
        
        [SerializeField]
        private Canvas rootCanvas = null;
        /// <summary>ルートキャンバス</summary>
        public Canvas RootCanvas { get { return rootCanvas; } }

        [SerializeField]
        private EffectManager effectManager = null;
        public EffectManager EffectManager { get { return effectManager; } }
        
        [SerializeField]
        private PageManager pageManager = null;
        /// <summary>ページ</summary>
        public PageManager PageManager { get { return pageManager; } }

        [SerializeField]
        private ModalManager modalManager = null;
        /// <summary>モーダル</summary>
        public ModalManager ModalManager { get { return modalManager; } }

        [SerializeField]
        private ModalManager errorModalManager = null;
        /// <summary>エラーモーダル</summary>
        public ModalManager ErrorModalManager { get { return errorModalManager; } }
        
        [SerializeField]
        private FadeManager fadeManager = null;
        /// <summary>フェードマネージャー</summary>
        public FadeManager FadeManager { get { return  fadeManager; } }
        
        [SerializeField]
        private UIHeader header = null;
        /// <summary>ヘッダー</summary>
        public UIHeader Header { get { return header; } }
        
        [SerializeField]
        private UIFooter footer = null;
        /// <summary>フッター</summary>
        public UIFooter Footer { get { return  footer; } }

        [SerializeField]
        private SystemUIManager system = null;
        /// <summary>システムUI</summary>
        public SystemUIManager System { get { return system; } }
        
        [SerializeField]
        private Transform splashScreenParent = null;
        /// <summary>スプラッシュスクリーン表示位置</summary>
        public Transform SplashScreenParent { get { return splashScreenParent; } }
        
        [SerializeField]
        private H2MDUIManager h2mdUIManager = null;
        /// <summary>H2MDUIマネージャー</summary>
        public H2MDUIManager H2MDUIManager { get { return h2mdUIManager; } }
    }
}
