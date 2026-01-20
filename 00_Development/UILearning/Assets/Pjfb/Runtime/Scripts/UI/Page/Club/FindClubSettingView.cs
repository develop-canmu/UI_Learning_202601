using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CruFramework.Page;
using Pjfb.Master;
using Pjfb.Networking.App.Request;

namespace Pjfb.Club {
    public class FindClubSettingView : MonoBehaviour {
        public class FindParam{
            public string clubName {get; set;} 
            public int rankLower {get; set;} 
            public int rankUpper{get; set;} 
            public int memberLower{get; set;} 
            public int memberUpper{get; set;} 
            public int activityPolicy{get; set;} 
            public int isAutoEnrollment{get; set;} 
            public int memberRecruitmentStatus{get; set;} 
            public int clubMatchPolicy{get; set;} 
            public long participationPriority{get; set;}

        }
        
        public System.Action onClickFindButton{get;set;} = null;


        [SerializeField]
        ClubInputField _clubName = null;
        [SerializeField]
        TMP_Dropdown _rankLower = null;
        [SerializeField]
        TMP_Dropdown _rankUpper = null;
        [SerializeField]
        TMP_Dropdown _memberLower = null;
        [SerializeField]
        TMP_Dropdown _memberUpper = null;
        [SerializeField]
        TMP_Dropdown _activityPolicy = null;
        [SerializeField]
        TMP_Dropdown _joinConditions = null;
        [SerializeField]
        TMP_Dropdown _memberRecruitmentStatus = null;
        [SerializeField]
        TMP_Dropdown _clubMatchPolicy = null;
        [SerializeField]
        TMP_Dropdown _participationPriority = null;

        // 優先度順にオプションを格納するので対応付け用
        private ConfGuildSearchParticipationPriorityData[] participationPrioritySortedOptions;

        public void OnClickFindButton(){
            onClickFindButton?.Invoke();
        }

        public FindParam CreateFindParam( ){
            var param = new FindParam();
            param.clubName = _clubName.text;
            param.rankLower = _rankLower.value;
            param.rankUpper = _rankUpper.value;
            param.memberLower = _memberLower.value;
            param.memberUpper = _memberUpper.value;
            param.activityPolicy = ClubUtility.ActivityPolicyStringsIndexToId(_activityPolicy.value);
            param.isAutoEnrollment = _joinConditions.value;
            param.memberRecruitmentStatus = _memberRecruitmentStatus.value;
            param.clubMatchPolicy = _clubMatchPolicy.value;
            param.participationPriority = GetSelectedParticipantPriorityId(_participationPriority.value);
            return param;
        }


        void Start() {
            InitUI();
        }


