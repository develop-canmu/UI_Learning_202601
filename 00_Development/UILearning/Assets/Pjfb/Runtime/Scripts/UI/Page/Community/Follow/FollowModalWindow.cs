using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using TMPro;

namespace Pjfb.Community
{
    [Serializable]
    public enum FollowTabStatus
    {
        None,
        Follow,
        Block
    }
    public class FollowModalWindow : ModalWindow
    {
        #region Params
        public class WindowParams
        {
            public FollowTabStatus Status;
            public Action onClosed;
            public bool showUserProfileOtherButtons = true; 
        }

        [Serializable]
        public class FollowTabInfo
        {
            public FollowTabStatus status;
            public FollowSheetButton button;
        }

        [SerializeField] private List<FollowTabInfo> tabList;
        [SerializeField] private ScrollGrid scroller;
        [SerializeField] private TextMeshProUGUI scrollEmptyText;
        [SerializeField] private TextMeshProUGUI followCountText;
        [SerializeField] private FollowSheetManager sheetManager;
        [SerializeField] private UINotification notificationUI;

        private WindowParams _windowParams;
        private List<FollowBlockScrollItemInfo> followUserList = new List<FollowBlockScrollItemInfo>();
        private List<FollowBlockScrollItemInfo> blockUserList = new List<FollowBlockScrollItemInfo>();
        private List<FollowBlockScrollItem> cacheScrollItems = new List<FollowBlockScrollItem>();
        private int sendYellCount;
        private CancellationTokenSource cancellationTokenSource = null;

        #endregion

