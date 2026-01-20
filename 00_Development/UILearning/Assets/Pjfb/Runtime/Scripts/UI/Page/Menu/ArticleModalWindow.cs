using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Pjfb.Menu
{
    public class ArticleModalWindow : ModalWindow
    {
        #region Params
        public class WindowParams
        {
            public string titleText = "";
            public string bodyText = "";
            public Action onClosed;
        }

        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI bodyText;
        private WindowParams _windowParams;
        
        #endregion

        #region Life Cycle
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Article, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args ?? new WindowParams();
            titleText.text = _windowParams.titleText;
            bodyText.text = _windowParams.bodyText;
            return base.OnPreOpen(args, token);
        }
        #endregion

        #region EventListeners
        public void OnClickClose()
        {
            Close(onCompleted: _windowParams.onClosed);
        }
        #endregion
    }
}