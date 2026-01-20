using System;
using System.Linq;
using System.Threading;
using CruFramework;
using CruFramework.ResourceManagement;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Menu;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Community
{
    public class YellHistoryScrollItem : ScrollGridItem
    {
        public class Info
        {
            public ModelsUYell yell;
            public UserChatUserStatus status;
        }
        
        [SerializeField] private TextMeshProUGUI sendTimeText;
        [SerializeField] private TextMeshProUGUI userNameText;
        [SerializeField] private ItemIconContainer userIconImage;
        [SerializeField] private UIButton yellButton;

        private Info info;
        
        protected override void OnSetView(object value)
        {
            info = value as Info;
            if (info == null) return;
            
            userNameText.text = info.status.name;
            ActiveYellButton();
            SetTimeText();
            userIconImage.SetIcon(ItemIconType.UserIcon,info.status.mIconId);
        }
        
        public void OnClickYell()
        {
            OnClickConfirmYell(info.status.uMasterId,info.status.name);
        }
        
        public void SetTimeText()
        {
            if(info != null) sendTimeText.text = CommunityManager.GetDateTimeDiffByString(info.yell.createdAt);
        }

        private void OnClickConfirmYell(long targetUMasterId,string userName)
        {
            string badgeText = string.Format(StringValueAssetLoader.Instance["community.yell.count_badge"],ConfigManager.Instance.yellLimit - CommunityManager.yellCount, ConfigManager.Instance.yellLimit);
            FollowConfirmModalWindow.Open(new FollowConfirmModalWindow.WindowParams
            {
                UMasterId = targetUMasterId,
                UserName = userName,
                BadgeCountText = badgeText,
                onClosed = ActiveYellButton
            });
        }

        public void OnClickUserIcon()
        {
            TrainerCardModalWindow.WindowParams param = new TrainerCardModalWindow.WindowParams(info.status.uMasterId);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainerCard, param);
        }

        private void ActiveYellButton()
        {
            bool canYell = CommunityManager.CheckCanYell(info.status.uMasterId);
            if(yellButton != null) yellButton.interactable = canYell;
        }
    }
}