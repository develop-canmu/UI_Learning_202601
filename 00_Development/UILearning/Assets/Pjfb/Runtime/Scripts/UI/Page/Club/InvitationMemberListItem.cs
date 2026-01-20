using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;

namespace Pjfb.Club
{
    public class InvitationMemberListItem : ClubMemberInfo {
        public class Param {
            public ClubInvitationMemberData userData{ get;set; } = null;
            public System.Action<ClubInvitationMemberData> onInvitationUser{ get;set; } = null;
        }

        Param _param = null;
        protected override void OnSetView(object value){
            _param = (Param)value;
            base.OnSetView(_param.userData);
        }

        public void OnClickMember(){
            _param.onInvitationUser?.Invoke( _param.userData );
        }   
    }
}