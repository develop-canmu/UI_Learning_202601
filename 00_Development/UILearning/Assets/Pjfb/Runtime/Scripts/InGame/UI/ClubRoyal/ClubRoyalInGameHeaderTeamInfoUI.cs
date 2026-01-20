using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using MagicOnion;
using TMPro;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameHeaderTeamInfoUI : MonoBehaviour
    {
        [SerializeField] private ClubRoyalInGameHeaderPlayerInfoUI playerInfoUIPrefab;
        [SerializeField] private ClubEmblemImage emblemImage;
        [SerializeField] private TMP_Text clubNameText;
        [SerializeField] private GameObject playerInfoListRoot;
        [SerializeField] private GameObject[] openListImageObjects;
        
        private Dictionary<long, ClubRoyalInGameHeaderPlayerInfoUI> playerInfoUis = new Dictionary<long, ClubRoyalInGameHeaderPlayerInfoUI>();

        private bool isInitialized = false;

        private void Awake()
        {
            playerInfoUIPrefab.gameObject.SetActive(false);
        }

        public void InitializeUI(long emblemId, string clubName, List<BattlePlayerModel> playerData)
        {
            if (isInitialized)
            {
                return;
            }

            isInitialized = true;
            
            emblemImage.SetTextureAsync(emblemId).Forget();
            clubNameText.text = clubName;
            foreach (var data in playerData)
            {
                var instance = Instantiate(playerInfoUIPrefab, playerInfoUIPrefab.transform.parent);
                instance.InitializeUI(data.IconId, data.Name);
                playerInfoUis.Add(data.UserId, instance);
            }
        }

        public void UpdateActivePlayerView(long playerId, bool isActive)
        {
            if (playerInfoUis.TryGetValue(playerId, out var infoUI))
            {
                infoUI.SetOnlineStatus(isActive);
            }
        }

        private bool isOpen = false;
        public void OnClickSwitchActivePlayerListOpen()
        {
            isOpen = !isOpen;
            playerInfoListRoot.SetActive(isOpen);
            openListImageObjects[0].SetActive(isOpen);
            openListImageObjects[1].SetActive(!isOpen);
        }
    }
}