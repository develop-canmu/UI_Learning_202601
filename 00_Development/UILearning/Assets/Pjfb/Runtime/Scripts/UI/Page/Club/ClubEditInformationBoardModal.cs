using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using CruFramework.UI;
using Pjfb.Storage;

namespace Pjfb.Club
{
    public class ClubEditInformationBoardModal : ModalWindow {
    

        public class Param {
            public ClubData clubData = null;
            public System.Action<string> onUpdateMessage = null;
        }


        [SerializeField]
        ClubInputField _editText = null;
        [SerializeField]
        UIButton _updateButton = null;
        


        Param _param = null;

        protected override async UniTask OnPreOpen(object args, CancellationToken token) {
            _param = (Param)args;
            
            _editText.text = _param.clubData.tactics;
            _updateButton.interactable = false;
            await base.OnPreOpen(args, token);
        }

        protected async override UniTask OnOpen(CancellationToken token)
        {
            await base.OnOpen(token);;
        }

        public void OnClickUpdate(){
            
            // モーダルデータ
            ConfirmModalData modalData = new ConfirmModalData();
            modalData.Title = StringValueAssetLoader.Instance["gacha.editInformationBoardConfirmTitle"];
            modalData.Message = StringValueAssetLoader.Instance["gacha.editInformationBoardConfirmText"];
            modalData.NegativeButtonParams = new ConfirmModalButtonParams(
                StringValueAssetLoader.Instance["common.cancel"],
                window =>
                {
                    
                    window.Close();
                }
            );

            modalData.PositiveButtonParams = new ConfirmModalButtonParams(
                StringValueAssetLoader.Instance["common.executeUpdate"],
                window =>
                {
                    UpdateMessage(window);
                }
            );
            AppManager.Instance.UIManager.ErrorModalManager.OpenModal(ModalType.Confirm, modalData);
        }

        public void OnEndEdit( string str ){
            _updateButton.interactable = _param.clubData.tactics != str;
            
        }
        public void OnClickClose(){
            Close();
        }

        void UpdateMessage( ModalWindow window  ){
            UpdateMessageTask(window).Forget();
        }

        async UniTask UpdateMessageTask( ModalWindow window  ){
            await window.CloseAsync();

            var request = new GuildEditAPIRequest();
            var post = new GuildEditAPIPost();
            post.mGuildEmblemId = _param.clubData.emblemId;
            post.introduction = _param.clubData.introduction;
            post.mGuildPlayStyleId = _param.clubData.activityPolicyId;
            post.membersWantedFlg =  _param.clubData.recruitmentStatus;
            post.membersWantedComment = _param.clubData.recruitmentComment;
            post.autoEnrollmentFlg = (long)_param.clubData.isAutoEnrollment;
            post.guildBattleStyleType = _param.clubData.clubMatchPolicy;
            post.tactics = _editText.text;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            
            _param.clubData.UpdateTactics(_editText.text);
            _param.onUpdateMessage?.Invoke(_editText.text);
            OnEndEdit(_editText.text);

            LocalSaveManager.saveData.clubCheckNotificationData.informationBoard.Update( _param.clubData );
            LocalSaveManager.Instance.SaveData();
        }


    }
}
