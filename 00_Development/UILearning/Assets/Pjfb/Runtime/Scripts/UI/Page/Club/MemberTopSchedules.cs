using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Page;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using CruFramework;
using CruFramework.UI;
using TMPro;


namespace Pjfb.Club
{
    public class MemberTopSchedules : MonoBehaviour {
        [SerializeField]
        MemberTopMatchSchedule _schedulePrefab = null;
        [SerializeField]
        Transform _listContent = null;
    
        List<MemberTopMatchSchedule> _schedules = new List<MemberTopMatchSchedule>();

        public void UpdateView( GuildBattleMatchingMatchingStatus[] matchingStatuses ){
            ClearList();
            foreach( var status in matchingStatuses ){
                var item = Instantiate<MemberTopMatchSchedule>(_schedulePrefab, _listContent);
                item.UpdateView(status);
                _schedules.Add(item);
            }
        } 

        void ClearList(){
            foreach( var schedule in _schedules ){
                Destroy(schedule.gameObject);
            }
            _schedules.Clear();
        }
    }
}
