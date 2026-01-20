using System.Collections;
using System.Collections.Generic;
using CruFramework.Page;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    
    public class CharacterEventSkillSheet : Sheet
    {
        [SerializeField]
        private CharacterEventSkillScrollView eventSkillView = null;
        /// <summary>View</summary>
        public  CharacterEventSkillScrollView EventSkillView{get{return eventSkillView;}}
    }
}