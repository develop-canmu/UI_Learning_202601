using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pjfb.Networking.App.Request;

namespace Pjfb.Club
{
    public class FindMemberListItem : MemberListItem.OverrideHandler {
        
        public class Param : MemberListItem.Param{
            public System.Action<ClubUserData> onInvitationUser{ get;set; } = null;
            public ClubAccessLevel myAccessLevel = ClubAccessLevel.None;

            public Param( ClubUserData data, ClubAccessLevel myAccessLevel, System.Action<ClubUserData> onInvitationUser ) : base(data) {
                this.onInvitationUser = onInvitationUser;
                this.myAccessLevel = myAccessLevel;
            }   
        } 

        [SerializeField]
        UIButton _invitationButton = null;

        Param _param = null;

        public override void UpdateView( MemberListItem.Param param ){
            
            _param = param as  Param;   
            _invitationButton.gameObject.SetActive( _param.myAccessLevel == ClubAccessLevel.Master || _param.myAccessLevel == ClubAccessLevel.SubMaster );
        }

        public void OnClickInvitationButton(  ){
            _param.onInvitationUser?.Invoke( _param.userData );
        }

        
        
    }
}