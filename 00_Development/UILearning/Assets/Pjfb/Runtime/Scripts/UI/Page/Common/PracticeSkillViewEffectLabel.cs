using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public enum SupportEquipmentLotterySkillEffectType
    {
        None, // 表示しない
        EffectKeep, // 効果量Keep
        EffectUp, // 効果量Up
        EffectDown, // 効果量Down
    }
    
    public class PracticeSkillViewEffectLabel : MonoBehaviour
    {
        [SerializeField] private GameObject effectsKeep;
        [SerializeField] private GameObject effectsUp;
        [SerializeField] private GameObject effectsDown;
        
        public void SetEffectLabel(SupportEquipmentLotterySkillEffectType type)
        {
            HideLabel();
            
            switch (type)
            {
                case SupportEquipmentLotterySkillEffectType.None:
                    break;
                case SupportEquipmentLotterySkillEffectType.EffectKeep:
                    effectsKeep.SetActive(true);
                    break;
                case SupportEquipmentLotterySkillEffectType.EffectUp:
                    effectsUp.SetActive(true);
                    break;
                case SupportEquipmentLotterySkillEffectType.EffectDown:
                    effectsDown.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public void HideLabel()
        {
            effectsKeep.SetActive(false);
            effectsUp.SetActive(false);
            effectsDown.SetActive(false);
        }
        public bool IsAnyActiveLabel()
        {
            return effectsKeep.activeSelf || effectsUp.activeSelf || effectsDown.activeSelf;
        }
    }
}