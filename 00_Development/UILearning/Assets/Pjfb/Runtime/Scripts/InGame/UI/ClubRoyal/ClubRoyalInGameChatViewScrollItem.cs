using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using MagicOnion;
using Pjfb.Community;
using Pjfb.UserData;
using UnityEngine.UI;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameChatViewScrollItem : ScrollDynamicItem
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private ChatItem myselfView;
        [SerializeField] private ChatItem otherView;
        [SerializeField] private GActionLogItem logView;
        
        protected override void OnSetView(object value)
        {
            myselfView.gameObject.SetActive(false);
            otherView.gameObject.SetActive(false);
            logView.gameObject.SetActive(false);
            
            var data = (GuildBattleCommonChatData)value;
            var userId = UserDataManager.Instance.user.uMasterId;
            if (data.IsSystemLog())
            {
                logView.Init(data);
                logView.gameObject.SetActive(true);
            }
            else
            {
                var playerData = PjfbGuildBattleDataMediator.Instance.GetBattlePlayer(data.UserId);
                // failsafe
                if (playerData == null)
                {
                    return;
                }
                
                if (data.IsSelfChatData(userId))
                {
                    myselfView.Init(data, playerData.Name, playerData.IconId);
                    myselfView.gameObject.SetActive(true);
                }
                else
                {
                    otherView.Init(data, playerData.Name, playerData.IconId);
                    otherView.gameObject.SetActive(true);
                }
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }
}