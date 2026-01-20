using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.Page;
using CruFramework.UI;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.App.Request;
using TMPro;

namespace Pjfb.Common
{
    public class RewardModal : ModalWindow
    {
        #region InnerClass
        public class Parameters
        {
            public IList<PrizeJsonWrap> prizeJsonWraps;
            public Action onWindowClosed;
            public string buttonStringKey;

            public Parameters(IList<PrizeJsonWrap> prizeJsonWraps, Action onWindowClosed, string buttonStringKey = "common.close")
            {
                this.prizeJsonWraps = prizeJsonWraps;
                this.onWindowClosed = onWindowClosed;
                this.buttonStringKey = buttonStringKey;
            }
            
            public Parameters(IList<Pjfb.Master.PrizeJsonWrap> prizeJsonWraps, Action onWindowClosed, string buttonStringKey = "common.close")
            {
                this.prizeJsonWraps = prizeJsonWraps.Select(x => PrizeJsonUtility.GetPrizeContainerData(x).prizeJsonWrap).ToList();
                this.onWindowClosed = onWindowClosed;
                this.buttonStringKey = buttonStringKey;
            }
            
            public Parameters(IList<HuntPrizeSet> prizeSetList, Action onWindowClosed, string buttonStringKey = "common.close")
            {
                this.prizeJsonWraps = prizeSetList?.SelectMany(aData => aData.prizeJsonList).ToList() ?? new List<PrizeJsonWrap>();
                this.onWindowClosed = onWindowClosed;
                this.buttonStringKey = buttonStringKey;
            }
            
        }
        #endregion

        #region SerializeFields
        [SerializeField] private ScrollGrid scrollGrid;
        [SerializeField] private TextMeshProUGUI buttonText;
        #endregion
        
        #region PrivateFields
        private Parameters _parameters;
        #endregion
        
        #region PublicStaticMethods
        public static void Open(Parameters parameters, ModalOptions modalOptions = ModalOptions.None)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Reward, parameters, modalOptions);
        }
        
        public static async UniTask<CruFramework.Page.ModalWindow> OpenAsync(Parameters parameters, ModalOptions modalOptions = ModalOptions.None)
        {
            return await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.Reward, parameters, modalOptions);
        }

        public static void TryOpen(Parameters parameters, ModalOptions modalOptions = ModalOptions.None)
        {
            if (parameters.prizeJsonWraps is {Count: > 0}) Open(parameters, modalOptions);
            else parameters.onWindowClosed?.Invoke();
        }
        #endregion

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            SetCloseParameter(false);
            _parameters = (Parameters) args;
            _parameters.prizeJsonWraps ??= new List<PrizeJsonWrap>();
            var itemDataList = _parameters.prizeJsonWraps.Select(aData => new RewardGridItem.Parameters{prizeJsonWrap = aData}).ToList();
            scrollGrid.SetItems(itemDataList);
            buttonText.text = StringValueAssetLoader.Instance[_parameters.buttonStringKey];
            return base.OnPreOpen(args, token);
        }

        #region EventListener
        public void OnClickCloseButton()
        {
            SetCloseParameter(true);
            Close(_parameters?.onWindowClosed);
        }
        #endregion
    }
}
