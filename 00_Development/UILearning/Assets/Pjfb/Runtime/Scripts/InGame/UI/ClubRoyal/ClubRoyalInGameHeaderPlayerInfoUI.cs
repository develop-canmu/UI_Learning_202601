using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameHeaderPlayerInfoUI : MonoBehaviour
    {
        [SerializeField] private UserIcon userIcon;
        [SerializeField] private TMP_Text userNameText;
        [SerializeField] private GameObject offlineEffect;

        public void InitializeUI(long userIconId, string userName)
        {
            gameObject.SetActive(true);
            userIcon.SetTextureAsync(userIconId).Forget();
            userNameText.text = userName;
        }

        public void SetOnlineStatus(bool isOnline)
        {
            offlineEffect.SetActive(!isOnline);
        }
    }
}