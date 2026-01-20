using System;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb.Club
{
    public class ClubInfoModalMemberListItem : ClubMemberInfo {
        public class Param{
            public ClubData clubData{get;set;} = null;
            public ClubMemberData member{get;set;} = null;
            public ClubAccessLevel myAccessLevel{get;set;} = ClubAccessLevel.None;
            public Func<ClubAccessLevel, string, UniTask> updateViewRequest{get;set;} = null;
            public Action closeModalRequest{get;set;} = null;
            public long myUserId{get;set;} = 0;
            public bool showUserProfileOtherButtons {get;set;} = true; 
            public bool showClubInfoHeaderButtons {get;set;} = true; 
            public Action onFinishedDissolution {get;set;} = null;
            public Action onFinishedSecession {get;set;} = null;
        }


        protected override bool showUserProfileOtherButtons => _param.showUserProfileOtherButtons;
        protected override bool showClubInfoHeaderButtons => _param.showClubInfoHeaderButtons;
        protected override Action onFinishedDissolution => _param.onFinishedDissolution;
        protected override Action onFinishedSecession => _param.onFinishedSecession;

        [SerializeField]
        UIButton _manageButton = null;

        Param _param = null;

        protected override void OnSetView(object value){
            _param = (Param)value;
            _manageButton?.gameObject.SetActive( IsViewManagerButton() );
            base.OnSetView( _param.member );
        }
        

        public void OnClickManageButton(){
            var param = new ClubMemberPostManageModal.Param();
            param.clubData = _param.clubData;
            param.member = _param.member;
            param.myAccessLevel = _param.myAccessLevel;
            param.myUserID = _param.myUserId;
            param.updateViewRequest = _param.updateViewRequest;
            param.closeModalRequest = _param.closeModalRequest;
            
            
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubMemberPostManage, param);
        }

        bool IsViewManagerButton(){
            if( _param.myAccessLevel == ClubAccessLevel.None ) {
                return false;
            }

            if( _param.myAccessLevel == ClubAccessLevel.SubMaster ) {
                var accessLevel = ClubUtility.CreateAccessLevel( _param.member.roleId );
                if ( accessLevel == ClubAccessLevel.None ){
                    return true ;
                }

                if ( accessLevel == ClubAccessLevel.SubMaster ){
                    return _param.member.userId == _param.myUserId;
                }
                return false;
            }

            if( _param.myAccessLevel == ClubAccessLevel.Master ) {
                return _param.member.userId != _param.myUserId;
            }

            return false;
        }

    }
}