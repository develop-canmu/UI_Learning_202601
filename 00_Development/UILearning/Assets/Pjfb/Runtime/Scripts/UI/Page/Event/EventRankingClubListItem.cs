using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using TMPro;
using CruFramework.UI;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Club;
using Pjfb.UserData;

namespace Pjfb.EventRanking {
    public class EventRankingClubListItem : EventRankingListItem {
        public class Param{
            public string name = "";
            public long id = 0;
            public long emblemId = 0;
            public long rank = 0;
            public BigValue point = BigValue.Zero;
            public bool isCurrent = false;
        }

        [SerializeField]
        TextMeshProUGUI _name = null;
        [SerializeField]
        TextMeshProUGUI _point = null;
        [SerializeField]
        OmissionTextSetter _pointOmissionTextSetter = null;
        [SerializeField]
        Image _emblem = null;

        Param _param = null;

        public void UpdateView(Param param){
            _param = param;
            _name.text = param.name;
            _point.text = param.point.ToDisplayString(_pointOmissionTextSetter.GetOmissionData());
            UpdateRankView( param.rank, param.isCurrent );
            ClubUtility.LoadAndSetEmblemIconSync(_emblem, param.emblemId);
        }

        public void OnClickItem(){
            OpenClubInfo( _param.id ).Forget();
            
        }

        async UniTask OpenClubInfo( long id ){
            //クラブ情報表示
            var request = new GuildGetGuildAPIRequest();
            var post = new GuildGetGuildAPIPost();
            post.gMasterId = id;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            
            var param = new ClubInfoModal.Param();
            param.clubData = new ClubData(response.guild);
            param.myUserID = 0;
            param.showUserProfileOtherButtons = false;
            param.showHeaderButtons = false;
            param.onFinishedDissolution = null;
            param.onFinishedSecession = null;
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubInfo, param);

        }


        protected override void OnSetView(object value){
            var param = value as Param;
            UpdateView(param);
        }

        
    }
}