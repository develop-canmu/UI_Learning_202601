using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Networking.App.Request;
using Pjfb.Training;
using TMPro;

namespace Pjfb
{
    public class AutoTrainingSummaryBoostPointView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI extraTurnConvertPoint = null;

        [SerializeField]
        private TextMeshProUGUI conditionBonusConvertPoint = null;

        [SerializeField]
        private TextMeshProUGUI totalBoostPoint = null;

        // 引き直し表示オブジェクト
        [SerializeField]
        private GameObject redrawRoot = null;
        
        [SerializeField]
        private TextMeshProUGUI redrawCount = null;

        [SerializeField]
        private TextMeshProUGUI executeBoostChanceCount = null;
        
        [SerializeField]
        private TextMeshProUGUI extraBonusChoosedCount = null;

        // スペシャルブースト表示オブジェクト
        [SerializeField]
        private GameObject specialBoostRoot = null;
        
        // スペシャルブースト当選回数
        [SerializeField]
        private TextMeshProUGUI specialBoostBonusCount = null;
        
        [SerializeField]
        private TextMeshProUGUI boostChanceLv = null;

        private TrainingMainArguments trainingMainArguments;
        private TrainingAutoResultStatus trainingResultStatus;
        
        public void SetData(TrainingMainArguments mainArguments, TrainingAutoResultStatus resultStatus)
        {
            trainingMainArguments = mainArguments;
            trainingResultStatus = resultStatus;
            
            // シナリオID
            long scenarioId = mainArguments.Pending.mTrainingScenarioId;
            // ターン延長変換
            extraTurnConvertPoint.text = string.Format("{0:#,0}", resultStatus.trainingPointAddedTurnValue);
            // コンディション変換
            conditionBonusConvertPoint.text = string.Format("{0:#,0}", resultStatus.trainingPointConditionValue);
            // トータルのトレーニングポイント
            totalBoostPoint.text = string.Format("{0:#,0}", resultStatus.trainingPointAddedTurnValue + resultStatus.trainingPointConditionValue);

            // 引き直しが有効な場合のみ表示
            bool activeHandReload = TrainingUtility.IsEnableHandReload(scenarioId);
            redrawRoot.SetActive(activeHandReload);
            if (activeHandReload)
            {
                // 手札引き直し
                redrawCount.text = resultStatus.handResetCount.ToString();
            }

            // ブーストチャンス実行回数
            executeBoostChanceCount.text = resultStatus.trainingPointLevelUpCount.ToString();
            // エクストラブースト当選回数
            extraBonusChoosedCount.text = resultStatus.trainingPointStatusAdditionCount.ToString();

            // Spブーストが有効な場合のみ表示
            bool activeSpBoost = TrainingUtility.IsEnableSpBoost(scenarioId);
            specialBoostRoot.SetActive(activeSpBoost);
            if (activeSpBoost)
            {
                // スペシャルブースト当選回数
                specialBoostBonusCount.text = resultStatus.trainingPointStatusEffectCharaCount.ToString();
            }

            // ブーストチャンスレベル
            boostChanceLv.text = resultStatus.trainingPointLevel.ToString();
        }

        public void OnClickBoostStatusDetail()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.AutoTrainingGetBoostList, new AutoTrainingGetBoostListModal.Arguments(trainingMainArguments, trainingResultStatus));
        }
    }
}