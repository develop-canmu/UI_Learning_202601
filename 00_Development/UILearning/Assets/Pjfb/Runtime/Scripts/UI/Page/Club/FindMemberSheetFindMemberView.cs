using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.Page;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Cysharp.Threading.Tasks;
using CruFramework.UI;
using TMPro;


namespace Pjfb.Club {
    public class FindMemberSheetFindMemberView : FindMemberSheetView {

        [SerializeField]
        ClubInputField _findName = null;
        
        [SerializeField]
        TextMeshProUGUI _findSettingPower = null;
        [SerializeField]
        OmissionTextSetter _findSettingPowerOmissionTextSetter = null;
        [SerializeField]
        TextMeshProUGUI _findSettingRank = null;
        [SerializeField]
        TextMeshProUGUI _findSettingActivityPolicy = null;
        [SerializeField]
        TextMeshProUGUI _findSettingClubMatchPolicy = null;
        [SerializeField]
        TextMeshProUGUI _findSettingParticipationPriority = null;
        [SerializeField]
        ScrollGrid _memberScroll = null;
        [SerializeField]
        UIButton _allSolicitationButton = null;
        [SerializeField]
        TextMeshProUGUI _noneText = null;
        

        ClubFindMemberSettingModal.EditData _editData = null;

        List<FindMemberListItem.Param> _findMemberParams = new List<FindMemberListItem.Param>();

        protected override async UniTask OnPreOpen(object args) {
            UpdateEditData(new ClubFindMemberSettingModal.EditData());
            _allSolicitationButton.interactable = _findMemberParams.Count > 0;
            _noneText.gameObject.SetActive( _findMemberParams.Count <= 0 );
            await base.OnPreOpen(args);
        }

        public void OnClickAllSolicitationButton(){
            ConfirmModalData data = new ConfirmModalData(
                StringValueAssetLoader.Instance["club.invitation"],
                StringValueAssetLoader.Instance["club.allInvitationText"],
                null,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["club.doInvitation"], (window) => { 
                    window.Close();
                    AllSolicitation(); 
                }),
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.cancel"], window => window.Close())
            );
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
        }

        public void OnClickFindMemberButton()
        {
            UpdateFindMember().Forget();
        }


