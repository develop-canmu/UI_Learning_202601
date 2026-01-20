using System;
using MagicOnion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Community
{
    public class ChatItem : MonoBehaviour
    {
        [SerializeField]
        private GameObject stampRoot;
        [SerializeField]
        private TextMeshProUGUI sendTimeText;
        [SerializeField]
        private TextMeshProUGUI chatUserNameText;
        [SerializeField]
        private TextMeshProUGUI bodyText;
        [SerializeField]
        private Image chatRoot;
        [SerializeField]
        private IconImage stampImage;
        [SerializeField]
        private ItemIconContainer userIconImage;

        private string sendTimeString;

        public void Init(ChatInfo info, string userName, long mIconId)
        {
            bool isText = info.MessageType is MessageType.Message or MessageType.System;
            bool isStamp = info.MessageType is MessageType.Stamp;

            chatRoot.gameObject.SetActive(isText);
            stampRoot.gameObject.SetActive(isStamp);

            chatUserNameText.SetText(userName);
            sendTimeString = info.CreatedAt;
            SetTimeText();
            
            if (isStamp)
            {
                stampImage.SetTexture(int.Parse(info.MessageText));
            }
            else
            {
                bodyText.text = info.MessageText;
            }

            userIconImage.SetIcon(ItemIconType.UserIcon, mIconId);
        }

        public void Init(GuildBattleCommonChatData chatData, string userName, long iconId)
        {
            var isStamp = chatData.IsStampChatData();
            chatRoot.gameObject.SetActive(!isStamp);
            stampRoot.gameObject.SetActive(isStamp);
            
            chatUserNameText.SetText(userName);
            // これ他人のアイコン表示される可能性ない?
            userIconImage.SetIcon(ItemIconType.UserIcon, iconId);
            if (isStamp)
            {
                stampImage.SetTexture(chatData.StampId);
            }
            else
            {
                bodyText.text = chatData.Body;
            }
            
            // 表示する必要あれば.
            sendTimeString = string.Empty;
        }
        
        public void SetTimeText()
        {
            sendTimeText.text = string.IsNullOrEmpty(sendTimeString) ? string.Empty : CommunityManager.GetDateTimeDiffByString(sendTimeString);
        }
    }
}