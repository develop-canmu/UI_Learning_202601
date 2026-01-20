using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pjfb.Networking.App.Request;

namespace Pjfb.Club
{
    public class RequestJoinMemberListItem : MemberListItem.OverrideHandler {
        
        public class Param : MemberListItem.Param{
            public bool isViewBadge{get;set;} = false;
            public ClubRequestUserData requestUserData{ get;set; } = null;
            public ClubAccessLevel myAccessLevel = ClubAccessLevel.None;
            public System.Action<ClubRequestUserData> onClickAgreeButton{ get;set; } = null;
            public System.Action<ClubRequestUserData> onClickDisagreeButton{ get;set; } = null;


            public Param( bool isViewBadge, ClubRequestUserData data, ClubAccessLevel myAccessLevel, System.Action<ClubRequestUserData> onClickAgreeButton, System.Action<ClubRequestUserData> onClickDisagreeButton ) : base(data) {
                this.isViewBadge = isViewBadge;
                requestUserData = data;
                this.myAccessLevel = myAccessLevel;
                this.onClickAgreeButton = onClickAgreeButton;
                this.onClickDisagreeButton = onClickDisagreeButton;
            }   
        } 

        [SerializeField]
        UIBadgeNotification _badge = null;

        [SerializeField]
        UIButton _agreeButton = null;

        [SerializeField]
        UIButton _disagreeButton = null;

        Param _param = null;

        public override void UpdateView( MemberListItem.Param param ){
            _param = param as Param; 
            _badge.SetActive(_param.isViewBadge);  
            _agreeButton.gameObject.SetActive( _param.myAccessLevel == ClubAccessLevel.Master || _param.myAccessLevel == ClubAccessLevel.SubMaster );
            _disagreeButton.gameObject.SetActive( _param.myAccessLevel == ClubAccessLevel.Master || _param.myAccessLevel == ClubAccessLevel.SubMaster );
        }

        public void OnClickAgreeButton(){
            _param.onClickAgreeButton?.Invoke( _param.requestUserData );
        }

        public void OnClickDisagreeButton(){
            _param.onClickDisagreeButton?.Invoke( _param.requestUserData );
        }
        
        
    }
}