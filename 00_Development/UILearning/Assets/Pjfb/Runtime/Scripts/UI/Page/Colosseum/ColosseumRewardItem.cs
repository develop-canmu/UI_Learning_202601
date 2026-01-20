using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pjfb.UI;
using Cysharp.Threading.Tasks;
using CruFramework.UI;
using Pjfb.Master;

namespace Pjfb.Colosseum
{
    
    public class RewardScrollData
    {
        public long rankTop;
        public long rankBottom;
        public bool myRank;
        public PrizeJsonWrap[] prizeList;
    }
    
    public class ColosseumRewardItem : ScrollGridItem
    {
        [SerializeField] private TMP_Text rankingText;
        [SerializeField] private TMP_Text rankingTopText;
        [SerializeField] private TMP_Text rankingBottomText;
        [SerializeField] private GameObject textRankingRoot;
        [SerializeField] private Image rankingImage;
        [SerializeField] private Image rankBaseImage;
        [SerializeField] private ScrollGrid rewardScroller;
        [SerializeField] private Sprite[] rankingIcon;
        
        protected override void OnSetView(object value)
        {
            var data = (RewardScrollData)value;
            SetRewardRankingUi(data.rankTop, data.rankBottom, data.myRank);
            SetRewardUi(data.prizeList);
        }

        private void SetRewardRankingUi(long rankTop, long rankBottom, bool myRank)
        {
            var colorKey = myRank ? "pvp.my.rank" : "pvp.rank";
            rankBaseImage.color = ColorValueAssetLoader.Instance[colorKey];
            
            if (rankTop == rankBottom)
            {
                var isTop = rankTop <= 3;
                textRankingRoot.SetActive(!isTop);
                rankingImage.gameObject.SetActive(isTop);
                if (isTop)
                {
                    rankingImage.sprite = rankingIcon[rankTop - 1];
                }
                else
                {
                    rankingTopText.gameObject.SetActive(true);
                    rankingText.gameObject.SetActive(false);
                    rankingBottomText.gameObject.SetActive(false);
                    rankingTopText.text = rankTop.ToString();   
                }
            }
            else
            {
                textRankingRoot.gameObject.SetActive(true);
                rankingImage.gameObject.SetActive(false);
                rankingTopText.gameObject.SetActive(true);
                rankingText.gameObject.SetActive(true);
                rankingBottomText.gameObject.SetActive(true);
                rankingTopText.text = rankTop.ToString();
                rankingBottomText.text = rankBottom.ToString();
            }
        }
        
        private void SetRewardUi(PrizeJsonWrap[] prizeJson)
        {
            var rewardList = new List<PrizeJsonGridItem.Data>();
            foreach (var prize in prizeJson)
            {
                var data = new PrizeJsonGridItem.Data(
                    prize);
                rewardList.Add(data);
            }
            rewardScroller.SetItems(rewardList);
        }

    }
   
}