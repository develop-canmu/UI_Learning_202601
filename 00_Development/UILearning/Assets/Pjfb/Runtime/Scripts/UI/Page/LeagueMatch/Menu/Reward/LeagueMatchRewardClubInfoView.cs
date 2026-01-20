using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Club;
using UnityEngine.UI;

namespace Pjfb.LeagueMatch
{
    public class LeagueMatchRewardClubInfoView : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TextMeshProUGUI rankText = null;
        [SerializeField]
        private Image rankImage = null;
        
        [SerializeField]
        private TMPro.TextMeshProUGUI nameText = null;
        
        [SerializeField]
        private ClubEmblemImage icon = null;
        
        [SerializeField]
        private Sprite[] rankSprites = null;
        
        
        /// <summary>名前の表示</summary>
        public void SetName(string name)
        {
            nameText.text = name;
        }
        
        /// <summary>名前の表示</summary>
        public void SetRanking(long rank)
        {
            if(rankSprites.Length >= rank)
            {
                // 表示切り替え
                rankImage.gameObject.SetActive(true);
                rankText.gameObject.SetActive(false);
                // スプライト表示
                rankImage.sprite = rankSprites[rank-1];
            }
            else
            {
                // 表示切り替え
                rankImage.gameObject.SetActive(false);
                rankText.gameObject.SetActive(true);
                // ランク表示
                rankText.text = rank.ToString();
            }
        }
        
        /// <summary>エンブレム設定</summary>
        public void SetEmblem(long id)
        {
            icon.SetTexture(id);
        }
    }
}