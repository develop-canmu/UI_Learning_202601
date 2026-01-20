using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using CruFramework.UI;
using Pjfb.Menu;
using Pjfb.Storage;
using Pjfb.Runtime.Scripts.Utility;
using Pjfb.UserData;

namespace Pjfb.Club
{
    public class ClubInfoModal : ModalWindow {
        public abstract class OverrideHandler : MonoBehaviour{
            public abstract void Init( ClubInfoModal modal, ClubInfoModal.Param param );
            public abstract UniTask UpdateView(ClubInfoModal modal, ClubData data, ClubAccessLevel accessLevel);
            
        }

        public class Param {
            public Func<ClubData, UniTask> onUpdateClub = null;
            public Action onFinishedDissolution = null;
            public Action onFinishedSecession = null;
            public ClubData clubData = null;
            public Func<UniTask> updateViewRequest = null;
            //自身のユーザーId
            public long myUserID = 0;
            public bool showUserProfileOtherButtons = true;
            public bool showHeaderButtons = true;
        }

        public UIButton editButton => _editButton;
        public UINotification notification => _notification;
        public OverrideHandler handler => _handler;

        [SerializeField]
        TextMeshProUGUI _clubName = null;
        [SerializeField]
        TextMeshProUGUI _power = null;
        [SerializeField]
        OmissionTextSetter _powerOmissionTextSetter = null;
        [SerializeField]
        TextMeshProUGUI _introduction = null;
        [SerializeField]
        Image _rank = null;
        [SerializeField]
        Image _emblem = null;
        [SerializeField]
        TextMeshProUGUI _activityPolicy = null;
        [SerializeField]
        TextMeshProUGUI _joinConditions = null;
        [SerializeField]
        TextMeshProUGUI _clubMatchPolicy = null;
        [SerializeField]
        TextMeshProUGUI _participationPriority = null;
        [SerializeField]
        ClubInfoModalSheetManager _sheetManager = null;
        [SerializeField]
        UIButton _editButton = null;
        [SerializeField]
        OverrideHandler _handler = null;
        [SerializeField]
        UINotification _notification = null;


        //自身のアクセスレベル
        ClubAccessLevel _myAccessLevel = ClubAccessLevel.None;
        
        Param _param = null;

        protected override async UniTask OnPreOpen(object args, CancellationToken token) {
            _param = (Param)args;
            _myAccessLevel = ClubUtility.CreateAccessLevel( _param.myUserID, _param.clubData );
            
            await UpdateView( _param.clubData, _myAccessLevel );
            
            _sheetManager.OnOpenSheet += OnOpenSheet;
            var sheetParam = new ClubInfoModalSheetParam();
            sheetParam.data = _param.clubData;
            sheetParam.showUserProfileOtherButtons = _param.showUserProfileOtherButtons;
            sheetParam.showHeaderButtons = _param.showHeaderButtons;
            sheetParam.onFinishedDissolution = _param.onFinishedDissolution;
            sheetParam.onFinishedSecession = _param.onFinishedSecession;
            
            _sheetManager.OpenSheet(ClubInfoModalSheetType.Member, sheetParam);
            var sheet =  (ClubInfoModalSheet)_sheetManager.CurrentSheet;
            await sheet.Init( sheetParam, _myAccessLevel, _param.myUserID, this, async (updateAccessLevel)=>{
                await UpdateView( _param.clubData, updateAccessLevel );
                if( _param.updateViewRequest != null ) {
                    await _param.updateViewRequest();
                }
            } );
            _handler?.Init(this, _param);

            LocalSaveManager.saveData.clubCheckNotificationData.clubInfo.Update( _param.clubData );
            LocalSaveManager.Instance.SaveData();

            await base.OnPreOpen(args, token);
        }

        protected async override UniTask OnOpen(CancellationToken token)
        {
            await base.OnOpen(token);;
        }
        

