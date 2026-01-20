using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace Pjfb.Character
{
    public class PracticeSkillViewData
    {
        public string Content;
        public string Value;

        public PracticeSkillViewData(string content, string value)
        {
            Content = content;
            Value = value;
        }
    }
    
    public class PracticeSkillScrollItem : ScrollGridItem
    {
        [SerializeField] private PracticeSkillView practiceSkillView = null;

        protected override void OnSetView(object value)
        {
            var skillData = (PracticeSkillViewData)value;
            practiceSkillView.SetPracticeSkill(skillData.Content, skillData.Value);
        }
    }
}