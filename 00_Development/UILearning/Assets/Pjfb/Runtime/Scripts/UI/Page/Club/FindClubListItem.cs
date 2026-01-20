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
    public class FindClubListItem : ClubListItem {

        public void OnClickItem(){
            OpenJoinRequestModal().Forget();
        }

        public async UniTask OpenJoinRequestModal(){
            var request = new GuildGetGuildAPIRequest();
            var post = new GuildGetGuildAPIPost();
            post.gMasterId = _param.gMasterId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();

            var modalParam = new ClubInfoModal.Param();
            modalParam.clubData = new ClubData(response.guild);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubInfo, modalParam);

        }
        
    }
}