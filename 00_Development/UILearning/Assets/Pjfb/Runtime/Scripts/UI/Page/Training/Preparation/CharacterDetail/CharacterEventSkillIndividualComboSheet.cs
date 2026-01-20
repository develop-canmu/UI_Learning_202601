using System.Collections;
using System.Collections.Generic;
using CruFramework.Page;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    
    public class CharacterEventSkillIndividualComboSheet : CharacterEventSkillSheet
    {
        protected override UniTask OnOpen(object args)
        {
            EventSkillView.UpdateList( CharacterEventSkillViewType.IndividualCombo );
            return base.OnOpen(args);
        }
    }
}