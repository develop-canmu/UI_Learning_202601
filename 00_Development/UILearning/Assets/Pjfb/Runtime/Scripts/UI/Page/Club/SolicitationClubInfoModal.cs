using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using CruFramework.UI;

namespace Pjfb.Club
{
    public class SolicitationClubInfoModal : ClubInfoModal.OverrideHandler
    {
        public class Param : ClubInfoModal.Param {
            public Func<ClubData, ClubInfoModal, UniTask> onClickedAgree {get; set;} 
            public Action<long> onClickedDisagree {get; set;} 

        }

        [SerializeField]
        UIButton _agreeButton = null;


        Param _param = null;
        ClubInfoModal _modal = null;
        public override void Init( ClubInfoModal modal ,ClubInfoModal.Param param ){
            _param = (Param)param;
            _modal = modal;
            _agreeButton.interactable = UserData.UserDataManager.Instance.user.gMasterId == 0;
        }

        public override UniTask UpdateView( ClubInfoModal modal, ClubData data, ClubAccessLevel accessLevel ) {
            modal.editButton.gameObject.SetActive(false);
            return default;
        }

        public void OnClickedAgree(){
            JoinAgree().Forget();
        }

        public void OnClickedDisagree(){
            _param.onClickedDisagree?.Invoke(_param.clubData.clubId);
            _modal.Close();
        }

        public async UniTask JoinAgree(){
            if( _param.onClickedAgree!= null ) {
                await _param.onClickedAgree(_param.clubData, _modal);
            }
        }

    }
}
