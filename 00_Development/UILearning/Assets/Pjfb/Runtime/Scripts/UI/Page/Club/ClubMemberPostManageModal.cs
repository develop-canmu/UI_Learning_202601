using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;

namespace Pjfb.Club
{
    public class ClubMemberPostManageModal : ModalWindow
    {

        public class Param{
            public ClubData clubData{get;set;} = null;
            public ClubMemberData member{get;set;} = null;
            public ClubAccessLevel myAccessLevel{get;set;} = ClubAccessLevel.None;
            public Func<ClubAccessLevel, string, UniTask> updateViewRequest{get;set;} = null;
            public Action closeModalRequest{get;set;} = null;
            public long myUserID{get;set;} = 0;
        }
       
        [SerializeField]
        UIButton _masterButton = null;
        [SerializeField]
        UIButton _subMasterButton = null;
        [SerializeField]
        TextMeshProUGUI _subMasterButtonText = null;
        [SerializeField]
        UIButton _noneRoleButton = null;
        [SerializeField]
        UIButton _expulsionButton = null;

        //キャプテンの人数
        [SerializeField]
        TextMeshProUGUI _masterMemberCount = null;
        //サブキャプテンの人数
        [SerializeField]
        TextMeshProUGUI _subMasterMemberCount = null;
        [SerializeField]
        ClubMemberInfoView _memberView = null;
       


        Param _param = null;
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token) {
            _param = (Param)args;
            UpdateView();
            await base.OnPreOpen(args, token);
        }


        public void OnClickCloseButton() {
            Close();
        }

        /// <summary>
        /// 除名ボタンがおされた
        /// </summary>
        public void OnClickExpulsionButton(){
            if( _param.myAccessLevel != ClubAccessLevel.Master ) {
                return;
            }
            var title = StringValueAssetLoader.Instance["club.expulsionConfirmTitle"];
            var text = string.Format(StringValueAssetLoader.Instance["club.expulsionConfirmText"], _param.member.name );
            var buttonText = StringValueAssetLoader.Instance["club.doExpulsion"];
            OpenConfirmDialog(title, text, buttonText, true, ExpulsionMember, true);
        }

        /// <summary>
        /// 任命解除ボタンが押された
        /// </summary>
        public void OnClickRoleReleaseButton(){
            if( _param.myAccessLevel == ClubAccessLevel.None ) {
                return;
            }

            var title = StringValueAssetLoader.Instance["club.releaseSubMasterConfirmTitle"];
            var text = string.Format(StringValueAssetLoader.Instance["club.releaseSubMasterConfirmText"], _param.member.name );
            var buttonText = StringValueAssetLoader.Instance["club.doRoleRelease"];
            OpenConfirmDialog(title, text, buttonText, true, RoleRelease);

        }

        /// <summary>
        /// キャプテン管理ボタンがおされた
        /// </summary>
        public void OnClickMasterManageButton(){
            if( _param.myAccessLevel != ClubAccessLevel.Master ) {
                return;
            }
            var title = StringValueAssetLoader.Instance["club.transferConfirmTitle"];
            var text = string.Format(StringValueAssetLoader.Instance["club.transferMasterConfirmText"], _param.member.name );
            var buttonText = StringValueAssetLoader.Instance["club.doTransfer"];
            OpenConfirmDialog(title, text, buttonText, false, TransferMaster);
        }

        
        /// <summary>
        /// サブキャプテン管理ボタンがおされた
        /// </summary>
        public void OnClickSubMasterManageButton(){
            
            if( _param.myAccessLevel == ClubAccessLevel.Master ) {
                var title = StringValueAssetLoader.Instance["club.appointmentConfirmTitle"];
                var text = string.Format(StringValueAssetLoader.Instance["club.appointmentSubMasterConfirmText"], _param.member.name );
                var buttonText = StringValueAssetLoader.Instance["club.executeAppointment"];
                OpenConfirmDialog(title, text, buttonText, false, AppointmentSubMaster);
            } else if ( _param.myAccessLevel == ClubAccessLevel.SubMaster ){
                var title = StringValueAssetLoader.Instance["club.transferConfirmTitle"];
                var text = string.Format(StringValueAssetLoader.Instance["club.transferSubMasterConfirmText"], _param.member.name );
                var buttonText = StringValueAssetLoader.Instance["club.doTransfer"];
                OpenConfirmDialog(title, text, buttonText, false, TransferSubMaster);
            }
            
        }


