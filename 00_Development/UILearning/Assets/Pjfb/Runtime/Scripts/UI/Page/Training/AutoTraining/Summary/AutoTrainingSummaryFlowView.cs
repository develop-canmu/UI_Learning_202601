using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.Training
{
    public class AutoTrainingSummaryFlowView : MonoBehaviour
    {
        [SerializeField]
        private AutoTrainingSummaryStatusItemView flowLevelItemView = null;
        
        [SerializeField]
        private AutoTrainingSummaryStatusItemView flowPointItemView = null;
        
        [SerializeField]
        private AutoTrainingSummaryStatusItemView externalTurnConvertFlowPointItemView = null;
        
        [SerializeField]
        private AutoTrainingSummaryStatusItemView conditionBonusConvertFlowPointItemView = null;
        
        [SerializeField]
        private AutoTrainingSummaryStatusItemView flowZoneEnterValueItemView = null;

        /// <summary>フロー関連のステータス表示をセット</summary>
        public void SetData( TrainingAutoResultStatus resultStatus)
        {
            flowLevelItemView.SetValue(resultStatus.concentrationLevel);
            flowPointItemView.SetValue(resultStatus.concentrationExp);
            externalTurnConvertFlowPointItemView.SetValue(resultStatus.trainingPointAddedTurnValue);
            conditionBonusConvertFlowPointItemView.SetValue(resultStatus.trainingPointConditionValue);
            flowZoneEnterValueItemView.SetValue(resultStatus.flowConcentrationCount);
        }
    }
}