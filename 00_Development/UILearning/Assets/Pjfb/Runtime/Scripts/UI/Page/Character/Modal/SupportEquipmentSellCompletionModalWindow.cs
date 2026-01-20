using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.Page;
using CruFramework.UI;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Shop;
using Pjfb.UserData;
using Pjfb.Common;

namespace Pjfb.Character
{
    public class SupportEquipmentSellCompletionModalWindow : ModalWindow
    {
        [SerializeField] private ScrollGrid scroll;
        
        #region Params

        public class WindowParams
        {
            public List<ItemIconGridItem.Data> uPointScrollDataList;
            public Action onClose;
        }

        #endregion

        private WindowParams _windowParams;
        
        
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.SupportEquipmentSellCompletion, data);
        }
        

        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            InitializeView();
            return base.OnPreOpen(args, token);
        }
        
        #region PrivateMethods
        private void InitializeView()
        {
            scroll.SetItems(_windowParams.uPointScrollDataList);
        }
        #endregion

        #region EventListeners
        public void OnClickClose()
        {
            Close(onCompleted: _windowParams?.onClose);
        }

        #endregion
       
        
        
    }
}
