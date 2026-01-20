using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using TMPro;

namespace Pjfb.Event
{
    public class EventRewardScrollData
    {
        public EventRewardScrollData(DailyMissionMasterObject mDailyMission, bool hasReceived, float sortOrder)
        {
            MDailyMission = mDailyMission;
            HasReceived = hasReceived;
            SortOrder = sortOrder;
        }
        public readonly DailyMissionMasterObject MDailyMission;
        public readonly bool HasReceived;
        public readonly float SortOrder;
    }
    
    
    
    public class EventRewardScrollItem : ScrollGridItem
    {
        [SerializeField] private TextMeshProUGUI prizeNameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private PrizeJsonView prizeJsonView;
        [SerializeField] private GameObject receivedCover;

        protected override void OnSetView(object value)
        {
            var data = (EventRewardScrollData)value;
            var prizeContainerData = PrizeJsonUtility.GetPrizeContainerData(data.MDailyMission.prizeJson[0]);

            prizeJsonView.SetView(prizeContainerData.prizeJsonWrap);
            prizeNameText.text = prizeContainerData.name;
            descriptionText.text = data.MDailyMission.description;
            receivedCover.SetActive(data.HasReceived);
        }
    }
}