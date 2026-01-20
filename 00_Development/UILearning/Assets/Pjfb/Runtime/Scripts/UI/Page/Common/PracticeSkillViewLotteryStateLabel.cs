using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public enum SupportEquipmentLotterySkillStateType
    {
        None, //　表示しない
        OverwriteProhibited, // 上書き不可
        Redrawing, //　再抽選
        RedrawingSubject, //　抽選対象 
    }
    
    public class PracticeSkillViewLotteryStateLabel : MonoBehaviour
    {
        [SerializeField] private GameObject overwriteProhibited;
        [SerializeField] private GameObject redrawing;
        [SerializeField] private GameObject redrawingSubject;
        
        public void SetStateLabel(SupportEquipmentLotterySkillStateType type)
        {
            HideLabel();
            
            switch (type)
            {
                case SupportEquipmentLotterySkillStateType.None:
                    break;
                case SupportEquipmentLotterySkillStateType.OverwriteProhibited:
                    overwriteProhibited.SetActive(true);
                    break;
                case SupportEquipmentLotterySkillStateType.Redrawing:
                    redrawing.SetActive(true);
                    break;
                case SupportEquipmentLotterySkillStateType.RedrawingSubject:
                    redrawingSubject.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public void HideLabel()
        {
            overwriteProhibited.SetActive(false);
            redrawing.SetActive(false);
            redrawingSubject.SetActive(false);
        }
        public bool IsAnyActiveLabel()
        {
            return overwriteProhibited.activeSelf || redrawing.activeSelf || redrawingSubject.activeSelf;
        }
    }
}