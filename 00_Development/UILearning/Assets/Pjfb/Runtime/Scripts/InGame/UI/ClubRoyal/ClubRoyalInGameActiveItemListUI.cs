using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameActiveItemListUI : MonoBehaviour
    {
        [SerializeField] private ClubRoyalInGameActiveItemUI originalActiveItemUI;

        private Dictionary<long, ClubRoyalInGameActiveItemUI> itemUIs = new Dictionary<long, ClubRoyalInGameActiveItemUI>();
        private bool isInitialized = false;

        private void Awake()
        {
            originalActiveItemUI.gameObject.SetActive(false);
        }

        private void Initialize(GuildBattlePlayerData playerData)
        {
            isInitialized = true;
            originalActiveItemUI.gameObject.SetActive(false);
            foreach (var item in playerData.GuildBattleItemList)
            {
                var obj = Instantiate(originalActiveItemUI, originalActiveItemUI.transform.parent);
                obj.gameObject.SetActive(true);
                obj.Initialize(item);
                itemUIs.Add(item.ItemId, obj);
            }
        }

        public void UpdateUI(GuildBattlePlayerData playerData)
        {
            if(!isInitialized)
            {
                Initialize(playerData);
            }
            
            foreach (var item in playerData.GuildBattleItemList)
            {
                if (itemUIs.TryGetValue(item.ItemId, out var ui))
                {
                    ui.UpdateUI(item);
                }
            }
        }
    }
}