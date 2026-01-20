using System;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Page;
using CruFramework.UI;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

namespace Pjfb.Deck
{
    public class TeamRankRewardListWindow : ModalWindow
    {
        [SerializeField] private ScrollGrid rankRewardScroll;
        #region Params

        public class WindowParams
        {
            public Action onClosed;
        }

        #endregion
        private WindowParams _windowParams;
        
        
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TeamRankRewardList, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            return base.OnPreOpen(args, token);
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            Init();
        }

        #region PrivateMethods
        private void Init()
        {
            rankRewardScroll.SetItems(new List<int>{1,2,3,4,5});
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
