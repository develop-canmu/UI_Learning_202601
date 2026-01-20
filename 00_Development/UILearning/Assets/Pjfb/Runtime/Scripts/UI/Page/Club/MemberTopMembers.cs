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
    public class MemberTopMembers : MonoBehaviour {
        [SerializeField]
        MemberTopClubMember _memberPrefab = null;
        [SerializeField]
        Transform _memberContent = null;

        List<MemberTopClubMember> _members = new List<MemberTopClubMember>();

        public void UpdateView( List<ClubMemberData> guildMemberList ){
            ClearList();
            var members = ClubUtility.CreateSortList(guildMemberList);
            foreach( var status in members ){
                var item = Instantiate<MemberTopClubMember>(_memberPrefab, _memberContent);
                item.UpdateView(status);
                _members.Add(item);
            }
        } 

        void ClearList(){
            foreach( var member in _members ){
                Destroy(member.gameObject);
            }
            _members.Clear();
        }

        
    }
}
