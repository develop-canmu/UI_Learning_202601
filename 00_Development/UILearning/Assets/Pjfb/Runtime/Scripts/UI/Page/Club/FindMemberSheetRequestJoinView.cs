using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.Page;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Cysharp.Threading.Tasks;
using CruFramework.UI;
using TMPro;
using Pjfb.Storage;

namespace Pjfb.Club {
    public class FindMemberSheetRequestJoinView : FindMemberSheetView {
        [SerializeField]
        ScrollGrid _scroll = null;
        [SerializeField]
        TextMeshProUGUI _noneText = null;

        protected override async UniTask OnPreOpen(object args) {
            
            _noneText.gameObject.SetActive( false );
            await base.OnPreOpen(args);
        }

        protected override void InitView(){
            UpdateRequestList();
        }
       
        /// <summary>
        /// ユーザーの招待のキャンセル
        /// </summary>
        /// <param name="data"></param>
        async UniTask ConnectCancelInvitationAPI( ClubInvitationMemberData cancelUser ) {
            var request = new GuildCancelInvitationAPIRequest();
            var post = new GuildCancelInvitationAPIPost();
            post.targetUMasterId = cancelUser.userId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);

            UpdateRequestList();
        }


        /// <summary>
        /// 勧誘中リスト更新
        /// </summary>
        /// <param name="editData"></param>
        void UpdateRequestList() {
            var listParam = new List<RequestJoinMemberListItem.Param>();
            var notificationData = LocalSaveManager.saveData.clubCheckNotificationData;
            foreach( var user in _clubData.requestUserList ) {
                var isViewBadge = !notificationData.requestJoin.Contains( user.requestId );
                var param = new RequestJoinMemberListItem.Param( isViewBadge, user, _myAccessLevel, OnClickAgreeButton, OnClickDisagreeButton );
                listParam.Add(param);
            }
            _scroll.SetItems(listParam);

            _noneText.gameObject.SetActive( listParam.Count <= 0 );
            _onFinishedInit?.Invoke(this);
        }

        /// <summary>
        /// 承認ボタンをおした
        /// </summary>
        /// <param name="cancelUser"></param>
        void OnClickAgreeButton( ClubRequestUserData user ) {
            ConnectAgreeAPI( user ).Forget();
        }


        async UniTask ConnectAgreeAPI( ClubRequestUserData user ){
            var request = new GuildAgreeJoinRequestAPIRequest();
            var post = new GuildAgreeJoinRequestAPIPost();
            post.gJoinRequestId = user.requestId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);

            await UpdateClubData();

            UpdateRequestList();

        }   

        /// <summary>
        /// 拒否ボタンを押した
        /// </summary>
        /// <param name="user"></param>
        void OnClickDisagreeButton( ClubRequestUserData user ) {
            ConnectDisagreeAPI( user ).Forget();
        }

        async UniTask ConnectDisagreeAPI( ClubRequestUserData user ){
            var request = new GuildDisagreeJoinRequestAPIRequest();
            var post = new GuildDisagreeJoinRequestAPIPost();
            post.targetUMasterId = user.userId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);

            await UpdateClubData();

            UpdateRequestList();
        }   

        async UniTask UpdateClubData(  ){
            var request = new GuildGetGuildAPIRequest();
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            _clubData.UpdateData(response.guild);
        }   

    }
}