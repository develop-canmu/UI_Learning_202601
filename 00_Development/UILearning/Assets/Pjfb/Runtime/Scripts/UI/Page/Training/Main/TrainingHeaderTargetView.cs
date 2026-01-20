using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.Networking.App.Request;

namespace Pjfb.Training
{
    public class TrainingHeaderTargetView : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TMP_Text descriptionText = null;
        [SerializeField]
        private TMPro.TMP_Text conditionText = null;
        
        [SerializeField]
        private GameObject conditionRoot = null;
        
        public void SetTarget(TrainingMainArguments args)
        {
            // 現在の目標
            TrainingGoal target = args.CurrentTarget;
            if(target == null)
            {
                descriptionText.text = string.Empty;
                conditionRoot.SetActive(false);
            }
            else
            {
                descriptionText.text = target.goalDescription;
                // 条件
                CharaVariableConditionElementMasterObject mElement = MasterManager.Instance.charaVariableConditionElementMaster.FinDataByCharaVariableConditionId(target.mCharaVariableConditionId);
                // 条件なし
                if(mElement == null)
                {
                    conditionRoot.SetActive(false);
                }
                else
                {
                    conditionRoot.SetActive(true);
                    // 条件クリア
                    if(args.CurrentTipCount >= mElement.value)
                    {
                        conditionText.text = StringValueAssetLoader.Instance["training.target_condition_clear"];
                    }
                    else
                    {
                        conditionText.text = string.Format( StringValueAssetLoader.Instance["training.target_condition"], mElement.value );
                    }
                    
                }
                
            }
            
        }
    }
}