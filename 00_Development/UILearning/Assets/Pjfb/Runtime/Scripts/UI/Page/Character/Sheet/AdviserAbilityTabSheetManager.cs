using System.Collections;
using System.Collections.Generic;
using CruFramework.Page;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Character
{
    public enum AdviserAbilityTabSheetType
    {
        // 練習能力
        PracticeSkill,
        // エールスキル
        YellSkill,
        // サポートスキル
        SupportSkill,
    }
    
    public class AdviserAbilityTabSheetManager : SheetManager<AdviserAbilityTabSheetType>
    {
        
    }
}