using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using MagicOnion;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameChatView : MonoBehaviour
    {
        [SerializeField] private ScrollDynamic scrollDynamic;
        private float chatUpdateScrollThreshold = 250f;

        private void Awake()
        {
            var chatList = new List<GuildBattleCommonChatData>();
            scrollDynamic.SetItems(chatList);
        }

        public void InitializeUI(List<GuildBattleCommonChatData> allChat)
        {
            scrollDynamic.SetItems(allChat);
            ReloadAndScrollToEnd().Forget();
        }

        public void AddReceivedChat(GuildBattleCommonChatData chatData)
        {
            var isLookLatest = IsLookLatest();
            scrollDynamic.AddItem(chatData);
            if (isLookLatest)
            {
                ReloadAndScrollToEnd().Forget();
            }
        }
        
        private bool IsLookLatest()
        {
            var contentHeight = scrollDynamic.content.sizeDelta.y - scrollDynamic.viewport.rect.height;
            // スクロール量
            var scrollValue = contentHeight * scrollDynamic.normalizedPosition.y;
            // 指定量スクロールされている
            return scrollValue < chatUpdateScrollThreshold;
        }

        private async UniTask ReloadAndScrollToEnd()
        {
            await UniTask.NextFrame();
            scrollDynamic.ForceLoadAll();
            scrollDynamic.ScrollToEnd();
        }
    }
}