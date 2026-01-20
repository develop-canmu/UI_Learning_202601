using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using TMPro;
using CruFramework.UI;
using Pjfb.Menu;
using Pjfb.Networking.App.Request;

namespace Pjfb.Club {

    public class EditClubDate {
        public string name{get;set;} = "";
        public string introduction{get;set;} = "";
        public long emblemId{get;set;} = 1;
        public long activityPolicyId{get;set;} = 1;
        public long recruitmentStatus{get;set;} = 1;
        public string recruitmentComment{get;set;} = "";
        public ClubEntryPermissionType isAutoEnrollment{get;set;} = ClubEntryPermissionType.AutoApprove;
        public long clubMatchPolicy{get;set;} = 1;
        public long participationPriority {get;set;}
        public EditClubDate(){

        }

        public EditClubDate( ClubData data ){
            name = data.name;           
            introduction = data.introduction;
            emblemId = data.emblemId;
            activityPolicyId = data.activityPolicyId;
            recruitmentComment = data.recruitmentComment;
            isAutoEnrollment = data.isAutoEnrollment;
            clubMatchPolicy = data.clubMatchPolicy;
            recruitmentStatus = data.recruitmentStatus;
            participationPriority = data.participationPriority;
        }

        public bool Equals( EditClubDate data ){
            if( this.name != data.name ) {
                return false;
            }
            if( this.introduction != data.introduction ) {
                return false;
            }
            if( this.emblemId != data.emblemId ) {
                return false;
            }
            if( this.activityPolicyId != data.activityPolicyId ) {
                return false;
            }
            if( this.recruitmentStatus != data.recruitmentStatus ) {
                return false;
            }
            if( this.recruitmentComment != data.recruitmentComment ) {
                return false;
            }
            if( this.isAutoEnrollment != data.isAutoEnrollment ) {
                return false;
            }
            if( this.clubMatchPolicy != data.clubMatchPolicy ) {
                return false;
            }
            if( this.participationPriority != data.participationPriority ) {
                return false;
            }
          
            return true;
        }

    }


    public class EditClubView : MonoBehaviour, ISelectEmblemHandler {
        
        public long emblemIconId => _emblemIconId;
        public string clubName => _clubName.text;
        public string introduction => _introduction.text;
        
        [SerializeField]
        Image _emblem = null;
        [SerializeField]
        ClubInputField _clubName = null;
        [SerializeField]
        ClubInputField _introduction = null;
        [SerializeField]
        ToggleGroup _activityPolicy = null;
        [SerializeField]
        ToggleGroup _recruitmentStatus = null;
        [SerializeField]
        ClubInputField _recruitmentComment = null;
        [SerializeField]
        ToggleGroup _clubMatchPolicy = null;
        [SerializeField]
        ToggleGroup _joinConditions = null;
        [SerializeField]
        UIButton _updateButton = null;

        [SerializeField]
        ClubToggle _participationPriorityObj = null;
        [SerializeField]
        Transform _participationPriorityParent = null;

        private List<ClubToggle> _participationPriorityToggleList = new ();

        long _emblemIconId = 0;

        long[] _selectIdList = null;

        EditClubDate _prevEditData = null;

        public async UniTask Init( EditClubDate data, long[] selectIdList ) {
            _selectIdList = selectIdList;
            _prevEditData = data;
            CreateParticipationPriorityToggleList(data.participationPriority);
            await UpdateView(data);
            CheckButtonActive();
        }

        public void OnClickEmblem(){
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubSelectEmblem, this);
        }
        

        public EditClubDate CreateEditData(){
            var data = new EditClubDate();
            data.name = _clubName.text;
            data.emblemId = _emblemIconId;
            data.introduction = _introduction.text;
            data.activityPolicyId = ClubUtility.GetToggleParam(_activityPolicy);
            data.recruitmentStatus = ClubUtility.GetToggleParam(_recruitmentStatus);
            data.recruitmentComment = _recruitmentComment.text;
            data.isAutoEnrollment = (ClubEntryPermissionType) ClubUtility.GetToggleParam(_joinConditions);
            data.clubMatchPolicy = ClubUtility.GetToggleParam(_clubMatchPolicy);
            // オンになってるやつを取得
            data.participationPriority = _participationPriorityToggleList.First(t => t.toggleObj.toggle.isOn).toggleObj.type;
            return data;
        }

        public async UniTask UpdateView( EditClubDate data ){
            _clubName.text = data.name;
            _emblemIconId = data.emblemId;
            await ClubUtility.LoadAndSetEmblemIcon(_emblem, _emblemIconId);
            _introduction.text = data.introduction;
            ClubUtility.SetActiveToggle(_activityPolicy, data.activityPolicyId);
            ClubUtility.SetActiveToggle(_recruitmentStatus, data.recruitmentStatus);
             _recruitmentComment.text = data.recruitmentComment;
            ClubUtility.SetActiveToggle(_joinConditions, (long)data.isAutoEnrollment);
            ClubUtility.SetActiveToggle(_clubMatchPolicy, data.clubMatchPolicy);
        }

        public void CheckButtonActive() {
            if( _updateButton == null ) {
                return;
            }
            var data = CreateEditData();
            _updateButton.interactable = !string.IsNullOrEmpty(_introduction.text) && !_prevEditData.Equals(data);
        }


        void ISelectEmblemHandler.SetEmblem(Sprite sprite, long id){
            _emblem.sprite = sprite;
            _emblemIconId = id;
            CheckButtonActive();
        }

        long[] ISelectEmblemHandler.CreateSelectIdList(){
            return _selectIdList;
        }

        /// <summary> トグル生成 </summary>
        private void CreateParticipationPriorityToggleList(long currentId)
        {
            // すでに生成済みなら
            if (_participationPriorityToggleList.Count > 0) return;
            
            foreach (ConfGuildSearchParticipationPriorityData priorityData in ConfigManager.Instance.guildSearchParticipationPriorityTypeList.OrderByDescending(t => t.priority))
            {
                ClubToggle toggleObj = Instantiate(_participationPriorityObj, _participationPriorityParent);
                toggleObj.toggleObj.type = priorityData.id;
                toggleObj.label.text = priorityData.name;
                toggleObj.toggleObj.toggle.gameObject.SetActive(true);
                // 現在設定されているものを選択状態にする
                toggleObj.toggleObj.toggle.SetIsOnWithoutNotify(priorityData.id == currentId);
                _participationPriorityToggleList.Add(toggleObj);
            }
        }
    }
}