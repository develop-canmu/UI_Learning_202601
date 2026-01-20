using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pjfb.Networking.App.Request;

namespace Pjfb.Club
{
    public class MemberTopClubMember : MonoBehaviour {
        [SerializeField]
        ClubMemberInfoView _view = null;

        public void UpdateView( ClubMemberData status ){
            var param = new ClubMemberInfoView.Param(status, true, true, ClubUtility.ChangeFindClubPage, ClubUtility.ChangeFindClubPage);
            _view.UpdateView( param );
        }
        
    }
}