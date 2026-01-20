using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.UI;
using TMPro;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Cysharp.Threading.Tasks;

namespace Pjfb.Club
{
    public class SolicitationClubListItem : ClubListItem {

        public new class Param : ClubListItem.Param {
            public bool isBadgeView = false;
            public string message = ""; // 勧誘メッセージ
		    public string expireAt = ""; // 勧誘が無効になる日時

            public Func<ClubData, ClubInfoModal, UniTask> onClickedAgree {get; set;} 
            public Action<long> onClickedDisagree {get; set;} 
            public Param( bool isBadgeView, GuildInvitationInvitationStatus status, Func<ClubData, ClubInfoModal, UniTask> onClickedAgree, Action<long> onClickedDisagree ) : base(status) {
                this.isBadgeView = isBadgeView;
                this.message = status.message;
                this.expireAt = status.expireAt;
                this.onClickedAgree = onClickedAgree;
                this.onClickedDisagree = onClickedDisagree;
            }
        }

        [SerializeField]
        TextMeshProUGUI _message = null;
        [SerializeField]
        UIBadgeNotification _badge = null;

        protected override async UniTask UpdateView(ClubListItem.Param param){
            await base.UpdateView(param);
            if( param is Param ) {
                var solicitationParam = param as Param;
                _message.text = solicitationParam.message;
                _badge.SetActive( solicitationParam.isBadgeView );
            }
        }
        public void OnClickedItem(){
            OpenClubInfo().Forget();
        }


        public async UniTask OpenClubInfo(){
            var request = new GuildGetGuildAPIRequest();
            var post = new GuildGetGuildAPIPost();
            post.gMasterId = _param.gMasterId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();

            var param = (Param)_param;
            var modalParam = new SolicitationClubInfoModal.Param();
            modalParam.clubData = new ClubData(response.guild);
            modalParam.onClickedAgree = param.onClickedAgree;
            modalParam.onClickedDisagree = param.onClickedDisagree;
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.SolicitationClubInfo, modalParam);
        }
        
    }
}