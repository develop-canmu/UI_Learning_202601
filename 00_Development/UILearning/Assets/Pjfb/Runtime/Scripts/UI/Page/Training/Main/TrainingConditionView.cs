using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Pjfb.Training
{
    public class TrainingConditionView : MonoBehaviour
    {
        
        public static readonly string OpenAnimation = "Open";
        public static readonly string CloseAnimation = "Close";
        public static readonly string IdleAnimation = "Idle";
        
        [SerializeField]
        private Animator[] effects = null;
        
        private Animator currentEffect = null;
        
        private long scenarioId = -1;
        private long condition = -1;
        private long conditionType = -1;
        
        /// <summary>コンディションの設定</summary>
        public void SetCondition(long currentScenarioId, long current, long type)
        {
            if(gameObject.activeInHierarchy)
            {
                SetConditionAsync(currentScenarioId, current, type).Forget();
            }
            else
            {
                this.scenarioId = currentScenarioId;
                condition = current;
                conditionType = type;
            }
        }

        private void OnEnable()
        {
            if(condition >= 0)
            {
                SetConditionAsync(scenarioId, condition, conditionType).Forget();
                condition = -1;
            }
        }

        /// <summary>コンディションの設定</summary>
        public async UniTask SetConditionAsync(long currentScenarioId, long current, long type) 
        {
            TrainingConditionStateData conditionData = TrainingUtility.GetConditionState(currentScenarioId, current, type);
                        
            if(currentEffect == effects[conditionData.EffectIndex])
            {
                currentEffect.SetTrigger(IdleAnimation);
            }
            else if(currentEffect != effects[conditionData.EffectIndex])
            {
                if(currentEffect != null)
                {
                    await AnimatorUtility.WaitStateAsync(currentEffect, CloseAnimation);
                }
            
                currentEffect = effects[conditionData.EffectIndex];
                await AnimatorUtility.WaitStateAsync(currentEffect, OpenAnimation);
            }

        }
    }
}