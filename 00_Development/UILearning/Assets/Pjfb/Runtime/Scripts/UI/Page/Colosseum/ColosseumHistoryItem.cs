using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.UI;
using TMPro;
using Pjfb.Networking.App.Request;
using Pjfb.Extensions;
using Pjfb.Master;

namespace Pjfb.Colosseum
{
    
    public class ColosseumHistoryItem : ScrollGridItem
    {
        public class Data
        {
            public ColosseumUserSeasonStatus seasonStatus;
            public long mColosseumGradeGroupId;
        }
        
        [SerializeField] private ColosseumRoomImage roomImage;
        [SerializeField] private ColosseumRankImage rankImage;
        [SerializeField] private ColosseumRoomImage beforeRoomImage;
        [SerializeField] private ColosseumRoomImage afterRoomImage;
        [SerializeField] private TMP_Text expireText;
        [SerializeField] private TMP_Text rankText;
        [SerializeField] private Image changeArrowImage;
        [SerializeField] private TMP_Text changeArrowText;
        
        protected override void OnSetView(object value)
        {
            var data = (Data)value;
            var seasonStatus = data.seasonStatus;
            
            roomImage.SetTexture(seasonStatus.gradeNumber);
            
            var rankNumber = MasterManager.Instance.colosseumGradeRankLabelMaster.GetRankNumber(
                data.mColosseumGradeGroupId, seasonStatus.gradeNumber, seasonStatus.ranking);
            
            rankImage.SetTexture(rankNumber);
            
            var startAt = seasonStatus.startAt.TryConvertToDateTime().GetDateTimeString();
            var endAt = seasonStatus.endAt.TryConvertToDateTime().GetDateTimeString(); 
            expireText.text = string.Format(StringValueAssetLoader.Instance["pvp.period.history"],startAt,endAt);
            
            beforeRoomImage.SetTexture(seasonStatus.gradeNumber);
            afterRoomImage.SetTexture(seasonStatus.gradeAfter);

            rankText.text = string.Format(StringValueAssetLoader.Instance["pvp.rank.value"], seasonStatus.ranking);

            var arrowColorKey = "";
            var arrowTextKey = "";
            if (seasonStatus.gradeNumber == seasonStatus.gradeAfter)
            {
                arrowColorKey = "pvp.rank.stay";
                arrowTextKey = "pvp.stay";
            }
            else if (seasonStatus.gradeNumber > seasonStatus.gradeAfter)
            {
                arrowColorKey = "pvp.rank.down";
                arrowTextKey = "pvp.relegation";
            }
            else
            {
                arrowColorKey = "pvp.rank.up";
                arrowTextKey = "pvp.promotion";
            }

            changeArrowImage.color = ColorValueAssetLoader.Instance[arrowColorKey];
            changeArrowText.text = StringValueAssetLoader.Instance[arrowTextKey];

        }
    }
}