using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.Page;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.UI;

namespace Pjfb.SubscribeInfo
{
    public class SubscribeInfoModal : ModalWindow
    {
        #region InnerClass
        public class Parameters
        {
            public IEnumerable<NativeApiTagContainer> orderedOpenTagObject;
            public Action onWindowClosed;

            public Parameters(IEnumerable<NativeApiTagContainer> orderedOpenTagObject, Action onWindowClosed)
            {
                this.orderedOpenTagObject = orderedOpenTagObject;
                this.onWindowClosed = onWindowClosed;
            }
        }
        #endregion

        #region SerializeFields
        [SerializeField] private PoolListContainer poolListContainer;
        #endregion
        
        #region PrivateFields
        private Parameters _parameters;
        #endregion
        
        #region PublicStaticMethods
        public static void Open(Parameters parameters, ModalOptions modalOptions = ModalOptions.None)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.SubscribeInfo, parameters, modalOptions);
        }
        #endregion

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _parameters = (Parameters) args;

            var itemParams = new List<SubscribeInfoPoolListItem.ItemParams>();
            _parameters.orderedOpenTagObject.ToList().ForEach(anOpenTagObject => {
                itemParams.AddRange(anOpenTagObject.loginPassMasterObjectList
                    .Select(aLoginPass => new SubscribeInfoPoolListItem.ItemParams(new SubscribeInfoDescriptionView.ViewParams(
                        title: aLoginPass.name, // パス名 
                        description: aLoginPass.description)))); // ...n上昇の情報
                
                itemParams.Add(new SubscribeInfoPoolListItem.ItemParams(new SubscribeInfoExpireDateView.ViewParams(expireDateTime: anOpenTagObject.expireAtDateTime)));
            });
                
            poolListContainer.SetDataList(itemParams).Forget();
            return base.OnPreOpen(args, token);
        }

        #region EventListener
        public void OnClickCloseButton()
        {
            Close(_parameters?.onWindowClosed);
        }
        #endregion
    }
}
