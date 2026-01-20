using System;
using System.Collections.Generic;
using CruFramework.UI;
using Pjfb.Menu;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Community
{
    public class ChatMessageScrollItem : ScrollDynamicItem
    {
        private enum ItemType
        {
            Myself,
            Other,
            Log
        }
        
        [SerializeField]
        private RectTransform rect;
        
        [SerializeField]
        private ChatItem myselfView;
        [SerializeField]
        private ChatItem otherPlayerView;
        [SerializeField]
        private GActionLogItem gActionLog;
        
        private ChatInfo info;
        private ItemType type;
        protected override void OnSetView(object value)
        {
            info = (ChatInfo)value;
            if (info == null) return;
            
            type = DetectType(info);
            
            myselfView.gameObject.SetActive(type == ItemType.Myself);
            otherPlayerView.gameObject.SetActive(type == ItemType.Other);
            gActionLog.gameObject.SetActive(type == ItemType.Log);

            switch (type)
            {
                case ItemType.Myself:
                    var myData = UserDataManager.Instance.user;
                    myselfView.Init(info, myData.name, myData.mIconId);
                    myselfView.SetTimeText();
                    break;
                case ItemType.Other:
                    var dic = (Dictionary<long, UserChatUserStatus>)ParentScrollDynamic.CommonItemValue;
                    if (dic.TryGetValue(info.UMasterId, out var userData))
                    {
                        // 最新のデータを反映
                        otherPlayerView.Init(info, userData.name, userData.mIconId);
                    }
                    else
                    {
                        // 退会している場合、最新のデータが取れないため過去のデータをそのまま反映
                        otherPlayerView.Init(info, info.UserName, info.MIconId);
                    }
                    otherPlayerView.SetTimeText();
                    break;
                case ItemType.Log:
                    gActionLog.Init(info);
                    break;
            }

            // レイアウトの強制計算をかける
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
        
        public void OnClickUserIcon()
        {
            TrainerCardModalWindow.WindowParams param = new TrainerCardModalWindow.WindowParams(info.UMasterId);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainerCard, param);
        }

        private ItemType DetectType(ChatInfo chatInfo)
        {
            bool isClubLog = chatInfo.Type == ChatInfo.ChatType.ClubLog;
            bool isMyself = UserDataManager.Instance.user.uMasterId == chatInfo.UMasterId;

            if (isClubLog)
            {
                return ItemType.Log;
            }

            if (isMyself)
            {
                return ItemType.Myself;
            }
            else
            {
                return ItemType.Other;
            }
        }
    }
}