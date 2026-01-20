
using System.Threading;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.UI;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.Colosseum
{
    public class ColosseumRecordModal : ModalWindow
    {

        public class ModalParams
        {
            public ColosseumEventMasterObject mColosseumEvent;
            
            public ModalParams(ColosseumEventMasterObject mColosseumEvent)
            {
                this.mColosseumEvent = mColosseumEvent;
            }
        }
        
        [SerializeField] private ListContainer rankingContainer;
        [SerializeField] private GameObject notificationText;
        
        protected ModalParams modalParams;
        
        public static void Open(ModalParams modalParams)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ColosseumRecord,modalParams);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            if (args != null)
            {
                modalParams = (ModalParams)args;
            }
            notificationText.gameObject.SetActive(false);
            return base.OnPreOpen(args, token);
        }

        protected override void OnOpened()
        {
            UpdateView();
            base.OnOpened();
        }

        private async void UpdateView()
        {
            if (modalParams.mColosseumEvent == null)
            {
                notificationText.gameObject.SetActive(true);
                return;
            }
            
            var historyList = await GetHistoryList();
            
            if (historyList == null || historyList.Length == 0)
            {
                notificationText.gameObject.SetActive(true);
                return;
            }
            
            notificationText.gameObject.SetActive(false);
            var dataList = new List<ColosseumRecordItem.Data>();
            foreach (var history in historyList)
            {
                var data = new ColosseumRecordItem.Data();
                data.history = history;
                dataList.Add(data);
            }
            rankingContainer.SetDataList(dataList);
        }

        protected virtual async UniTask<ColosseumHistory[]> GetHistoryList()
        {
            return await ColosseumManager.RequestGetHistoryListAsyncByMColosseumEventId(modalParams.mColosseumEvent.id);
        }

        #region EventListeners
        public void OnClickClose()
        {
            Close();
        }
        #endregion
    }
}