        #region Life Cycle
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Follow, data);
        }

        protected override void OnOpened()
        {
            if(NeedUpdate()) 
            {
                UpdateScrollItems();
            }
            else
            {
                bool isEmpty = _windowParams.Status switch
                {
                    FollowTabStatus.Follow => !followUserList.Any(),
                    FollowTabStatus.Block => !blockUserList.Any(),
                    _ => false
                };
                scrollEmptyText.gameObject.SetActive(isEmpty);
                if (_windowParams.Status == FollowTabStatus.Follow)
                {
                    followCountText.text = string.Format(StringValueAssetLoader.Instance["community.follow.limit_count"],followUserList.Count,ConfigManager.Instance.followingMaxCount);
                }
            }

            //Scroll　Itemのtime Text更新
            UpdateScrollItemTimeText().Forget();
            tabList.ForEach(tab =>
            {
                tab.button.OnTabClick = () => OnActiveFollowTab(tab.status);
            });
            UpdateScrollItemButton();
            base.OnOpened();
        }

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            await Init();
            await base.OnPreOpen(args, token);
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
            await AppManager.Instance.LoadingActionAsync(async () =>
            {
                await UpdateData();
                _windowParams.Status = (!CommunityManager.followUserList.Any() && CommunityManager.blockUserList.Any()) ? FollowTabStatus.Block : FollowTabStatus.Follow ;
                sheetManager.OpenSheet(_windowParams.Status  == FollowTabStatus.Block ? FollowSheetType.Block : FollowSheetType.Follow,null);
            });
        }
        #endregion

        #region EventListeners
        public void OnClickClose()
        {
            Close(onCompleted: _windowParams.onClosed);
        }
        
        public void OnClickUpdate()
        {
            UpdateData().Forget();
        }

        public void OnClickSearch()
        {
            FollowSearchModalWindow.Open(new FollowSearchModalWindow.WindowParams{showUserProfileOtherButtons = _windowParams.showUserProfileOtherButtons});
        }
        
        private void OnClickConfirmYell(long targetUMasterId,string userName)
        {
            string badgeText = string.Format(StringValueAssetLoader.Instance["community.yell.count_badge"],ConfigManager.Instance.yellLimit - CommunityManager.yellCount, ConfigManager.Instance.yellLimit);
            FollowConfirmModalWindow.Open(new FollowConfirmModalWindow.WindowParams
            {
                BadgeCountText = badgeText,
                UMasterId = targetUMasterId,
                UserName = userName,
                onClosed = UpdateScrollItemButton 
            });
        }

        private void OnClickChat(long targetUMasterId)
        {
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(window => true);
            Close(onCompleted: () =>
            {
                var info = new CommunityPage.CommunityPageInfo{ Status = CommunityStatus.PersonalChat, TargetUMasterId = targetUMasterId};
                
                if (AppManager.Instance.UIManager.PageManager.CurrentPageType == PageType.Community)
                {
                    (AppManager.Instance.UIManager.PageManager.CurrentPageObject as CommunityPage)?.OnClickOpenPersonalChat(targetUMasterId);
                }
                else
                {
                    AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Community, true, info);
                }
            });
        }

        private void OnClickUnFollow(UserCommunityUserStatus userStatus)
        {
            FollowConfirmModalWindow.Open(new FollowConfirmModalWindow.WindowParams()
            {
                OnClickUnfollow = () => UnfollowAPI(userStatus).Forget(),
            });
        }

        #endregion

        #region API
        private async UniTask<CommunityGetTodayYellDetailAPIResponse> GetTodayYellDetailAPI()
        {
            CommunityGetTodayYellDetailAPIRequest request = new CommunityGetTodayYellDetailAPIRequest();
            await APIManager.Instance.Connect(request);
            CommunityGetTodayYellDetailAPIResponse response = request.GetResponseData();
            return response;
        }

        private async UniTask GetFollowListAPI()
        {
            CommunityGetFollowListAPIRequest request = new CommunityGetFollowListAPIRequest();
            await APIManager.Instance.Connect(request);
        }
        
        private async UniTask GetBlockListAPI()
        {
            CommunityGetBlockListAPIRequest request = new CommunityGetBlockListAPIRequest();
            await APIManager.Instance.Connect(request);
        }
        
        private async UniTask UnfollowAPI(UserCommunityUserStatus userStatus)
        {
            CommunityUnfollowAPIRequest request = new CommunityUnfollowAPIRequest();
            CommunityUnfollowAPIPost post = new CommunityUnfollowAPIPost{targetUMasterId = userStatus.uMasterId};
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            string message = string.Format(StringValueAssetLoader.Instance["community.follow.unfollowed"],userStatus.name);
            notificationUI.ShowNotification(message);
            CommunityManager.followUserList.Remove(userStatus);
            CommunityManager.yellDetail.followedCount--;
            if(CommunityManager.yellDetail.todayYelledList.All(y => y.uMasterId != userStatus.uMasterId))
            {
                CommunityManager.yellDetail.followedCanYellCount--;
                //メニューバッジ更新
                AppManager.Instance.UIManager.Header.UpdateMenuBadge();
            }
            
            if(NeedUpdate()) UpdateScrollItems();
        }
        
        private async UniTask UnblockAPI(UserCommunityUserStatus userStatus)
        {
            CommunityRemoveBlockAPIRequest request = new CommunityRemoveBlockAPIRequest();
            CommunityRemoveBlockAPIPost post = new CommunityRemoveBlockAPIPost{targetUMasterId = userStatus.uMasterId};
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            string message = string.Format(StringValueAssetLoader.Instance["community.unblocked"],userStatus.name);
            notificationUI.ShowNotification(message);
            CommunityManager.blockUserList.Remove(userStatus);
            if(NeedUpdate())UpdateScrollItems();
            
            //コミュニティチャットリスト更新
            if (AppManager.Instance.UIManager.PageManager.CurrentPageType == PageType.Community)
            {
                ((CommunityPage)AppManager.Instance.UIManager.PageManager.CurrentPageObject).UpdateBlockChatList(userStatus.uMasterId);
            }
        }

        #endregion
       
        #region Other

        private async void UpdateScrollItems()
        {
            switch (_windowParams.Status)
            {
                case FollowTabStatus.Follow:
                    scroller.SetItems(followUserList);
                    scrollEmptyText.text = StringValueAssetLoader.Instance["community.follow.empty"];
                    followCountText.text = string.Format(StringValueAssetLoader.Instance["community.follow.limit_count"],followUserList.Count,ConfigManager.Instance.followingMaxCount) ;
                    scrollEmptyText.gameObject.SetActive(!followUserList.Any());
                    break;
                case FollowTabStatus.Block:
                    scroller.SetItems(blockUserList);
                    scrollEmptyText.text = StringValueAssetLoader.Instance["community.block.empty"];
                    scrollEmptyText.gameObject.SetActive(!blockUserList.Any());
                    break;
            }

            //scroller item　active状態更新のため,1 frame待ち
            await UniTask.NextFrame();
            cacheScrollItems.Clear();
            foreach (Transform child in scroller.content)
            {
                if (!child.gameObject.activeSelf) continue;
                var item = child.GetComponent<FollowBlockScrollItem>();
                if(item != null) cacheScrollItems.Add(item);
            }
        }
        
        private void OnActiveFollowTab(FollowTabStatus tabType)
        {
            _windowParams.Status = tabType;
            UpdateScrollItems();
        }

        private async UniTask<CommunityGetTodayYellDetailAPIResponse> UpdateBadge()
        {
            var res = await GetTodayYellDetailAPI();
            UpdateScrollItemButton();
            return res;
        }
        
        private async void InitAndUpdateScrollItems(bool needUpdate)
        {
            if (needUpdate)
            {
                await Init();
                UpdateScrollItems();
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

        private async UniTask UpdateData()
        {
            await GetFollowListAPI();
            await GetBlockListAPI();
            await UpdateBadge();
        }

        private void UpdateScrollItemButton()
        {
            cacheScrollItems.ForEach(item =>item.ActiveButtons());
        }

        private bool NeedUpdate()
        {
            bool needUpdate = false;
            if (followUserList.Count != CommunityManager.followUserList.Count)
            {
                needUpdate = true;
                //フォローリスト更新
                followUserList = GetFollowInfoList();
            }

            if (blockUserList.Count != CommunityManager.blockUserList.Count)
            {
                needUpdate = true;
                blockUserList = GetBlockInfoList();
            }
            
            return needUpdate;
        }

        private List<FollowBlockScrollItemInfo> GetFollowInfoList()
        {
            var list = CommunityManager.followUserList.Select(status => new FollowBlockScrollItemInfo
            {
                    userStatus = status,
                    tabStatus = FollowTabStatus.Follow,
                    OnClickYell = () => OnClickConfirmYell(status.uMasterId,status.name),
                    OnClickChat = () => OnClickChat(status.uMasterId),
                    OnClickUnfollow = () => OnClickUnFollow(status),
                    showUserProfileOtherButtons = _windowParams.showUserProfileOtherButtons
            }).ToList();
            return list;
        }

        private List<FollowBlockScrollItemInfo> GetBlockInfoList()
        {
            var list = CommunityManager.blockUserList.Select(status => new FollowBlockScrollItemInfo
            {
                userStatus = status,
                tabStatus = FollowTabStatus.Block,
                OnClickUnblock = () => UnblockAPI(status).Forget(),
                showUserProfileOtherButtons = _windowParams.showUserProfileOtherButtons
            }).ToList();
            return list;
        }

        #endregion
        
        
    }
}