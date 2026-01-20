using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using MagicOnion;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameRankingResultItem : MonoBehaviour
    {
        [SerializeField] private UserIcon userIcon;
        [SerializeField] private TMP_Text userNameText;
        [SerializeField] private TMP_Text valueText;
        [SerializeField] private Image baseImage;
        [SerializeField] private Image rankImage;
        [SerializeField] private Sprite[] baseSprites;
        [SerializeField] private Sprite[] rankSprites;

        public void SetUI(long userIconId, string userName, BigValue value)
        {
            userIcon.SetTextureAsync(userIconId);
            userNameText.text = userName;
            valueText.text = value.ToString();
        }
        
        public void SetUI(long userIconId, string userName, long value, int rank, bool isAlly)
        {
            userIcon.SetTextureAsync(userIconId);
            userNameText.text = userName;
            valueText.text = value.ToString();
            rankImage.sprite = rankSprites[Mathf.Clamp(rank, 0, rankSprites.Length - 1)];
            baseImage.sprite = baseSprites[isAlly ? 0 : 1]; 
        }

    }
}