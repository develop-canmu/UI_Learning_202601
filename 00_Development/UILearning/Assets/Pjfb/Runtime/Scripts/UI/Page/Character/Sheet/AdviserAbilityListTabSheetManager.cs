using System.Collections;
using System.Collections.Generic;
using CruFramework.Page;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Character
{
    public enum AdviserAbilityListTabSheetType
    {
        // エールスキル
        YellSkill = 0,
        // サポートスキル
        SupportSkill = 1,
    }
    
    public class AdviserAbilityListTabSheetManager : SheetManager<AdviserAbilityListTabSheetType>
    {
        
    }
}