using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.UI;
using TMPro;
using Pjfb.Networking.App.Request;
using Cysharp.Threading.Tasks;

namespace Pjfb.Club
{
    public class ClubListItem : ScrollGridItem {

        public class Param {
            public long gMasterId {get; set;} 
            public string name {get; set;} 
            public BigValue totalPower {get; set;} 
            public long rank {get; set;} 
            public long emblemIconId {get; set;} 
            public long memberQuantity{get; set;} 
            public long activityPolicy{get; set;} 
            public long isAutoEnrollment{get; set;} 
            public long clubMatchPolicy{get; set;} 
            public string comment {get; set;} 
            public long guildMasterMIconId{get; set;}
            public long membersWantedFlg{get; set;}// メンバー募集フラグ
            public long participationPriority { get; }
            public Param(){

            }
            public Param( GuildSearchGuildStatus status ){
                this.gMasterId = status.gMasterId;
                this.name = status.name;
                this.totalPower = new BigValue(status.combatPower);
                this.rank = status.mGuildRankId;
                this.emblemIconId = status.mGuildEmblemId;
                this.memberQuantity = status.numberOfPeople;
                this.activityPolicy = status.mGuildPlayStyleId;
                this.isAutoEnrollment = status.autoEnrollmentFlg;
                this.clubMatchPolicy = status.guildBattleStyleType;
                this.comment = status.membersWantedComment;
                this.guildMasterMIconId = status.guildMasterMIconId;
                this.membersWantedFlg = status.membersWantedFlg;
                this.participationPriority = status.participationPriorityType;
            }

            public Param( GuildInvitationInvitationStatus status ){
                this.gMasterId = status.gMasterId;
                this.name = status.name;
                this.totalPower = new BigValue(status.combatPower);
                this.rank = status.mGuildRankId;
                this.emblemIconId = status.mGuildEmblemId;
                this.memberQuantity = status.numberOfPeople;
                this.activityPolicy = status.mGuildPlayStyleId;
                this.isAutoEnrollment = status.autoEnrollmentFlg;
                this.guildMasterMIconId = status.guildMasterMIconId;
                this.clubMatchPolicy = status.guildBattleStyleType;
                this.participationPriority = status.participationPriorityType;
            }

            
        }

        [SerializeField]
        TextMeshProUGUI _clubName = null;
        [SerializeField]
        TextMeshProUGUI _totalPower = null;
        [SerializeField]
        OmissionTextSetter _totalPowerOmissionTextSetter = null;
        [SerializeField]
        Image _rank = null;
        [SerializeField]
        Image _emblem = null;
        [SerializeField]
        TextMeshProUGUI _memberQuantity = null;
        [SerializeField]
        TextMeshProUGUI _activityPolicy = null;
        [SerializeField]
        TextMeshProUGUI _joinConditions = null;
        [SerializeField]
        TextMeshProUGUI _clubMatchPolicy = null;
        [SerializeField]
        TextMeshProUGUI _participationPriority = null;
        [SerializeField]
        TextMeshProUGUI _comment = null;

        [SerializeField]
        ClubCharacterIcon _characterIcon = null;



        protected Param _param = null;
        protected override void OnSetView(object value){
            _param = (Param)value;
            UpdateView(_param).Forget();
        }

        protected virtual async UniTask UpdateView(Param param){
            _clubName.text = param.name;
            _totalPower.text = param.totalPower.ToDisplayString(_totalPowerOmissionTextSetter.GetOmissionData());
            _memberQuantity.text = ClubUtility.CreateMemberQuantityString( param.memberQuantity );
            _comment.text = param.comment;
            
            _activityPolicy.text = ClubUtility.FindActivityPolicyStrings(param.activityPolicy);

            var autoEnrollmentStrings = ClubUtility.CreateAutoEnrollmentStrings();
            _joinConditions.text = autoEnrollmentStrings[(int)param.isAutoEnrollment];

            var clubMatchPolicyString = ClubUtility.CreateMatchPolicyStrings();
            _clubMatchPolicy.text = clubMatchPolicyString[(int)param.clubMatchPolicy];

            _participationPriority.text = ClubUtility.GetParticipationPriorityData(param.participationPriority).name;

            _characterIcon.UpdateIcon( param.guildMasterMIconId );

            _emblem?.gameObject.SetActive(false);
            _rank?.gameObject.SetActive(false);
            await ClubUtility.LoadAndSetEmblemIcon(_emblem, param.emblemIconId);
            await ClubUtility.LoadAndSetRankIcon(_rank, param.rank);
            if( _emblem != null && _emblem.gameObject != null ) {
                _emblem.gameObject.SetActive(true);
            }
            if( _rank != null && _rank.gameObject != null ) {
                _rank.gameObject.SetActive(true);
            }
            
            
        }
    }
}