using System;
using Pjfb.Common;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using UnityEngine;
using TMPro;

namespace Pjfb
{
    public class HomeBeginnerMissionButton : MonoBehaviour
    {
        #region Params
        public class Parameters
        {
            public MissionCategoryStatus beginnerMissionCategoryStatus;
            public Action<Parameters> onClick;

            public DailyMissionMasterObject beginnerMissionObject;

            public bool hasActiveBeginnerMission => beginnerMissionCategoryStatus != null && beginnerMissionObject != null;
            public Parameters(MissionCategoryStatus beginnerMissionCategoryStatus, Action<Parameters> onClick = null)
            {
                this.beginnerMissionCategoryStatus = beginnerMissionCategoryStatus;
                this.onClick = onClick;
                
                var mDailyMissionId = beginnerMissionCategoryStatus?.targetMission.mDailyMissionId ?? 0;
                this.beginnerMissionObject = mDailyMissionId > 0 ? MasterManager.Instance.dailyMissionMaster.FindData(mDailyMissionId) : null;
            }
        }
        #endregion
        
        #region SerializeFields
        [SerializeField] private TextMeshProUGUI descriptionText = null;
        [SerializeField] private GameObject badgeClearGameObject;
        #endregion

        #region PrivateFields
        private Parameters _parameters = null;
        
        #endregion

        #region PublicMethods
        public void Init(Parameters parameters)
        {
            if (parameters.hasActiveBeginnerMission) {
                _parameters = parameters;
                descriptionText.text = _parameters.beginnerMissionObject.description;
                badgeClearGameObject.SetActive(parameters.beginnerMissionCategoryStatus.targetMission.progressStatus == (long)MissionProgressStatus.ReceivingReward);
                ShowButton(isActive: true);
            } else ShowButton(isActive: false);
        }
        #endregion

        #region PrivateMethods
        private void ShowButton(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
        #endregion

        #region EventListener
        public void OnClickButton()
        {
            _parameters?.onClick?.Invoke(_parameters);
        }
        #endregion
    }
    
}