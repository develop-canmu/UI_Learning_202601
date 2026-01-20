using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework;
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
    public class ChatRoomInfo
    {
        public ModelsUChat uChat;
        public UserChatUserStatus uStatus;
        public Action<long> OnChatButtonClick;
    }
    public class ChatRoomScrollItem : ScrollGridItem
    {
        [SerializeField] private ItemIconContainer iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI lastReceivedAtText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private GameObject attentionBadge;

        private ChatRoomInfo info;
        protected override void OnSetView(object value)
        {
            info = value as ChatRoomInfo;
            if (info == null) return;
            
            nameText.text = info.uStatus.name;
            bool isMyself = info.uChat.fromUMasterId == UserDataManager.Instance.user.uMasterId;
            messageText.text = (MessageType)info.uChat.type switch
            {
                MessageType.Stamp => string.Format(StringValueAssetLoader.Instance["community.chat.stamp_sent"], isMyself ? UserDataManager.Instance.user.name : info.uStatus.name),
                _ => info.uChat.body
            };
            attentionBadge.SetActive(!isMyself && info.uChat.readFlg == (int)ReadFlgType.NotView);
            SetTimeText();
            iconImage.SetIcon(ItemIconType.UserIcon,info.uStatus.mIconId);
        }

        public void OnClickChatButton()
        {
            info.OnChatButtonClick?.Invoke(info.uStatus.uMasterId);
        }

        public void OnClickUserIcon()
        {
            if (info.uStatus == null || info.uStatus.uMasterId <= 0) return;
            TrainerCardModalWindow.WindowParams param = new TrainerCardModalWindow.WindowParams(info.uStatus.uMasterId);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainerCard, param);
        }

        public void SetTimeText()
        {
            lastReceivedAtText.text = string.IsNullOrEmpty(info.uChat.createdAt) ? string.Empty : CommunityManager.GetDateTimeDiffByString(info.uChat.createdAt);
        }
    }
}


