using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameBeforeFightTimerUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;

        private DateTime utcStartAT;
        public void StartTimer(DateTime utcStartAt)
        {
            gameObject.SetActive(true);
            this.utcStartAT = utcStartAt;
            CountDownAsync().Forget();
        }

        private async UniTask CountDownAsync()
        {
            while (gameObject.activeSelf)
            {
                var utcNow = AppTime.Now.ToUniversalTime();
                var remainTime = utcStartAT - utcNow;
                if (remainTime.TotalSeconds <= 0)
                {
                    break;
                }
                
                timerText.text = $"{(int)remainTime.TotalMinutes:00}:{remainTime.Seconds:00}";
                await UniTask.Delay(TimeSpan.FromSeconds(0.3f), cancellationToken: destroyCancellationToken);
            }
            
            gameObject.SetActive(false);
        }
    }
}