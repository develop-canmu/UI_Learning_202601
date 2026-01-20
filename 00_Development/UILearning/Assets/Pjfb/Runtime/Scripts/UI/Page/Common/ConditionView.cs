using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Pjfb.ClubMatch;

namespace Pjfb
{
    

    public class ConditionView : MonoBehaviour
    {
        [SerializeField] private bool showBalloon = true;
        [SerializeField] private Animator animator;
        [SerializeField] private UIBadgeBalloon powerUpBalloon;
        [SerializeField] private UIBadgeBalloon powerDownBalloon;
        private ClubConditionData clubConditionData;
        
        
        public async void SetCondition(ClubConditionData conditionData)
        {
            animator.ResetAllTriggers();
            clubConditionData = conditionData;
            if (showBalloon)
            {
                
                string amplifierText = string.Format(StringValueAssetLoader.Instance["club.deck.character_power_amplifier"], conditionData.combatPowerAmplifier.ToRateString());
                switch (conditionData.condition)
                {
                    case ClubDeckCondition.Extreme:
                    case ClubDeckCondition.Best:
                        powerUpBalloon.SetActive(true);
                        powerDownBalloon.SetActive(false);
                        powerUpBalloon.SetText(amplifierText);
                        break;
                    case ClubDeckCondition.NotBad:
                        powerUpBalloon.SetActive(false);
                        powerDownBalloon.SetActive(true);
                        powerDownBalloon.SetText(amplifierText);
                        break;
                    case ClubDeckCondition.Good:
                    case ClubDeckCondition.Awful:
                        powerUpBalloon.SetActive(false);
                        powerDownBalloon.SetActive(false);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            await AnimatorUtility.WaitStateAsync(animator, clubConditionData.animationKey);
        }

        
        
        private async void OnEnable()
        {
            if (clubConditionData == null) return;
            animator.ResetAllTriggers();
            await AnimatorUtility.WaitStateAsync(animator, clubConditionData.animationKey);
        }
        
    }
}