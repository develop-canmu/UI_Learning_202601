using System;
using System.Collections;
using System.Collections.Generic;
using Pjfb.Menu;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pjfb.Networking.App.Request;

namespace Pjfb.Club
{
    public class ClubMemberInfoView : MonoBehaviour {
        public class Param{
            public ClubUserData user{get; private set;}
            public bool showUserProfileOtherButtons = true;
            public bool showClubInfoHeaderButtons = true;
            public Action onFinishedDissolution = null;
            public Action onFinishedSecession = null;
            
            public Param( ClubUserData status, bool showUserProfileOtherButtons = true ,bool showClubInfoHeaderButtons = true , Action onFinishedDissolution = null, Action onFinishedSecession = null){
                this.user = status;
                this.showUserProfileOtherButtons = showUserProfileOtherButtons;
                this.showClubInfoHeaderButtons = showClubInfoHeaderButtons;
                this.onFinishedDissolution = onFinishedDissolution;
                this.onFinishedSecession = onFinishedSecession;
            }   
        } 

        [SerializeField]
        TextMeshProUGUI _memberName = null;
        [SerializeField]
        TextMeshProUGUI _power = null;
        [SerializeField]
        OmissionTextSetter _powerOmissionTextSetter = null;
        [SerializeField]
        TextMeshProUGUI _login = null;

        [SerializeField]
        ClubCharacterIcon _character = null;
        [SerializeField]
        DeckRankImage _rank = null;
        [SerializeField]
        UserTitleImage _emblem = null;

        Param _param = null;

        public void UpdateView( Param param ){
            _param = param;
            _memberName.text = param.user.name;
            _power.text = param.user.power.ToDisplayString(_powerOmissionTextSetter.GetOmissionData());
            _character.UpdateIcon(param.user.iconId);
            _character.UpdateBadge( param.user.roleId);
            _rank.SetTexture(param.user.deckRank);
            _emblem.SetTexture(param.user.emblemId);

            if( _login != null ){
                _login.text = ClubUtility.CreateLastLoginText(param.user.lastLogin);
            }
        }

        public void OpenProfile()
        {
            //すでにプロフィールを開いていたら奥のを削除する
            AppManager.Instance.UIManager.ModalManager.RemoveModals(window => window is Pjfb.Menu.TrainerCardModalWindow);
            TrainerCardModalWindow.WindowParams param = new TrainerCardModalWindow.WindowParams(_param.user.userId, _param.showUserProfileOtherButtons, _param.showClubInfoHeaderButtons, _param.onFinishedDissolution, _param.onFinishedSecession);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainerCard, param);
        }
        
    }
}