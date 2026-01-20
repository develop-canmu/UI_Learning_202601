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
using System;
using System.Threading;
using CruFramework.ResourceManagement;
using CruFramework;


namespace Pjfb.Club
{
    
    public class ClubInvitationUserModal : ModalWindow
    {
        
        public class Param {
            public ClubUserData user = null;
            public Action<ClubUserData> onInvitationUser = null;
        }

        [SerializeField]
        TextMeshProUGUI _description = null;
        [SerializeField]
        ClubInputField _comment = null;

        Param _param = null;
        
        protected async override UniTask OnPreOpen(object args, CancellationToken token)
        {

            _param = (Param)args;
            _description.text = string.Format( StringValueAssetLoader.Instance["club.invitationMessageText"], _param.user.name );
            _comment.text = StringValueAssetLoader.Instance["club.invitationMessageDefault"];

            await base.OnPreOpen(args, token);
        }

        public void OnClickInvitationButton(){
            ConnectInvitationAPI().Forget();
        }

        public void OnClickClose(){
            Close();
        }
        

        public async UniTask ConnectInvitationAPI(){
            var request = new GuildSendInvitationAPIRequest();
            var post = new GuildSendInvitationAPIPost();
            post.targetUMasterIdListStr = _param.user.userId.ToString();
            post.message = _comment.text;
            
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            
            _param.onInvitationUser?.Invoke(_param.user);
            Close();
        }   
    }
}