        /// <summary>
        /// 解散ボタンをおした
        /// </summary>
        public void OnClickDissolutionButton(){
            
            ConfirmModalData data = new ConfirmModalData(
                StringValueAssetLoader.Instance["club.clubDissolution"],
                StringValueAssetLoader.Instance["club.clubDissolutionText"],
                StringValueAssetLoader.Instance["club.clubDissolutionCautionText"],
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["club.executeDissolution"],ExecuteDissolution),
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.cancel"], window => window.Close())
            );
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubDissolutionConfirm, data);
            
        }


        /// <summary>
        /// 脱退ボタンをおした
        /// </summary>
        public void OnClickSecessionButton(){

            ConfirmModalData data = new ConfirmModalData(
                StringValueAssetLoader.Instance["club.secessionConfirm"],
                StringValueAssetLoader.Instance["club.secessionConfirmText"],
                null,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["club.executeSecession"],ExecuteSecession),
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.cancel"], window => window.Close())
            );
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
            
        }
        
        public void OnClickEditButton() {
            var param = new EditClubTopModal.Param();
            param.clubData = _param.clubData;
            param.onUpdateClub = async ( clubData )=>{
                RemoveTopModals();
                await UpdateView(clubData, _myAccessLevel);
                if( _param.onUpdateClub != null ) {
                    await _param.onUpdateClub(clubData);
                }
                _notification.ShowNotification(StringValueAssetLoader.Instance["club.updateClubInfoNotification"]);
            };
            
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.EditClubTop, param);
        }

        
        /// <summary>加入ボタン </summary>
        public void OnClickJoinButton()
        {
            //加入確認モーダル
            OpenConfirmJoinRequestModal(_param.clubData, this).Forget();
        }
        
        public void OnClickCloseButton() {
            
            Close();
        }


        public void RemoveTopModals(){
            AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop( (itr)=> itr != this );
        }

        private async UniTask UpdateView(ClubData data, ClubAccessLevel accessLevel)
        {
            _param.clubData = data;
            _clubName.text = _param.clubData.name;
            _myAccessLevel = accessLevel;
            _power.text = _param.clubData.power.ToDisplayString(_powerOmissionTextSetter.GetOmissionData());
            _introduction.text = _param.clubData.introduction;
            
            _activityPolicy.text = ClubUtility.FindActivityPolicyStrings(_param.clubData.activityPolicyId);
            
            var autoEnrollmentStrings = ClubUtility.CreateAutoEnrollmentStrings();
            _joinConditions.text = autoEnrollmentStrings[(int)_param.clubData.isAutoEnrollment];
            _editButton.gameObject.SetActive( accessLevel != ClubAccessLevel.None && accessLevel != ClubAccessLevel.NotAffiliation );
            var clubMatchPolicyString = ClubUtility.CreateMatchPolicyStrings();
            _clubMatchPolicy.text = clubMatchPolicyString[(int)_param.clubData.clubMatchPolicy];
            // 参加優先度の文字列を取得
            _participationPriority.text = ClubUtility.GetParticipationPriorityData(_param.clubData.participationPriority).name;
            
            if( _handler != null ) {
                await _handler.UpdateView(this, data, accessLevel);
            }
            
            await ClubUtility.LoadAndSetEmblemIcon( _emblem, _param.clubData.emblemId );
            await ClubUtility.LoadAndSetRankIcon( _rank, _param.clubData.rankId );
        }
        

        /// <summary>
        /// 解散完了モーダル
        /// </summary>        
        private void OpenFinishedDissolutionModal() {
            ConfirmModalData data = new ConfirmModalData(
                StringValueAssetLoader.Instance["club.clubDissolution"],
                StringValueAssetLoader.Instance["club.finishedClubDissolution"],
                null,
                null,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], window =>{
                    window.Close();
                    _param.onFinishedDissolution?.Invoke();
                })
            );
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
        }

        /// <summary>
        /// 脱退完了モーダル
        /// </summary>
        private void OpenFinishedSecessionModal() {
            ConfirmModalData data = new ConfirmModalData(
                StringValueAssetLoader.Instance["club.secession"],
                StringValueAssetLoader.Instance["club.secessionText"],
                null,
                null,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], window =>{
                    window.Close();
                    _param.onFinishedSecession?.Invoke();
                })
            );
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
        }
        
        //// <summary>クラブ加入確認モーダル</summary>
        private async UniTask OpenConfirmJoinRequestModal( ClubData param , ClubInfoModal clubInfoModal){ 
            
            ConfirmModalData data = new ConfirmModalData(
                StringValueAssetLoader.Instance["common.confirm"],
                string.Format(StringValueAssetLoader.Instance["club.joinRequestConfirm"], param.name),
                null,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], async (window) =>
                {
                    // 自動認証の場合はプロフィール画面を開く
                    if (param.isAutoEnrollment == ClubEntryPermissionType.AutoApprove)
                    {
                        // プロフィールモーダル以外を削除
                        AppManager.Instance.UIManager.ModalManager.RemoveTopModalsIgnoreTop(m => m is not TrainerCardModalWindow);
                    }
                   
                    //ブロックユーザー確認
                    if( ! await ClubUtility.CheckAndShowConfirmByBlockUser(param.memberList) ) {
                        return;
                    }

                    await ConnectJoinRequestAPI(param, clubInfoModal);  
                    window.Close(); 
                }),
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.cancel"], (window) => { 
                    window.Close();
                })
            );
            var modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.Confirm, data);
            await modal.WaitCloseAsync();
        }
        
        //// <summary> クラブ参加リクエスト </summary>
        private async UniTask ConnectJoinRequestAPI(ClubData param, ClubInfoModal modal)
        {

            // 自動承認
            if (param.isAutoEnrollment == ClubEntryPermissionType.AutoApprove)
            {
                var post = new GuildJoinAPIPost();
                post.targetGMasterId = param.clubId;
                var request = new GuildJoinAPIRequest();
                request.SetPostData(post);
                await APIManager.Instance.Connect(request);
                //自動承認の場合はクラブ所属画面に遷移
                UserData.UserDataManager.Instance.user.UpdateGuildData(request);
                AppManager.Instance.UIManager.Footer.UpdateClubBadge();

                //クラブ情報取得
                var getClubRequest = new GuildGetGuildAPIRequest();
                await APIManager.Instance.Connect(getClubRequest);
                var response = getClubRequest.GetResponseData();

                //メンバートップへの移動はクラブページ以外の時があるのでクラブページにいるかで処理を分岐する
                ClubPage pageManager;
                // 今クラブページにいるなら
                if (AppManager.Instance.UIManager.PageManager.CurrentPageObject is ClubPage page)
                {
                    pageManager = page;
                    // クラブのメンバートップに移動する
                    var pageParam = new MemberTopPage.Param();
                    pageParam.clubData = new ClubData(response.guild);
                    pageParam.isShowCreateNotification = false;
                    pageParam.isShowJoinNotification = true;
                    pageParam.myAccessLevel = ClubUtility.CreateAccessLevel(UserDataManager.Instance.user.uMasterId, pageParam.clubData);
                    pageParam.guildBattleMatchingList = response.guildBattleMatchingList;
                    await pageManager.OpenPageAsync(ClubPageType.MemberTop, true, pageParam);
                }
                // クラブページ以外の場合、SystemUIManager側から通知を流す
                else
                {
                    var text = string.Format(StringValueAssetLoader.Instance["club.joinClubNotification"], _param.clubData.name);
                    AppManager.Instance.UIManager.System.UINotification.ShowNotification(text);
                }
            }
            else
            {
                var post = new GuildJoinRequestAPIPost();
                post.targetGMasterId = param.clubId;
                var request = new GuildJoinRequestAPIRequest();
                request.SetPostData(post);
                await APIManager.Instance.Connect(request);

                if (_sheetManager.CurrentSheet is ClubInfoModalMemberSheet memberSheet)
                {
                    memberSheet.SetRequestButtonState(false);
                }

                modal.notification.ShowNotification(StringValueAssetLoader.Instance["club.sendJoinRequestNotification"]);
            }
        }

        private void ExecuteDissolution( ModalWindow window ) {
            DissolutionTask().Forget();   
        }

        private void ExecuteSecession( ModalWindow window ) {
            SecessionTask().Forget();   
        }

        private async UniTask DissolutionTask() {
            var request = new GuildDissoluteAPIRequest();
            await APIManager.Instance.Connect(request);
            AppManager.Instance.UIManager.Footer.UpdateClubBadge();
            // ローカルPUSHの登録削除
            LocalPushNotificationUtility.AllClear();
            OpenFinishedDissolutionModal(); 
        }

        private async UniTask SecessionTask() {
            var request = new GuildWithdrawAPIRequest();
            await APIManager.Instance.Connect(request);
            AppManager.Instance.UIManager.Footer.UpdateClubBadge();
            // ローカルPUSHの登録削除
            LocalPushNotificationUtility.AllClear();
            OpenFinishedSecessionModal(); 
        }



        private void OnOpenSheet( ClubInfoModalSheetType type ){
            var sheet =  (ClubInfoModalSheet)_sheetManager.CurrentSheet;
            var sheetParam = new ClubInfoModalSheetParam();
            sheetParam.data = _param.clubData;
            sheetParam.showUserProfileOtherButtons = _param.showUserProfileOtherButtons;
            sheetParam.showHeaderButtons = _param.showHeaderButtons;
            
            sheet.Init( sheetParam, _myAccessLevel, _param.myUserID, this, async (updateAccessLevel)=>{
                await UpdateView( _param.clubData, updateAccessLevel );
                if (_param.onUpdateClub != null)
                {
                    await _param.onUpdateClub(_param.clubData);
                }
                if( _param.updateViewRequest != null ) {
                    await _param.updateViewRequest();
                }
            } ).Forget();
        }

    }
}
