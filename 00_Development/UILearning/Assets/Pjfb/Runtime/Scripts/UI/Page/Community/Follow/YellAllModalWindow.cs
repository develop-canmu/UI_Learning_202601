using System;
using System.Threading;
using CruFramework.Page;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;

namespace Pjfb.Community
{
    public class YellAllModalWindow : ModalWindow
    {
        #region Params

        public enum YellCategory
        {
            None = 0,
            Followed = 1,
            Follower = 2,
            Guild = 3
        }

        public class WindowParams
        {
            public int followListCount;
            public Action onYellSent;
            public Action<string> showNotification;
        }

        #endregion
        
        [SerializeField] private UIButton sendClubButton;
        [SerializeField] private UIButton sendFollowButton;
        [SerializeField] private TextMeshProUGUI clubCountText;
        [SerializeField] private TextMeshProUGUI followCountText;
        private WindowParams _windowParams;

        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.YellAll, data);
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
            bool isReachYellLimit = CommunityManager.yellCount >= ConfigManager.Instance.yellLimit;
            bool isAllFollowSent = CommunityManager.yellDetail.followedCanYellCount == 0; 
            bool isAllClubSent = CommunityManager.yellDetail.guildCanYellCount == 0;
            sendFollowButton.interactable = !isReachYellLimit && !isAllFollowSent;
            sendClubButton.interactable = !isReachYellLimit && !isAllClubSent;
            clubCountText.text = $"({CommunityManager.yellDetail.guildCanYellCount}/{CommunityManager.yellDetail.guildMemberCount})";
            followCountText.text = $"({CommunityManager.yellDetail.followedCanYellCount}/{CommunityManager.yellDetail.followedCount})";
        }
        #endregion

        #region EventListeners
        /// <summary>
        /// クラブメンバー全員にエール送信ボタンをクリックした後の処理
        /// </summary>
        private async UniTask SendYellToAllGuildMembers()
        {
            long successCount = await SendYellByCategoryAsync((int)YellCategory.Guild);
            
            if (successCount > 0)
            {
                string message = string.Format(StringValueAssetLoader.Instance["community.yell.all_sent"],successCount);
                _windowParams.showNotification?.Invoke(message);
                _windowParams.onYellSent?.Invoke();
                RefreshOrCloseModal();
            }
        }
        /// <summary>
        /// フォロー中のユーザー全員にエール送信ボタンをクリックした後の処理
        /// </summary>
        private async UniTask SendYellToAllFollowedUsers()
        {
            long successCount = await SendYellByCategoryAsync((int)YellCategory.Followed);
            
            if (successCount > 0)
            {
                string message = string.Format(StringValueAssetLoader.Instance["community.yell.all_sent"],successCount);
                _windowParams.showNotification?.Invoke(message);
                _windowParams.onYellSent?.Invoke();
                RefreshOrCloseModal();
            }
        }
        
        public void OnClickYellClub()
        {
            SendYellToAllGuildMembers().Forget();
        }

        public void OnClickYellFollow()
        {
            SendYellToAllFollowedUsers().Forget();
        }
        
        /// <summary>
        /// エール送信後にモーダルを更新するか閉じる
        /// </summary>
        private void RefreshOrCloseModal()
        {
            //送信可能なエールが無い場合はモーダルを閉じる
            if (CommunityManager.yellDetail.followedCanYellCount == 0 && CommunityManager.yellDetail.guildCanYellCount == 0)
            {
                Close();
            }
            else
            {
                //表示の更新
                Init();
            }
        }

        #endregion

        #region API
        
        private async UniTask<long> SendYellByCategoryAsync(int category = 0)
        {
            CommunitySendYellAPIRequest request = new CommunitySendYellAPIRequest();
            CommunitySendYellAPIPost post = new CommunitySendYellAPIPost{category = category};
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            CommunitySendYellAPIResponse response = request.GetResponseData();
            return response.yellResult.successCount;
        }
        
        #endregion

    }
}