using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using TMPro;
using CruFramework.UI;

namespace Pjfb.Club {
    public class ClubInfoModalMemberSheet : ClubInfoModalSheet {
        [SerializeField]
        ScrollGrid _scroll = null;
        [SerializeField]
        TextMeshProUGUI _member = null;

        [SerializeField]
        UIButton _dissolutionButton = null;
        [SerializeField]
        UIButton _secessionButton = null;
        [SerializeField]
        UIButton _requestButton = null;
        [SerializeField]
        ScrollGridItem _manageMemberListItem = null;
        [SerializeField]
        ScrollGridItem _memberListItem = null;


        ClubAccessLevel _myAccessLevel = ClubAccessLevel.None;
        long _myUserId = 0;
        Func<ClubAccessLevel, UniTask> _updateViewRequest = null;
        ClubInfoModal _modal = null;

        protected override UniTask InitView( ClubInfoModalSheetParam param, ClubAccessLevel myAccessLevel, long myUserId, ClubInfoModal modal, Func<ClubAccessLevel,UniTask> updateViewRequest ){
            _param = param;
            _myUserId = myUserId;
            _updateViewRequest = updateViewRequest;
            _modal = modal;
            UpdateView(myAccessLevel);
            return default;
        }

        public void UpdateView( ClubAccessLevel accessLevel ){
            _myAccessLevel = accessLevel;
            var paramList = new List<ClubInfoModalMemberListItem.Param>();
            var sortMembers = ClubUtility.CreateSortList(_param.data.memberList);
            //リスト用param作成
            foreach( var member in sortMembers ){
                var param = new ClubInfoModalMemberListItem.Param();
                param.clubData = _param.data;
                param.member = member;
                param.myAccessLevel = _myAccessLevel;
                param.myUserId = _myUserId;
                param.showUserProfileOtherButtons = _param.showUserProfileOtherButtons;
                param.showClubInfoHeaderButtons = _param.showHeaderButtons;
                param.onFinishedDissolution = _param.onFinishedDissolution;
                param.onFinishedSecession = _param.onFinishedSecession;
                
                param.updateViewRequest = async (updateAccessLevel, notificationText ) =>{
                    UpdateView(updateAccessLevel);
                    if( _updateViewRequest != null ) {
                        await _updateViewRequest( updateAccessLevel );
                    }
                    _modal.RemoveTopModals();
                    if( !string.IsNullOrEmpty(notificationText) ) {
                        _modal.notification.ShowNotification(notificationText);
                    }
                };

                param.closeModalRequest = () =>{
                    _modal.RemoveTopModals();
                };

                
                paramList.Add(param);
            }
            _scroll.ItemPrefab = _myAccessLevel == ClubAccessLevel.None ? _memberListItem : _manageMemberListItem;
            _scroll.SetItems( paramList );


            _member.text = ClubUtility.CreateMemberQuantityString( _param.data.memberList.Count );
            
            // ボタンを表示できるならクラブの加入状況に応じてボタンを表示する
            if (_param.showHeaderButtons)
            {
                if (_myAccessLevel == ClubAccessLevel.NotAffiliation)
                {
                    _dissolutionButton.gameObject.SetActive(false);
                    _secessionButton.gameObject.SetActive(false);
                    // ギルドに所属していないかつメンバー募集中ならアクティブに
                    SetRequestButtonState(UserData.UserDataManager.Instance.user.gMasterId == 0 &&
                                          ClubUtility.IsMembersWantedFlg(_param.data.recruitmentStatus, _param.data.memberList.Count));
                }
                else
                {
                    _dissolutionButton.gameObject.SetActive(_myAccessLevel == ClubAccessLevel.Master);
                    _secessionButton.gameObject.SetActive(_myAccessLevel != ClubAccessLevel.Master);
                    _requestButton.gameObject.SetActive(false);
                }
            }
            // 表示できないならボタンは全てオフ
            else
            {
                _dissolutionButton.gameObject.SetActive(false);
                _secessionButton.gameObject.SetActive(false);
                _requestButton.gameObject.SetActive(false);
            }
        }

        public void SetRequestButtonState(bool isActive)
        {
            _requestButton.interactable = isActive;
        }
    }
}