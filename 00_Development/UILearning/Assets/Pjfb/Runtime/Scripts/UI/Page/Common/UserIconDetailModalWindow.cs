using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using UnityEngine;

namespace Pjfb
{
    public class UserIconDetailModalWindow : ModalWindow
    {
        #region Params
        public class WindowParams
        {
            public long Id;
            public Action onClosed;
        }
        
        [SerializeField] private UserIcon userIcon;
        [SerializeField] private TMPro.TMP_Text nameText;
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
            if (_windowParams != null)
            {
                userIcon.SetIconId(_windowParams.Id);
                var iconMaster = MasterManager.Instance.iconMaster.FindData(_windowParams.Id);
                nameText.text = iconMaster?.name ?? string.Empty;
            }
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