using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CruFramework.UI;
using Pjfb;

namespace MyNamespace
{
    public class CharacterSkillViewItem : ScrollGridItem
    {
        [SerializeField]
        private CharacterDetailSkillView skillView = null;
        
        protected override void OnSetView(object value)
        {
            skillView.SetSkill( (SkillData)value );
        }
    }
}