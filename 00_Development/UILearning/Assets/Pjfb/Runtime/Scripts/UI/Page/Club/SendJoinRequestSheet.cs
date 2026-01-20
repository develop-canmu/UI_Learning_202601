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
    public class SendJoinRequestSheet : FindClubPageSheet {
        [SerializeField]
        TextMeshProUGUI _emptyText = null;

        [SerializeField]
        ScrollGrid _scrollGrid = null;
        
        protected async override UniTask OnPreOpen(object args)
        {
            await UpdateList();
            await base.OnPreOpen(args);
        }

        
        async UniTask UpdateList(){
            
            var request = new GuildGetJoinRequestAPIRequest();
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            
            var paramList = new List<SendJoinRequestListItem.Param>();
            foreach( var guild in response.guildList ){
                
                var isBadgeView = !LocalSaveManager.saveData.clubCheckNotificationData.solicitation.Contains(guild.gMasterId);
                var param = new SendJoinRequestListItem.Param(  guild, OnClickedCancelButton );
                paramList.Add(param);

            }
            _scrollGrid.SetItems(paramList);
            
            _emptyText.gameObject.SetActive( paramList.Count <= 0);
            _scrollGrid.gameObject.SetActive( paramList.Count > 0);
        }

        void OnClickedCancelButton( ClubListItem.Param param ){
            var text = string.Format( StringValueAssetLoader.Instance["club.sendJoinRequestCancelText"], param.name );
            ConfirmModalData data = new ConfirmModalData(
                StringValueAssetLoader.Instance["common.confirm"],
                text,
                null,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], window => {
                    CancelRequest(window, param).Forget();
                }),
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.cancel"], window => {
                    window.Close();
                })
            );
            
            AppManager.Instance.UIManager.ErrorModalManager.OpenModal(ModalType.Confirm, data);
        }
        async UniTask CancelRequest ( ModalWindow window, ClubListItem.Param param ){
            var request = new GuildCancelJoinRequestAPIRequest();
            var post = new GuildCancelJoinRequestAPIPost();
            post.targetGMasterId = param.gMasterId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);

            await window.CloseAsync();

            await UpdateList();
        }
    }
}