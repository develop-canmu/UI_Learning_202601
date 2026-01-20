using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.UserData;

namespace Pjfb.Event
{
    public enum EventPageType
    {
        //Todo:対応時に追加
        EventTop
    }
    
    public class EventPage : PageManager<EventPageType>
    {
        public static HashSet<long> MCharaPossessionHashSet;

        private long mFestivalTimetableId;
        public long MFestivalTimetableId
        {
            get => mFestivalTimetableId;
        }
        
        protected override string GetAddress(EventPageType page)
        {
            return $"Prefabs/UI/Page/Event/{page}Page.prefab";
        }

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            mFestivalTimetableId = (long)args;
            MCharaPossessionHashSet =  UserDataManager.Instance.chara.data.Values.Select(x => (long)x.charaId).ToHashSet();
            await OpenPageAsync(EventPageType.EventTop, true, new EventTopPage.Data(mFestivalTimetableId), token);
        }
    }
}