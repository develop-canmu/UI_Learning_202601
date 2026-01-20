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
    public class ClubInformationBoardModal : ModalWindow {
    

        public class Param {
            public ClubData clubData = null;
            public ClubAccessLevel myAccessLevel = ClubAccessLevel.None;
        }


        [SerializeField]
        TextMeshProUGUI _message = null;
        [SerializeField]
        UIButton _editButton = null;
        [SerializeField]
        GameObject _scrollViewRoot = null;
        [SerializeField]
        TextMeshProUGUI _noneText = null;
        


        Param _param = null;

        protected override async UniTask OnPreOpen(object args, CancellationToken token) {
            _param = (Param)args;
            
            _message.text = _param.clubData.tactics;
            UpdateViewActive();
            await base.OnPreOpen(args, token);

            LocalSaveManager.saveData.clubCheckNotificationData.informationBoard.Update( _param.clubData );
            LocalSaveManager.Instance.SaveData();
        }

        protected async override UniTask OnOpen(CancellationToken token)
        {
            await base.OnOpen(token);;
        }

        public void OnClickClose(){
            Close();
        }

        public void OnClickEdit(){
            var param = new ClubEditInformationBoardModal.Param();
            param.clubData = _param.clubData;
            param.onUpdateMessage = (str)=>{
                _message.text = str;
                UpdateViewActive();
            };
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubEditInformationBoard, param);
        }

        void UpdateViewActive(){
            _editButton.gameObject.SetActive( _param.myAccessLevel == ClubAccessLevel.Master || _param.myAccessLevel == ClubAccessLevel.SubMaster );
            _scrollViewRoot.gameObject.SetActive( !string.IsNullOrEmpty(_message.text) );
            _noneText.gameObject.SetActive( string.IsNullOrEmpty(_message.text) );

        }
        

    }
}
