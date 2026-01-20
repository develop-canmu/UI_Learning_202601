using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pjfb.Training
{
    
    public enum TrainingPracticeSkillModalSheetType
    {
        SupportCharacter = 0, 
        SpecialSupportCharacter = 1, 
        SupportEquipment = 2, 
        Adviser = 3,
        TotalPracticeSkill = 4,
    }
    
    public class TrainingPracticeSkillModalSheetManager : SheetManager<TrainingPracticeSkillModalSheetType>
    {
    }
}