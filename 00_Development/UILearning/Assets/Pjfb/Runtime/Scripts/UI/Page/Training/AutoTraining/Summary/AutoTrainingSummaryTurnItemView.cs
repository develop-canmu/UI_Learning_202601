using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using TMPro;

namespace Pjfb.Training
{
    public class AutoTrainingSummaryTurnItemView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text nameText = null;
        [SerializeField]
        private TMP_Text turnText = null;
        [SerializeField]
        private TMP_Text extraTurnText = null;
        
        /// <summary>結果表示</summary>
        public void SetData(int index, ResultTurn result)
        {
            // マスタ
            TrainingEventMasterObject mEvent = MasterManager.Instance.trainingEventMaster.FindData(result.mTrainingEventId);
            
            SetData(string.Format(StringValueAssetLoader.Instance["auto_training.summary.turn_menu"], index, mEvent.name), result.turnMax, result.turnAddValue);
        }
        
        public void SetData(string name, long turn, long extraTurn)
        {
            // 名前
            nameText.text = name;
            // ターン
            turnText.text = turn.ToString();
            // 延長ターン
            if(extraTurn > 0)
            {
                extraTurnText.gameObject.SetActive(true);
                extraTurnText.text = extraTurn.ToString();  
            }
            else
            {
                extraTurnText.gameObject.SetActive(false);
            }
        }
    }
}