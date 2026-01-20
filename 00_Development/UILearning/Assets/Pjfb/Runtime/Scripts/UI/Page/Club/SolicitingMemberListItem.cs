using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pjfb.Networking.App.Request;

namespace Pjfb.Club
{
    public class SolicitingMemberListItem : MemberListItem.OverrideHandler {
        
        public class Param : MemberListItem.Param{
            public System.Action<ClubUserData> onCancelInvitationUser{ get;set; } = null;
            public ClubAccessLevel myAccessLevel = ClubAccessLevel.None;
            public bool isBadgeView = false;
            public Param( bool isBadgeView, ClubUserData data, ClubAccessLevel myAccessLevel, System.Action<ClubUserData> onCancelInvitationUser ) : base(data) {
                this.isBadgeView = isBadgeView;
                this.onCancelInvitationUser = onCancelInvitationUser;
                this.myAccessLevel = myAccessLevel;
            }   
        } 

        [SerializeField]
        UIButton _cancelButton = null;
        [SerializeField]
        UIBadgeNotification _badge = null;

        Param _param = null;

        public override void UpdateView( MemberListItem.Param param ){
            _param = param as  Param;   
            _badge.SetActive(_param.isBadgeView);
            _cancelButton.gameObject.SetActive( _param.myAccessLevel == ClubAccessLevel.Master || _param.myAccessLevel == ClubAccessLevel.SubMaster );
        }

        public void OnClickCancelButton(  ){
            _param.onCancelInvitationUser?.Invoke( _param.userData );
        }

        
        
    }
}