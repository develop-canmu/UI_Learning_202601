using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;

namespace Pjfb.Club
{

    public class CreateClubConfirmModal : ModalWindow
    {
        public class Param {
            public EditClubDate editData = null;
            public Action<EditClubDate> onClickOk = null;
        }

        [SerializeField]
        Image _emblem = null;
        [SerializeField]
        TextMeshProUGUI _clubName = null;
        [SerializeField]
        TextMeshProUGUI _introduction = null;
        [SerializeField]
        TextMeshProUGUI _activityPolicy = null;
        [SerializeField]
        TextMeshProUGUI _recruitmentStatus = null;
        [SerializeField]
        TextMeshProUGUI _recruitmentComment = null;
        [SerializeField]
        TextMeshProUGUI _clubMatchPolicy = null;
        [SerializeField]
        TextMeshProUGUI _joinConditions = null;
        [SerializeField]
        TextMeshProUGUI _participationPriority = null;

        Param _param = null;
        

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            _param = (Param)args;
            await UpdateView(_param.editData);
            await base.OnPreOpen(args, token);
        }

        
        public void OnClickCancelButton() {
            Close();
        }

        public void OnClickOkButton() {
             _param.onClickOk?.Invoke( _param.editData );
             Close();
        }


        async UniTask UpdateView( EditClubDate data ){
            await ClubUtility.LoadAndSetEmblemIcon(_emblem, data.emblemId);

            _clubName.text = data.name;
            _introduction.text = data.introduction;
            
            
            _activityPolicy.text = ClubUtility.FindActivityPolicyStrings(data.activityPolicyId);

            var recruitmentStatusStrings = ClubUtility.CreateMemberRecruitmentStatusStrings();
            _recruitmentStatus.text = recruitmentStatusStrings[(int)data.recruitmentStatus];
            _recruitmentComment.text = data.recruitmentComment;
            var matchPolicyStrings = ClubUtility.CreateMatchPolicyStrings();
            _clubMatchPolicy.text = matchPolicyStrings[(int)data.clubMatchPolicy];
            var autoEnrollmentStrings = ClubUtility.CreateAutoEnrollmentStrings();
            _joinConditions.text = autoEnrollmentStrings[(int)data.isAutoEnrollment];
            _participationPriority.text = ClubUtility.GetParticipationPriorityData(data.participationPriority).name;
        }

    }
}
