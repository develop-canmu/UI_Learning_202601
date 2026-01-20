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
    public class FindMemberSheetSolicitingView : FindMemberSheetView {
        
        [SerializeField]
        ScrollGrid _scroll = null;
        [SerializeField]
        TextMeshProUGUI _noneText = null;


        protected override async UniTask OnPreOpen(object args) {
            
            _noneText.gameObject.SetActive( false );
            await base.OnPreOpen(args);
        }

        protected override async UniTask OnOpen(object args) {

            await base.OnOpen(args);

            
        }

        protected override void InitView(){
            UpdateRequestList().Forget();
        }

        /// <summary>
        /// キャンセルボタンをおした
        /// </summary>
        /// <param name="cancelUser"></param>
        void OnClickCancelButton( ClubUserData cancelUser ) {
            ConnectCancelInvitationAPI( cancelUser ).Forget();
        }

        /// <summary>
        /// ユーザーの招待のキャンセル
        /// </summary>
        /// <param name="data"></param>
        async UniTask ConnectCancelInvitationAPI( ClubUserData cancelUser ) {
            var request = new GuildCancelInvitationAPIRequest();
            var post = new GuildCancelInvitationAPIPost();
            post.targetUMasterId = cancelUser.userId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            LocalSaveManager.saveData.clubCheckNotificationData.sendSolicitation.Remove(cancelUser.userId);
            await UpdateRequestList();
        }


        /// <summary>
        /// 勧誘中リスト更新
        /// </summary>
        /// <param name="editData"></param>
        async UniTask UpdateRequestList() {
            var request = new GuildGetInvitationUserListAPIRequest();
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            var listParam = new List<SolicitingMemberListItem.Param>();
            foreach( var user in response.userList ) {
                var isBadgeView = !LocalSaveManager.saveData.clubCheckNotificationData.sendSolicitation.Contains(user.uMasterId);
                var param = new SolicitingMemberListItem.Param( isBadgeView,  new ClubInvitationMemberData(user), _myAccessLevel ,OnClickCancelButton );
                listParam.Add(param);
            }
            _scroll.SetItems(listParam);

            _noneText.gameObject.SetActive( listParam.Count <= 0 );
            LocalSaveManager.saveData.clubCheckNotificationData.sendSolicitation.Update(response);
            LocalSaveManager.Instance.SaveData();
            _onFinishedInit?.Invoke(this);
        }

           
    }
}