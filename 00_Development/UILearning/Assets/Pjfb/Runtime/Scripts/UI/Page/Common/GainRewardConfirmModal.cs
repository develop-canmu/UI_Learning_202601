using System;
using System.Threading;
using CruFramework.Page;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using Pjfb.Shop;
using Pjfb.UserData;
using Pjfb.Common;
using Pjfb.Networking.App.Request;

namespace Pjfb.Character
{
    public class GainRewardConfirmModal : ModalWindow
    {
        #region Params

        public class WindowParams
        {
            public WindowParams(string title, string message, long pointId, long beforeCount, long afterCount, Action onClosed)
            {
                Title = title;
                Content = message;
                PointId = pointId;
                BeforeCount = beforeCount;
                AfterCount = afterCount;
                OnClosed = onClosed;
            }

            public readonly string Title;
            public readonly string Content;
            public readonly long PointId;
            public readonly long BeforeCount;
            public readonly long AfterCount;
            public readonly Action OnClosed;
        }

        #endregion
        private WindowParams _windowParams;

        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI contentText;
        [SerializeField] private PossessionItemUi possessionItemUi;
        [SerializeField] private ItemIcon itemIcon;
        
        
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.GainRewardConfirm, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            Init();
            return base.OnPreOpen(args, token);
        }

        #region PrivateMethods
        private void Init()
        {
            titleText.text = _windowParams.Title;
            contentText.text = _windowParams.Content;
            possessionItemUi.SetAfterCount(_windowParams.PointId, _windowParams.BeforeCount, _windowParams.AfterCount );
            itemIcon.SetIconId(_windowParams.PointId);
            itemIcon.SetCount(_windowParams.AfterCount - _windowParams.BeforeCount);
        }
        #endregion

        #region EventListeners

        public void OnClickClose()
        {
            Close(onCompleted: _windowParams.OnClosed);
        }
        #endregion
       
        
        
    }
}