        void InitUI() {
            //rank
            var rankOptions = new List<TMP_Dropdown.OptionData>();
            foreach( var rank in MasterManager.Instance.guildRankMaster.values ){
                rankOptions.Add(new TMP_Dropdown.OptionData(rank.name));
            }
            var rankLowerOptions = new List<TMP_Dropdown.OptionData>();
            rankLowerOptions.Add(new TMP_Dropdown.OptionData(StringValueAssetLoader.Instance["club.noneLowerLimit"]));
            rankLowerOptions.AddRange(rankOptions);
            _rankLower.onValueChanged.AddListener(OnChangedRankLower);
            _rankLower.options = rankLowerOptions;

            var rankUpperOptions = new List<TMP_Dropdown.OptionData>();
            rankUpperOptions.Add(new TMP_Dropdown.OptionData(StringValueAssetLoader.Instance["club.noneUpperLimit"]));
            rankUpperOptions.AddRange(rankOptions);
            _rankUpper.onValueChanged.AddListener(OnChangedRankUpper);
            _rankUpper.options = rankUpperOptions;

            //人数
            var memberOptions = new List<TMP_Dropdown.OptionData>();
            for(int i=0; i<ConfigManager.Instance.maxGuildMemberCount; ++i){
                memberOptions.Add(new TMP_Dropdown.OptionData((i+1).ToString()));
            }

            var memberLowerOptions = new List<TMP_Dropdown.OptionData>();
            memberLowerOptions.Add(new TMP_Dropdown.OptionData(StringValueAssetLoader.Instance["club.noneLowerLimit"]));
            memberLowerOptions.AddRange(memberOptions);
            _memberLower.options = memberLowerOptions;
            _memberLower.onValueChanged.AddListener(OnChangedMemberLower);

            var memberUpperOptions = new List<TMP_Dropdown.OptionData>();
            memberUpperOptions.Add(new TMP_Dropdown.OptionData(StringValueAssetLoader.Instance["club.noneUpperLimit"]));
            memberUpperOptions.AddRange(memberOptions);
            _memberUpper.options = memberUpperOptions;
            _memberUpper.onValueChanged.AddListener(OnChangedMemberUpper);
            
            //活動方針
            var activityStrings = ClubUtility.CreateActivityPolicyStrings();
            var activityOptions = new List<TMP_Dropdown.OptionData>();
            foreach( var str in activityStrings ){
                activityOptions.Add(new TMP_Dropdown.OptionData(str));
            }
            _activityPolicy.options = activityOptions;

            //入団条件
            var autoEnrollmentStrings = ClubUtility.CreateAutoEnrollmentStrings();
            var autoEnrollmentOptions = new List<TMP_Dropdown.OptionData>();
            foreach( var str in autoEnrollmentStrings ){
                autoEnrollmentOptions.Add(new TMP_Dropdown.OptionData(str));
            }
            _joinConditions.options = autoEnrollmentOptions;

            //募集状況
            var memberRecruitmentStatusStrings = ClubUtility.CreateMemberRecruitmentStatusStrings();
            var memberRecruitmentStatusOptions = new List<TMP_Dropdown.OptionData>();
            foreach( var str in memberRecruitmentStatusStrings ){
                memberRecruitmentStatusOptions.Add(new TMP_Dropdown.OptionData(str));
            }
            _memberRecruitmentStatus.options = memberRecruitmentStatusOptions;

            //マッチ方針
            var clubMatchPolicyStrings = ClubUtility.CreateMatchPolicyStrings();
            var clubMatchPolicyOptions = new List<TMP_Dropdown.OptionData>();
            foreach( var str in clubMatchPolicyStrings ){
                clubMatchPolicyOptions.Add(new TMP_Dropdown.OptionData(str));
            }
            _clubMatchPolicy.options = clubMatchPolicyOptions;

            // 参加優先度
            List<TMP_Dropdown.OptionData> participationPriorityOptions = new List<TMP_Dropdown.OptionData>();
            participationPrioritySortedOptions = new ConfGuildSearchParticipationPriorityData[ConfigManager.Instance.guildSearchParticipationPriorityTypeList.Length];
            int participationPriorityIndex = 0;
            foreach (ConfGuildSearchParticipationPriorityData data in ConfigManager.Instance.guildSearchParticipationPriorityTypeList.OrderByDescending(data => data.priority))
            {
                participationPriorityOptions.Add(new TMP_Dropdown.OptionData(data.name));
                // 対応付け用にオプションの順番データをキャッシュ
                participationPrioritySortedOptions[participationPriorityIndex] = data;
                
                participationPriorityIndex++;
            }
            _participationPriority.options = participationPriorityOptions;
        }

        void OnChangedMemberLower( int index ){
            if( _memberUpper.value == 0 || _memberLower.value == 0 ){
                return;
            }

            if( _memberLower.value > _memberUpper.value ){
                _memberUpper.value = _memberLower.value;
            }
        }

        void OnChangedMemberUpper( int index ){
            if( _memberUpper.value == 0 || _memberLower.value == 0 ){
                return;
            }


            if( _memberLower.value > _memberUpper.value ){
                _memberLower.value = _memberUpper.value;
            }
        }

        void OnChangedRankLower( int index ){
            if( _rankUpper.value == 0 || _rankLower.value == 0 ){
                return;
            }

            if( _rankLower.value > _rankUpper.value ){
                _rankUpper.value = _rankLower.value;
            }
        }

        void OnChangedRankUpper( int index ){
            if( _rankUpper.value == 0 || _rankLower.value == 0 ){
                return;
            }

            if( _rankLower.value > _rankUpper.value ){
                _rankLower.value = _rankUpper.value;
            }
        }

        /// <summary> 優先度制御のためIDを引っ張る </summary>
        private long GetSelectedParticipantPriorityId(int index)
        {
            return participationPrioritySortedOptions[index].id;
        }
        
        
    }
}