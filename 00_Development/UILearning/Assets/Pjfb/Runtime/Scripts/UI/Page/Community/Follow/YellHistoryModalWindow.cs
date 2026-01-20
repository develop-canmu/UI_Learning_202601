using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.Page;
using CruFramework.UI;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;

namespace Pjfb.Community
{
    public class YellHistoryModalWindow : ModalWindow
    {
        #region Params

        public class WindowParams
        {
            public CommunityGetYellListAPIResponse yellListResponse;
            public Action onClosed;
        }
        
        [SerializeField] private ScrollGrid scroller;
        [SerializeField] private GameObject scrollEmptyTextRoot;
        
        private WindowParams _windowParams;
        private CancellationTokenSource cancellationTokenSource = null;
        
        private List<YellHistoryScrollItem.Info> yellInfoList = new List<YellHistoryScrollItem.Info>();
        private List<YellHistoryScrollItem> cacheScrollItems = new List<YellHistoryScrollItem>();
        #endregion
        
        #region Life Cycle
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.YellHistory, data);
        }

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            await base.OnPreOpen(args, token);
        }

        protected override async void OnOpened()
        {
            await Init();
            UpdateScrollItems();
            //Scroll　Itemのtime Text更新
            UpdateScrollItemTimeText().Forget();
            base.OnOpened();
        }

        protected override void OnClosed()
        {
            if(cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
            base.OnClosed();
        }

        #endregion
        
        #region PrivateMethods
        private async UniTask Init()
        {
            var response = await GetYellListAPI();
            yellInfoList.Clear();
            foreach (var modelsUYell in response.uYellList)
            {
                //ブロックユーザー非表示
                if(CommunityManager.blockUserList.Any(user => user.uMasterId == modelsUYell.fromUMasterId)) continue;
                yellInfoList.Add(new YellHistoryScrollItem.Info
                {
                    yell = modelsUYell,
                    status = response.chatUserStatusList.FirstOrDefault(s => s.uMasterId == modelsUYell.fromUMasterId)
                });
            }
        }
        #endregion

        #region EventListeners
        public void OnClickClose()
        {
            Close(onCompleted: _windowParams.onClosed);
        }
        #endregion
       
        #region API
        private async UniTask<CommunityGetYellListAPIResponse> GetYellListAPI()
        {
            CommunityGetYellListAPIRequest request = new CommunityGetYellListAPIRequest();
            await APIManager.Instance.Connect(request);
            CommunityGetYellListAPIResponse response = request.GetResponseData();
            return response;
        }
        #endregion
        
        #region Other

        private async void UpdateScrollItems()
        {
            scroller.SetItems(yellInfoList);
            scrollEmptyTextRoot.SetActive(!yellInfoList.Any());
            //scroller item　active状態更新のため,1 frame待ち
            await UniTask.NextFrame();
            cacheScrollItems.Clear();
            foreach (Transform child in scroller.content)
            {
                if (!child.gameObject.activeSelf) continue;
                var item = child.GetComponent<YellHistoryScrollItem>();
                if(item != null) cacheScrollItems.Add(item);
            }
        }
        
        private async UniTask UpdateScrollItemTimeText()
        {
            if(cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
            cancellationTokenSource = new CancellationTokenSource();

            await CommunityManager.UpdateActionInterval(5, () =>
            {
                cacheScrollItems.ForEach(item=>item.SetTimeText());
            }, cancellationTokenSource: cancellationTokenSource);
        }

        #endregion
        
    }
}