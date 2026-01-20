using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CruFramework.UI;
using Pjfb.Menu;
using Pjfb.Networking.App.Request;

namespace Pjfb.Club
{
    public class MemberListItem : ScrollGridItem {
        public abstract class OverrideHandler : MonoBehaviour{
            public abstract void UpdateView( MemberListItem.Param param );
        }

        public class Param {
            public ClubUserData userData{get; private set;}

            public Param( ClubUserData data ){
                this.userData =  data;
            }   
        } 

        [SerializeField]
        TextMeshProUGUI _memberName = null;
        [SerializeField]
        TextMeshProUGUI _login = null;
        [SerializeField]
        TextMeshProUGUI _power = null;
        [SerializeField]
        OmissionTextSetter _powerOmissionTextSetter = null;
        [SerializeField]
        TextMeshProUGUI _rankCondition = null;
        [SerializeField]
        TextMeshProUGUI _activityPolicy = null;
        [SerializeField]
        TextMeshProUGUI _clubMatchPolicy = null;
        [SerializeField]
        TextMeshProUGUI _participationPriority = null;
        [SerializeField]
        ClubCharacterIcon _characterIcon = null;
        [SerializeField]
        DeckRankImage _rank = null;
        [SerializeField]
        UserTitleImage _emblem = null;
        [SerializeField]
        TextMeshProUGUI _comment = null;
        [SerializeField]
        OverrideHandler _handler = null;

        Param _param = null;

        protected override void OnSetView(object value){
            var param = (Param)value;
            _param = param;
            UpdateView(param);
        }

        public void UpdateView( Param param ){
            _memberName.text = param.userData.name;
            _comment.text = param.userData.message;
            _characterIcon.UpdateIcon(param.userData.iconId);
            _emblem.SetTexture(param.userData.emblemId);
            _rank.SetTexture(param.userData.deckRank);
            _power.text = param.userData.power.ToDisplayString(_powerOmissionTextSetter.GetOmissionData());
            _login.text = ClubUtility.CreateLastLoginText(param.userData.lastLogin);

            var format = StringValueAssetLoader.Instance["club.find_setting_upper_rank"];
            _rankCondition.text = StringValueAssetLoader.Instance["club.noneConditions"];
            foreach( var master in Master.MasterManager.Instance.guildRankMaster.values ){
                if( master.id == param.userData.rankCondition ) {
                    _rankCondition.text = string.Format(format, master.name);
                    break;
                }
            }
            

            _activityPolicy.text = ClubUtility.FindActivityPolicyStrings(param.userData.activityPolicy);

            var clubMatchPolicyStrings = ClubUtility.CreateMatchPolicyStrings();
            _clubMatchPolicy.text = clubMatchPolicyStrings[(int)param.userData.clubMatchPolicy];

            _participationPriority.text = ClubUtility.GetParticipationPriorityData(param.userData.guildParticipationPriorityType).name;

            _handler?.UpdateView(param);
            
        }

        public void OpenProfile()
        {
            TrainerCardModalWindow.WindowParams param = new TrainerCardModalWindow.WindowParams(_param.userData.userId);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.TrainerCard, param);
        }

    }
}