        public void OnClickFindMemberSettingButton() {
            var param = new ClubFindMemberSettingModal.Param();
            param.editData = _editData;
            param.onClickButton = UpdateEditData;
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubFindMemberSetting, param);
        }

        /// <summary>
        /// 検索メンバー更新
        /// </summary>
        /// <returns></returns>
        public async UniTask UpdateFindMember()
        {
            var request = new GuildSearchInvitationUserAPIRequest();
            var post = new GuildSearchInvitationUserAPIPost();
            post.name = _findName.text;
            post.combatPowerFrom = _editData.powerFrom.ToString();
            post.combatPowerTo = _editData.powerTo.ToString();
            post.guildRank = _editData.rank;
            post.playStyleType = _editData.activityPolicy;
            post.guildBattleType = _editData.clubMatchPolicy;
            post.participationPriorityType = _editData.participationPriority;
            request.SetPostData(post);
            
            await APIManager.Instance.Connect( request );
            var response = request.GetResponseData();
            
            _findMemberParams.Clear();
            
            foreach( var user in response.userList ) {
                var myAccessLevel = _myAccessLevel;
                var param = new FindMemberListItem.Param(new ClubInvitationMemberData(user), myAccessLevel, OnInvitationUser);
                _findMemberParams.Add(param);
            }
            _memberScroll.SetItems(_findMemberParams);
            _allSolicitationButton.interactable = _findMemberParams.Count > 0;
            _noneText.gameObject.SetActive( _findMemberParams.Count <= 0 );
        }


        /// <summary>
        /// 検索編集表示更新
        /// </summary>
        /// <param name="editData"></param>
        void UpdateEditData( ClubFindMemberSettingModal.EditData editData ) {
            _editData = editData;
            var powerFrom = StringValueAssetLoader.Instance["club.noneLowerLimit"]; 
            var powerTo = StringValueAssetLoader.Instance["club.noneUpperLimit"]; 
            var powerFormat = StringValueAssetLoader.Instance["club.findMemberPowerFormat"]; 
            if( _editData.powerFrom > 0 ){
                powerFrom = _editData.powerFrom.ToDisplayString(_findSettingPowerOmissionTextSetter.GetOmissionData());
            }
            if( _editData.powerTo > 0 ){
                powerTo = _editData.powerTo.ToDisplayString(_findSettingPowerOmissionTextSetter.GetOmissionData());
            }
            
            var powerText = string.Format(powerFormat, powerFrom, powerTo);
            _findSettingPower.text = powerText;
            var format = StringValueAssetLoader.Instance["club.find_setting_upper_rank"];
            _findSettingRank.text = StringValueAssetLoader.Instance["club.noneConditions"];
            foreach( var master in Master.MasterManager.Instance.guildRankMaster.values ){
                if( master.id == _editData.rank ) {
                    _findSettingRank.text = string.Format(format, master.name);
                    break;
                }
            }
            
            _findSettingActivityPolicy.text = ClubUtility.FindActivityPolicyStrings(editData.activityPolicy);

            var clubMatchPolicyStrings = ClubUtility.CreateMatchPolicyStrings();
            _findSettingClubMatchPolicy.text = clubMatchPolicyStrings[editData.clubMatchPolicy];

            _findSettingParticipationPriority.text = ClubUtility.GetParticipationPriorityData(editData.participationPriority).name;
        }

        /// <summary>
        /// 勧誘する
        /// </summary>
        /// <param name="user"></param>
        public void OnInvitationUser( ClubUserData user ){
            var param = new ClubInvitationUserModal.Param();
            param.user = user;
            param.onInvitationUser = OnFinishedInvitationUser;
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubInvitationUser, param);
        }   
 
    
        /// <summary>
        /// 一括勧誘
        /// </summary>
        public void AllSolicitation(){
            ConnectAllInvitationAPI(_findMemberParams).Forget();
        }

        public async UniTask ConnectAllInvitationAPI( List<FindMemberListItem.Param> users ){
            if( users.Count <= 0 ) {
                return;
            }

            var request = new GuildSendInvitationAPIRequest();
            var post = new GuildSendInvitationAPIPost();
            var idListStr = string.Empty;
            for( int i=0; i<users.Count; ++i ){
                if( i != 0 ) {
                    //複数ユーザーがいる場合はカンマ区切り
                    idListStr += ",";
                }
                idListStr += users[i].userData.userId.ToString();
            }
            post.targetUMasterIdListStr = idListStr;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
        
            //表示更新
            _findMemberParams.Clear();
            _memberScroll.SetItems(_findMemberParams);
            _allSolicitationButton.interactable = _findMemberParams.Count > 0;
            _noneText.gameObject.SetActive( _findMemberParams.Count <= 0 );

            _page.notification.ShowNotification(StringValueAssetLoader.Instance["club.allInvitationNotificationText"]);
            
        }   

        /// <summary>
        /// 勧誘が完了した
        /// </summary>
        /// <param name="user"></param>
        void OnFinishedInvitationUser( ClubUserData user ){
            RemoveUserFromList(user);
            LocalSaveManager.saveData.clubCheckNotificationData.sendSolicitation.Add(user.userId);
            LocalSaveManager.Instance.SaveData();
            var text = string.Format( StringValueAssetLoader.Instance["club.invitationNotificationText"], user.name);
            _page.notification.ShowNotification(text);
        }  

        /// <summary>
        /// ユーザーを招待した
        /// </summary>
        /// <param name="data"></param>
        void RemoveUserFromList( ClubUserData data ) {
            //検索一覧から削除
            FindMemberListItem.Param removeParam = null;
            foreach( var param in _findMemberParams  ){
                if( param.userData.userId == data.userId ){
                    removeParam = param;
                    break;
                }
            }
            if( removeParam == null ) {
                return;
            }   
            _findMemberParams.Remove(removeParam);
            _memberScroll.SetItems(_findMemberParams);
            _allSolicitationButton.interactable = _findMemberParams.Count > 0;
            _noneText.gameObject.SetActive( _findMemberParams.Count <= 0 );
        }

        
    }
}