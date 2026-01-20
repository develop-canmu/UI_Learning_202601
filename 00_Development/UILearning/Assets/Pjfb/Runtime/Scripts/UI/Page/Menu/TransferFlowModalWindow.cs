using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Pjfb.Menu
{
    public class TransferFlowModalWindow : ModalWindow
    {
        #region Params
        public class WindowParams
        {
            public Action onClosed;
        }
        
        private WindowParams _windowParams;
        
        #endregion

        #region Life Cycle
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TransferFlow, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
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