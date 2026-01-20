using System.Collections;
using System.Collections.Generic;
using CruFramework.Page;
using UnityEngine;

using CruFramework.UI;
using UnityEngine.UI;
using TMPro;
using System;
using Pjfb.UI;
using Pjfb.Extensions;

namespace Pjfb.Rivalry
{

    public class RivalryMatchBanner : ListItemBase
    {
        #region Params
        public class Data : ItemParamsBase
        {
            public RivalryMatchType matchType;
            public int mHuntId;
            public int mHuntTimetableId;
            public DateTime startDate;
            public DateTime endDate;
            public bool showBadge;
            public bool hasEventEnded;
        }
        #endregion

        #region SerializeFields
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private Image bannerImage;
        [SerializeField] private TMP_Text endDateText;
        [SerializeField] private GameObject endDateRoot;
        [SerializeField] private GameObject badgeItem;
        [SerializeField] private GameObject coverRoot;
        #endregion

        public Data data;

        public override void Init(ItemParamsBase value)
        {
            data = (Data) value;
            badgeItem?.SetActive(data.showBadge);
            coverRoot?.SetActive(data.hasEventEnded && data.matchType == RivalryMatchType.Event);
            endDateRoot?.SetActive(data.matchType == RivalryMatchType.Event);
            string endDateString = string.Empty;
            if (!data.hasEventEnded)
            {
                endDateString = string.Format(StringValueAssetLoader.Instance["rivalry.top.event_period"], 
                    DateTimeExtensions.GetNewsDateTimeString(data.startDate),
                    DateTimeExtensions.GetNewsDateTimeString(data.endDate));
            }
            else 
            {
                endDateString = StringValueAssetLoader.Instance["rivalry.top.event_end_period"];
            }
            endDateText.text = endDateString;
            titleText.text = data.matchType == RivalryMatchType.Event ?
                StringValueAssetLoader.Instance["rivalry.top.event"] :
                StringValueAssetLoader.Instance["rivalry.top.regular"];
            // ToDo: 画像ロード
        }
    }
}