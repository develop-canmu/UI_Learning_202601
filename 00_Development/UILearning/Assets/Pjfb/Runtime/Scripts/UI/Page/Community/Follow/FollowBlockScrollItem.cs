using System;
using System.Linq;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.Menu;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine;

namespace Pjfb.Community
{
    [Serializable]
    public class FollowBlockScrollItemInfo
    {
        public UserCommunityUserStatus userStatus;
        public Action OnClickYell = null;
        public Action OnClickUnfollow = null;
        public Action OnClickUnblock = null;
        public Action OnClickChat = null;
        public Action OnClickFollow = null;
        public Action OnClickBlock = null;
        public FollowTabStatus tabStatus = FollowTabStatus.None;
        public bool showUserProfileOtherButtons = true; 
    }
    public class FollowBlockScrollItem : ScrollGridItem
    {
        [SerializeField] private TextMeshProUGUI loginTimeText;
        [SerializeField] private TextMeshProUGUI userNameText;
        [SerializeField] private TextMeshProUGUI rankText;
        [SerializeField] private TextMeshProUGUI totalPowerText;
        [SerializeField] private OmissionTextSetter totalPowerOmissionTextSetter;
        [SerializeField] private ItemIconContainer userIconImage;
        [SerializeField] private UserTitleImage userTitleImage;
        [SerializeField] private GameObject followRoot;
        [SerializeField] private GameObject blockRoot;
        [SerializeField] private UIButton followButton;
        [SerializeField] private UIButton unfollowButton;
        [SerializeField] private UIButton yellButton;

        private FollowBlockScrollItemInfo info;
        
        protected override void OnSetView(object value)
        {
            info = value as FollowBlockScrollItemInfo;
            if(info == null) return;
            
            followRoot.SetActive(info.tabStatus != FollowTabStatus.Block);
            blockRoot.SetActive(info.tabStatus == FollowTabStatus.Block);
            
            followButton.gameObject.SetActive(info.OnClickFollow != null);
            unfollowButton.gameObject.SetActive(info.OnClickUnfollow != null);
            ActiveButtons();

            userNameText.text = info.userStatus.name;
            rankText.text = StatusUtility.GetRankNameByRankNumber(CharaRankMasterStatusType.PartyTotal,info.userStatus.maxDeckRank);
            BigValue maxCombatPower = new BigValue(info.userStatus.maxCombatPower);
            totalPowerText.text = maxCombatPower.ToDisplayString(totalPowerOmissionTextSetter.GetOmissionData());
            SetTimeText();

            userIconImage.SetIcon(ItemIconType.UserIcon,info.userStatus.mIconId);
            userTitleImage.SetTexture(info.userStatus.mTitleId);
        }
        
        public void SetTimeText()
        {
            if(info != null) loginTimeText.text = CommunityManager.GetDateTimeDiffByString(info.userStatus.lastLogin);
        }

        #region EventListeners
        
        public void OnClickFollow()
        {
            info.OnClickFollow?.Invoke();
        }
        public void OnClickYell()
        {
            info.OnClickYell?.Invoke();
        }
        
        public void OnClickChat()
        {
            info.OnClickChat?.Invoke();
        }
        
        public void OnClickUnfollow()
        {
            info.OnClickUnfollow?.Invoke();
        }
        
        public void OnClickUnblock()
        {
            info.OnClickUnblock?.Invoke();
        }
        
        public void OnClickBlock()
        {
            info.OnClickBlock?.Invoke();
        }

        public void OnClickUserIcon()
        {
            if (info.userStatus == null || info.userStatus.uMasterId <= 0) return;
            TrainerCardModalWindow.WindowParams param = new TrainerCardModalWindow.WindowParams(info.userStatus.uMasterId, info.showUserProfileOtherButtons);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainerCard, param);
        }
        #endregion

        #region Other
        
        public void ActiveButtons()
        {
            //エールボタン設定
            bool canYell = CommunityManager.CheckCanYell(info.userStatus.uMasterId);
            if(yellButton != null) yellButton.interactable = canYell;
            
            //フォローボタン設定
            bool isFollowed = CommunityManager.followUserList.Any(user => user.uMasterId == info.userStatus.uMasterId);
            if(followButton != null) followButton.interactable = !isFollowed;
        }
        
        #endregion
        
    }
}