using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using TMPro;

namespace Pjfb.Club
{
    public class ClubMemberInfo : ScrollGridItem {

        [SerializeField]
        protected ClubMemberInfoView _view = null;

        protected virtual bool showUserProfileOtherButtons => true;
        protected virtual bool showClubInfoHeaderButtons => true;
        protected virtual Action onFinishedDissolution => null;
        protected virtual Action onFinishedSecession => null;
        
        protected override void OnSetView(object value){
            var param = new ClubMemberInfoView.Param((ClubUserData)value, showUserProfileOtherButtons, showClubInfoHeaderButtons, onFinishedDissolution, onFinishedSecession);
            _view.UpdateView( param );   
        }
    }
}