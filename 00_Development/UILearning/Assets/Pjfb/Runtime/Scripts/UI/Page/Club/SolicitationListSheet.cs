using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using Pjfb.UserData;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Networking.App;
using TMPro;
using CruFramework.UI;
using Pjfb.Storage;

namespace Pjfb.Club {
    public class SolicitationListSheet : FindClubPageSheet {
        [SerializeField]
        TextMeshProUGUI _emptyText = null;

        [SerializeField]
        ScrollGrid _scrollGrid = null;
        [SerializeField]
        TextMeshProUGUI _solicitationStateText = null;

        string _allowInviteText = null;
        string _notAllowInviteText = null;

        protected async override UniTask OnPreOpen(object args)
        {
            _allowInviteText = StringValueAssetLoader.Instance["menu.club.allow_invite"];
            _notAllowInviteText = StringValueAssetLoader.Instance["menu.club.not_allow_invite"];
            await UpdateList();
            UpdateSolicitationStateText(false);
        }

        private void UpdateSolicitationStateText()
        {
            _solicitationStateText.text = UserDataManager.Instance.user.allowsGuildInvitation ? _allowInviteText : _notAllowInviteText;
        }

        private void UpdateSolicitationStateText(bool isUpdate)
        {
            UpdateSolicitationStateText();
            if( isUpdate ) {
                _page.notification.ShowNotification(StringValueAssetLoader.Instance["club.updateSolicitationState"]);
            }
        }

        public void OnClickSettingSolicitationStateButton(){
            Pjfb.Menu.ClubSolicitationSettingsModalWindow.Open(new Pjfb.Menu.ClubSolicitationSettingsModalWindow.WindowParams
            {
                onClosed = UpdateSolicitationStateText
            });
        }

        void Update(){
            UpdateSolicitationStateText();
        }
        async UniTask UpdateList(){
            
            var request = new GuildGetInvitationGuildListAPIRequest();
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            var paramList = new List<SolicitationClubListItem.Param>();
            foreach( var guild in response.invitationList ){
                
                var isBadgeView = !LocalSaveManager.saveData.clubCheckNotificationData.solicitation.Contains(guild.gMasterId);
                var param = new SolicitationClubListItem.Param( isBadgeView , guild, OnClickedAgreeButton, OnClickedDisagreeButton );
                paramList.Add(param);

            }
            _scrollGrid.SetItems(paramList);
            
            _emptyText.gameObject.SetActive(response.invitationList.Length <= 0);
            _scrollGrid.gameObject.SetActive(response.invitationList.Length > 0);

            LocalSaveManager.saveData.clubCheckNotificationData.solicitation.Update(response);
            LocalSaveManager.Instance.SaveData();
        }


        async UniTask OnClickedAgreeButton( ClubData data, ClubInfoModal modal ){
            if( await ClubUtility.CheckAndShowConfirmByBlockUser(data.memberList) ) {
                try{
                    await ConnectAgreeAPI( data.clubId );
                } catch( APIException e ){
                    if (APIUtility.CalcErrorType(e.errorParam) != ErrorModalType.MessageOnly) {
                        // メッセージのエラー以外は移行の処理をしない
                        return;
                    } else {
                       //リストを更新する
                       await UpdateList();
                    }

                }
                
                if( modal != null ) {
                    await modal.CloseAsync();
                }
            }
        }

        void OnClickedDisagreeButton( long guildId ){
            
            ConnectDisagreeAPI( guildId ).Forget();
        }

        async UniTask ConnectAgreeAPI( long guildId ){

            var request = new GuildAgreeInvitationAPIRequest();
            var post = new GuildAgreeInvitationAPIPost();
            post.targetGMasterId = guildId;
            request.SetPostData(post);

            await APIManager.Instance.Connect(request);
            //ローカルデータの更新
            UserDataManager.Instance.user.UpdateGuildData(request);
            AppManager.Instance.UIManager.Footer.UpdateClubBadge();

            //クラブの所属画面に遷移
            var sheetManager = (ClubPage)_page.Manager;
            var getGuildRequest = new GuildGetGuildAPIRequest();
            await APIManager.Instance.Connect(getGuildRequest);
            var response = getGuildRequest.GetResponseData();
            var pageParam = new MemberTopPage.Param();
            pageParam.clubData = new ClubData(response.guild);
            pageParam.isShowCreateNotification = false;
            pageParam.isShowJoinNotification = true;
            pageParam.myAccessLevel = ClubUtility.CreateAccessLevel( UserDataManager.Instance.user.uMasterId, pageParam.clubData );
            pageParam.guildBattleMatchingList = response.guildBattleMatchingList;
            await sheetManager.OpenPageAsync(ClubPageType.MemberTop, true, pageParam);
        }

        async UniTask ConnectDisagreeAPI( long guildId ){
            var request = new GuildDisagreeInvitationAPIRequest();
            var post = new GuildDisagreeInvitationAPIPost();
            post.targetGMasterId = guildId;
            request.SetPostData(post);
            
            await APIManager.Instance.Connect(request);

            await UpdateList();
        }
        
    }
}