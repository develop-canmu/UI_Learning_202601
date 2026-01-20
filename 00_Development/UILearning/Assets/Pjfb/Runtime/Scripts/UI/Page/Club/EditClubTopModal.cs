using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using System;
using System.Threading;
using Pjfb.Storage;

namespace Pjfb.Club
{

    public class EditClubTopModal : ModalWindow
    {
        public class Param {
            public Func<ClubData, UniTask> onUpdateClub = null;
            public ClubData clubData = null;
        }

        [SerializeField]
        EditClubView _editView = null;

        Param _param = null;
        

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            _param = (Param)args;
            var editData = new EditClubDate(_param.clubData);
            await _editView.Init( editData, _param.clubData.emblemIdList );
            await base.OnPreOpen(args, token);
        }

        
        public void OnClickCancelButton()
        {
            Close();
        }

        public void OnClickOkButton()
        {
             ConfirmModalData data = new ConfirmModalData(
                StringValueAssetLoader.Instance["club.updateInfoConfirmTitle"],
                StringValueAssetLoader.Instance["club.updateInfoConfirmText"],
                null,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.executeUpdate"], UpdateInfo),
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.cancel"], window => window.Close())
            );
            // uiManager.ModalManager.OpenModal(ModalType.Confirm, data);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
        }


        void UpdateInfo( ModalWindow window ){
            UpdateInfoSync(window).Forget();;
        }

        async UniTask UpdateInfoSync( ModalWindow window ){
            var editData = _editView.CreateEditData();

            var request = new GuildEditAPIRequest();
            var post = new GuildEditAPIPost();
            post.mGuildEmblemId = editData.emblemId;
            post.introduction = editData.introduction;
            post.mGuildPlayStyleId = editData.activityPolicyId;
            post.membersWantedFlg =  editData.recruitmentStatus;
            post.membersWantedComment = editData.recruitmentComment;
            post.autoEnrollmentFlg = (long)editData.isAutoEnrollment;
            post.guildBattleStyleType = editData.clubMatchPolicy;
            post.tactics = _param.clubData.tactics;
            post.participationPriorityType = editData.participationPriority;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            _param.clubData.UpdateData(editData);
            if( _param.onUpdateClub != null ) {
                await _param.onUpdateClub( _param.clubData );
            }

            LocalSaveManager.saveData.clubCheckNotificationData.clubInfo.Update( _param.clubData );
            LocalSaveManager.Instance.SaveData();
        
            window.Close();
        }


    }
}
