using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using TMPro;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.UserData;

namespace Pjfb.Club {
    public class CreateClubSheet : FindClubPageSheet {
        
        
        [SerializeField]
        EditClubView _editView = null;

        [SerializeField]
        UIButton _createButton = null;

        EditClubDate _editData = new();


        public void OnClickCreateButton(){
            var editData = _editView.CreateEditData();
            var param = new CreateClubConfirmModal.Param();
            param.editData = editData;
            param.onClickOk = CreateClub;
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.CreateClubConfirm, param);
        }

        public void OnFinishedEdit(){
            UpdateButtonState();
        }


        protected override async UniTask OnPreOpen(object args) {
            var selectIdList = new List<long>();
            foreach(var data in MasterManager.Instance.guildEmblemMaster.values){
                if( data.type == (int)EmblemType.General ) {
                    selectIdList.Add(data.id);
                }
            }
            await _editView.Init(_editData, selectIdList.ToArray());
            UpdateButtonState();
            await base.OnPreOpen(args);
        }

        protected override UniTask OnPreClose()
        {
            _editData = _editView.CreateEditData();
            return default;
        }


        void CreateClub( EditClubDate editData ){
            ConnectCreateAPI(editData).Forget();
        }

        async UniTask ConnectCreateAPI( EditClubDate editData ){
            var request = new GuildCreateAPIRequest();
            var post = new GuildCreateAPIPost();
            post.name = editData.name;
            post.mGuildEmblemId = editData.emblemId;
            post.introduction = editData.introduction;
            post.mGuildPlayStyleId = editData.activityPolicyId;
            post.membersWantedFlg =  editData.recruitmentStatus;
            post.membersWantedComment = editData.recruitmentComment;
            post.autoEnrollmentFlg = (long)editData.isAutoEnrollment;
            post.guildBattleStyleType = editData.clubMatchPolicy;
            post.participationPriorityType = editData.participationPriority;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            
            AppManager.Instance.UIManager.Footer.UpdateClubBadge();
            //クラブの所属画面に遷移

            //クラブ情報取得
            var getClubRequest = new GuildGetGuildAPIRequest();
            await APIManager.Instance.Connect(getClubRequest);
            var response = getClubRequest.GetResponseData();
            
            var sheetManager = (ClubPage)_page.Manager;
            var param = new MemberTopPage.Param();
            param.clubData = new ClubData(response.guild);
            param.isShowCreateNotification = true;
            param.myAccessLevel = ClubUtility.CreateAccessLevel( UserDataManager.Instance.user.uMasterId, param.clubData );
            param.guildBattleMatchingList = response.guildBattleMatchingList;
            
            await sheetManager.OpenPageAsync(ClubPageType.MemberTop, true, param);
        }

        void UpdateButtonState(){
            _createButton.interactable = (!string.IsNullOrEmpty(_editView.clubName) && !string.IsNullOrEmpty(_editView.introduction));
        }

    }
}