        public void OpenConfirmDialog( string title, string text, string buttonText, bool isRedButton, Func<UniTask> taskFunction, bool isThisClose = false)
        {
             ConfirmModalData data = new ConfirmModalData(
                title,
                text,
                null,
                new ConfirmModalButtonParams(buttonText, (window)=>{ ExecuteManage(window, taskFunction, isThisClose); }, isRedButton ),
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.cancel"], window => window.Close())
            );
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
        }

        void UpdateView( ){
            var param = new ClubMemberInfoView.Param( _param.member );
            _memberView.UpdateView(param);

            var masterMemberCount = 0;
            var subMasterMemberCount = 0;
            var masterMemberCountMax = 1;
            var subMasterMemberCountMax = 2;
            //役職の人数カウント
            foreach( var member in _param.clubData.memberList ){
                var accessLevel = ClubUtility.CreateAccessLevel( member.roleId );
                if( accessLevel == ClubAccessLevel.Master ) {
                    masterMemberCount++;
                } else if( accessLevel == ClubAccessLevel.SubMaster ) {
                    subMasterMemberCount++;
                }
            }

            _masterMemberCount.text = masterMemberCount + "/" + masterMemberCountMax; //todo 最大数はサーバーからもらうようにする
            _subMasterMemberCount.text = subMasterMemberCount + "/" + subMasterMemberCountMax; //todo 最大数はサーバーからもらうようにする

            //ボタン系の更新
            var targetAccessLevel = ClubUtility.CreateAccessLevel(_param.member.roleId);

            _masterButton.gameObject.SetActive(true);
            _subMasterButton.gameObject.SetActive(true);
            _noneRoleButton.gameObject.SetActive(true);
            _expulsionButton.gameObject.SetActive(true);

            if( _param.myAccessLevel == ClubAccessLevel.Master ) {
                _masterButton.interactable = _param.myUserID != _param.member.userId;
                _subMasterButton.gameObject.SetActive(targetAccessLevel == ClubAccessLevel.None && subMasterMemberCount < subMasterMemberCountMax);
                _subMasterButtonText.text = StringValueAssetLoader.Instance["club.appointment"];
                _noneRoleButton.gameObject.SetActive(targetAccessLevel != ClubAccessLevel.None);
                _expulsionButton.interactable = _param.myUserID != _param.member.userId;
            } else if ( _param.myAccessLevel == ClubAccessLevel.SubMaster ) {
                _masterButton.gameObject.SetActive(false);
                _subMasterButton.gameObject.SetActive(targetAccessLevel == ClubAccessLevel.None &&  _param.myUserID != _param.member.userId);
                _subMasterButtonText.text = StringValueAssetLoader.Instance["club.transfer"];
                _noneRoleButton.gameObject.SetActive(_param.myUserID == _param.member.userId);
                _expulsionButton.gameObject.SetActive(false);
            } else {
                _masterButton.gameObject.SetActive(false);
                _subMasterButton.gameObject.SetActive(false);
                _noneRoleButton.gameObject.SetActive(false);
                _expulsionButton.gameObject.SetActive(false);
            }

            
        }

        /// <summary>
        /// 管理処理関数実行のラップ関数
        /// </summary>
        void ExecuteManage( ModalWindow window, Func<UniTask> taskFunction, bool isThisClose ){
            ExecuteManageTask(window, taskFunction, isThisClose).Forget();
        }


        void SetMyAccessLevel( ClubAccessLevel accessLevel ){
            var roleId = ClubUtility.AccessLevelToRoleId(accessLevel);
            if( roleId < 0 ) {
                CruFramework.Logger.LogError("failed AccessLevelToRoleId : " + accessLevel);
                return;
            }
            _param.clubData.UpdateDataRoleId(_param.myUserID, roleId);
            _param.myAccessLevel = accessLevel;
            UpdateView();
        }

