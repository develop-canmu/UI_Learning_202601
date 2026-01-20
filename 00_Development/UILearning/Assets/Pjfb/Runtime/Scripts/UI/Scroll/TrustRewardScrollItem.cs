using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using TMPro;

namespace Pjfb.Encyclopedia
{
    public class TrustRewardScrollData
    {
        public TrustRewardScrollData(long level, PrizeJsonWrap prize, bool hasReceived)
        {
            Level = level;
            Prize = prize;
            HasReceived = hasReceived;
        }
        public readonly long Level;
        public readonly PrizeJsonWrap Prize;
        public readonly bool HasReceived;
    }
    
    public class TrustRewardScrollItem : ScrollGridItem
    {
        
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private PrizeJsonView prizeJsonView;
        [SerializeField] private GameObject receivedCover;

        protected override void OnSetView(object value)
        {
            var data = (TrustRewardScrollData)value;
            var prizeContainerData = PrizeJsonUtility.GetPrizeContainerData(data.Prize);

            prizeJsonView.SetView(data.Prize);
            titleText.text = string.Format(StringValueAssetLoader.Instance["character.trust_lv_reward"], data.Level);
            itemName.text = prizeContainerData?.name ?? "";
            receivedCover.SetActive(data.HasReceived);
        }
    }
}