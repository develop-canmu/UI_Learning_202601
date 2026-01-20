using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Club;
using Pjfb.Extensions;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.ClubRoyal
{
    public class ClubRoyalClubInfoView : MonoBehaviour
    {
        [SerializeField]
        private Image rankImage = null;
        [SerializeField]
        private Sprite[] rankSprites = null;
        [SerializeField]
        private TextMeshProUGUI rankText = null;
        [SerializeField]
        private Image emblemImage = null;
        [SerializeField]
        private TextMeshProUGUI nameText = null;
        
        /// <summary>
        /// 順位をセットする
        /// </summary>
        /// <param name="rank">順位</param>
        public void SetRank(long rank)
        {
            if (rankImage != null)
            {
                if (rank <= 0)
                {
                    rankText.gameObject.SetActive(true);
                    rankImage.gameObject.SetActive(false);
                    rankText.text = StringValueAssetLoader.Instance["ranking.rank_blank"];
                    return;
                }

                if (rank <= rankSprites.Length)
                {
                    rankImage.gameObject.SetActive(true);
                    rankImage.sprite = rankSprites[rank - 1];
                    rankText.gameObject.SetActive(false);
                }
                else
                {
                    rankText.gameObject.SetActive(true);
                    rankImage.gameObject.SetActive(false);
                }
            }
            rankText.text = rank.GetStringNumberWithComma();
        }
        
        /// <summary>
        /// エンブレムをセットする
        /// </summary>
        /// <param name="emblemId">エンブレムのID</param>
        public void SetEmblem(long emblemId)
        {
            // クラブ未所属の場合は設定を行わない
            if (emblemId == 0)
            {
                emblemImage.gameObject.SetActive(false);
                return;
            }
            emblemImage.gameObject.SetActive(true);
            ClubUtility.LoadAndSetEmblemIconSync(emblemImage, emblemId);
        }
        
        /// <summary>
        /// 名前をセットする
        /// </summary>
        /// <param name="userName">ユーザー名</param>
        public void SetName(string name)
        {
            nameText.text = name;
        }
    }
}