        /// <summary>
        /// 管理処理関数実行のラップtask関数
        /// </summary>
        async UniTask ExecuteManageTask( ModalWindow window, Func<UniTask> taskFunction, bool isThisClose ){
            await taskFunction();
            
            if( isThisClose ) {
                _param.closeModalRequest?.Invoke();
            }
            
            window.Close();
        }


        /// <summary>
        /// サブキャプテンの任命
        /// </summary>
        async UniTask AppointmentSubMaster(){
            await ConnectUpdateRoleAPI( ClubAccessLevel.SubMaster, _param.member.userId );
            if( _param.updateViewRequest != null ) {
                var text = string.Format(StringValueAssetLoader.Instance["club.appointmentSubMasterNotification"], _param.member.name );
                await _param.updateViewRequest(_param.myAccessLevel, text);
            }
        }

        /// <summary>
        /// 役職の解任
        /// </summary>
        async UniTask RoleRelease(){
            await ConnectUpdateRoleAPI( ClubAccessLevel.None, _param.member.userId );
            if( _param.member.userId == _param.myUserID ) {
                SetMyAccessLevel( ClubAccessLevel.None );
            }
            if( _param.updateViewRequest != null ) {
                var text = string.Format(StringValueAssetLoader.Instance["club.releaseSubMasterNotification"], _param.member.name );
                await _param.updateViewRequest(_param.myAccessLevel, text);
            }
        }

        /// <summary>
        /// キャプテンの移譲
        /// </summary>
        async UniTask TransferMaster(){
            await ConnectUpdateRoleAPI( ClubAccessLevel.Master, _param.member.userId );
            SetMyAccessLevel( ClubAccessLevel.None );
            if( _param.updateViewRequest != null ) {
                var text = string.Format(StringValueAssetLoader.Instance["club.transferMasterNotification"], _param.member.name );
                await _param.updateViewRequest(_param.myAccessLevel, text);
            }
        }

        /// <summary>
        /// 副キャプテンの移譲
        /// </summary>
        async UniTask TransferSubMaster(){
            await ConnectUpdateRoleAPI(ClubAccessLevel.SubMaster, _param.member.userId, _param.myUserID );
            _param.myAccessLevel = ClubAccessLevel.None;
            UpdateView();
            if( _param.updateViewRequest != null ) {
                var text = string.Format(StringValueAssetLoader.Instance["club.transferSubMasterNotification"], _param.member.name );
                await _param.updateViewRequest(_param.myAccessLevel, text);
            }
        }

        /// <summary>
        /// 除名
        /// </summary>
        /// <returns></returns>
        async UniTask ExpulsionMember(){
            var request = new GuildKickAPIRequest();
            var post = new GuildKickAPIPost();
            post.targetUMasterId = _param.member.userId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);

            //ローカルデータの更新
            _param.clubData.RemoveMember(_param.member.userId);
            if( _param.updateViewRequest != null ) {
                var text = string.Format(StringValueAssetLoader.Instance["club.expulsionNotification"], _param.member.name );
                await _param.updateViewRequest(_param.myAccessLevel, text);
            }
        }

        
        async UniTask ConnectUpdateRoleAPI( ClubAccessLevel level, long userId, long releaseUserId = 0 ){
            var roleId = ClubUtility.AccessLevelToRoleId(level);
            if( roleId < 0 ) {
                CruFramework.Logger.LogError("failed AccessLevelToRoleId : " + level);
                return;
            }

            var request = new GuildUpdateRoleAPIRequest();
            var post = new GuildUpdateRoleAPIPost();
            post.beforeUMasterId = releaseUserId;
            post.targetUMasterId = userId;
            post.mGuildRoleId = roleId;
            request.SetPostData(post);

            await APIManager.Instance.Connect(request);

            //ローカルデータの更新
            _param.clubData.UpdateDataRoleId(userId, roleId);
            if( releaseUserId != 0 ) {
                var releaseRoleId = ClubUtility.AccessLevelToRoleId(ClubAccessLevel.None);
                if( roleId < 0 ) {
                    CruFramework.Logger.LogError("failed AccessLevelToRoleId : " + ClubAccessLevel.None);
                    return;
                }
                _param.clubData.UpdateDataRoleId(releaseUserId, releaseRoleId);
            }
            UpdateView();
        }

    }
}
