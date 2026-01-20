using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pjfb
{
    public class TitleDetailModalWindow : ModalWindow
    {
        #region Params
        public class WindowParams
        {
            public long Id;
            public Action onClosed;
        }
        
        [SerializeField] private UserTitleImage image;
        private WindowParams _windowParams;
        #endregion
        
        #region Life Cycle
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TitleDetail, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            if(_windowParams!= null) image.SetTexture(_windowParams.Id);
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