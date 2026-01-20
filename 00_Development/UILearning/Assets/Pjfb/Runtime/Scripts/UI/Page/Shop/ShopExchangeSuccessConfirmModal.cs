using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Master;

namespace Pjfb.Shop
{
    public class ShopExchangeSuccessConfirmModal : ModalWindow
    {
        #region InnerClass
        public class Parameters
        {
            public List<PrizeJsonWrap> prizeJsonWraps;
            public Action onCloseWindow;

            public Parameters(List<PrizeJsonWrap> prizeJsonWraps, Action onCloseWindow)
            {
                this.prizeJsonWraps = prizeJsonWraps;
                this.onCloseWindow = onCloseWindow;
            }
        }
        #endregion

        #region SerializeFields
        [SerializeField] private ScrollGrid scrollGrid;
        #endregion
        
        #region PrivateFields
        private Parameters _parameters;
        #endregion
        
        #region PublicStaticMethods
        public static void Open(Parameters parameters)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ShopExchangeSuccessConfirm, parameters);
        }

        public static void TryOpen(Parameters parameters)
        {
            if (parameters.prizeJsonWraps is {Count: > 0}) Open(parameters);
            else parameters.onCloseWindow?.Invoke();
        }
        #endregion

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _parameters = (Parameters) args;
            _parameters.prizeJsonWraps ??= new List<PrizeJsonWrap>();
            var itemDataList = _parameters.prizeJsonWraps.Select(x => new PrizeJsonGridItem.Data(x)).ToList();
            scrollGrid.SetItems(itemDataList);
            return base.OnPreOpen(args, token);
        }

        #region EventListener
        public void OnClickCloseButton()
        {
            _parameters?.onCloseWindow();
            Close();
        }
        #endregion
    }
}
