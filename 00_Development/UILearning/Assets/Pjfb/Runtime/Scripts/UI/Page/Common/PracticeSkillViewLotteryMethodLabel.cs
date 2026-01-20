using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public enum SupportEquipmentLotterySkillMethodType
    {
        None, // 表示しない
        LotteryProbability, // 抽選確率Up
        Decision, // 練習能力確定
    }

    public class PracticeSkillViewLotteryMethodLabel : MonoBehaviour

    {
        [SerializeField] private GameObject lotteryProbabilityUp;
        [SerializeField] private TMPro.TMP_Text lotteryProbabilityUpText;
        [SerializeField] private GameObject decision;
        [SerializeField] private TMPro.TMP_Text decisionText;

        public void SetLotteryMethodLabel(SupportEquipmentLotterySkillMethodType type, string text)
        {
            HideLabel();
            
            switch (type)
            {
                case SupportEquipmentLotterySkillMethodType.None:
                    break;
                case SupportEquipmentLotterySkillMethodType.LotteryProbability:
                    lotteryProbabilityUp.SetActive(true);
                    lotteryProbabilityUpText.text = text;
                    break;
                case SupportEquipmentLotterySkillMethodType.Decision:
                    decision.SetActive(true);
                    decisionText.text = text;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public void HideLabel()
        {
            lotteryProbabilityUp.SetActive(false);
            decision.SetActive(false);
        }
        public bool IsAnyActiveLabel()
        {
            return lotteryProbabilityUp.activeSelf || decision.activeSelf;
        }
    }
}