using System.Collections.Generic;
using System.Linq;
using CruFramework.Utils;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using UnityEngine;
using Logger = CruFramework.Logger;

namespace Pjfb.Event
{
    public class EventManager :Singleton<EventManager>
    {
        #region PublicProperties
        public List<FestivalEffectStatus> FestivalEffectStatusList { get; private set; } = new();
        public bool showHomeBadge = false;
        #endregion

        #region PublicMethods
        public void SetFestivalEffectStatus(FestivalEffectStatus[] festivalEffectStatusList)
        {
            FestivalEffectStatusList = festivalEffectStatusList?.ToList() ?? new List<FestivalEffectStatus>();
        }

        public void UpdateFestivalEffectStatus(FestivalEffectStatus festivalEffectStatus)
        {
            var mFestivalTimetableId = festivalEffectStatus.mFestivalTimetableId;
            var index = FestivalEffectStatusList.FindIndex(aData => aData.mFestivalTimetableId == mFestivalTimetableId);
            if (index >= 0) FestivalEffectStatusList[index] = festivalEffectStatus;
            else FestivalEffectStatusList.Add(festivalEffectStatus);
        }
        #endregion
        
        #region StaticMethod
        /// <summary>
        /// ホーム画面にあるイベントバナーがクリックされた時に実行される
        /// </summary>
        public static void OnClickEventBannerButton(long mFestivalTimetableId)
        {
            Logger.Log("EventManager.OnClickEventBannerButton");
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Event, true, mFestivalTimetableId);
        }

        public static IEnumerable<FestivalTimetableMasterObject> GetShowingFestivalTimetableList(FestivalTimetableMasterContainer festivalTimetableMaster, Dictionary<long, HuntTimetableMasterObject> mHuntTimetableDictionary)
        {
            var now = AppTime.Now;
            var slideEventViewParams = festivalTimetableMaster.values
                .Where(aData =>
                    aData.startAt.TryConvertToDateTime(minValueDefault: false).IsPast(now) &&
                    aData.endAt.TryConvertToDateTime().IsFuture(now) &&
                    mHuntTimetableDictionary.TryGetValue(aData.mHuntTimetableId, out var huntTimetable) &&
                    huntTimetable.startAt.TryConvertToDateTime(minValueDefault: false).IsPast(now) &&
                    huntTimetable.endAt.TryConvertToDateTime().IsFuture(now) );
            return slideEventViewParams;
        }
        #endregion
    }
}