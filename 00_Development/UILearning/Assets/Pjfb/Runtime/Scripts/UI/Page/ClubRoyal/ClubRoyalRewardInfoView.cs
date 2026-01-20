using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Extensions;
using Pjfb.Master;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.ClubRoyal
{
    public class ClubRoyalRewardInfoView : MonoBehaviour
    {
        [SerializeField]
        private GameObject frameRankIn = null;
        [SerializeField]
        private Image rankImage = null;
        [SerializeField]
        private Sprite[] rankSprites = null;
        [SerializeField]
        private TextMeshProUGUI rankTopText = null;
        [SerializeField]
        private TextMeshProUGUI rankMiddleText = null;
        [SerializeField]
        private TextMeshProUGUI rankBottomText = null;
        [SerializeField]
        private ScrollGrid prizeGrid = null;
        
        /// <summary>
        /// 順位をセットする
        /// </summary>
        public void SetRank(long upperRanking, long lowerRanking, bool isCurrent)
        {
            if (upperRanking == lowerRanking)
            {
                if(upperRanking <= rankSprites.Length)
                {
                    rankImage.gameObject.SetActive(true);
                    rankImage.sprite = rankSprites[upperRanking - 1];
                    rankTopText.gameObject.SetActive(false);
                }
                else
                {
                    rankTopText.gameObject.SetActive(true);
                    rankTopText.text = upperRanking.GetStringNumberWithComma();
                    rankImage.gameObject.SetActive(false);
                }
                rankMiddleText.gameObject.SetActive(false);
                rankBottomText.gameObject.SetActive(false);
            }
            else
            {
                rankTopText.gameObject.SetActive(true);
                rankTopText.text = upperRanking.GetStringNumberWithComma();
                rankMiddleText.gameObject.SetActive(true);
                rankBottomText.gameObject.SetActive(true);
                rankBottomText.text = lowerRanking.GetStringNumberWithComma();
                rankImage.gameObject.SetActive(false);
            }
            frameRankIn.SetActive(isCurrent);
        }
        
        /// <summary>
        /// 報酬をセットする
        /// </summary>
        /// <param name="prizeJsonWrap">報酬データ</param>
        public void SetRewardPrize(PrizeJsonWrap[] prizeJsonWrap)
        {
            List<PrizeJsonGridItem.Data> dataList = new List<PrizeJsonGridItem.Data>();
            foreach (PrizeJsonWrap prize in prizeJsonWrap)
            {
                dataList.Add(new PrizeJsonGridItem.Data(prize));
            }
            prizeGrid.SetItems(dataList);
        }
    }
}