using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using UnityEngine.UI;

namespace Pjfb
{
    public class TrainingPracticeSkillView : ScrollGridItem
    {
        
        public enum ViewType
        {
            Character, PracticeSkill
        }

        public class ViewData
        {
            private PracticeSkillInfo skillInfo = default;
            /// <summary>Skill</summary>
            public PracticeSkillInfo SkillInfo{get{return skillInfo;}}
            
            private bool isSelected = false;
            /// <summary>選択中</summary>
            public bool IsSelected{get{return isSelected;}}

            private long mCharaId = -1;
            /// <summary>mCharId</summary>
            public long MCharaId{get{return mCharaId;}}
            
            private long uCharId = -1;
            /// <summary>Id</summary>
            public long UCharId{get{return uCharId;}}
            
            private int index = -1;
            /// <summary>Id</summary>
            public int Index{get{return index;}}
            
            private bool isSpecialAttack = false;
            /// <summary>特攻？</summary>
            public bool IsSpecialAttack{get{return isSpecialAttack;}}

            private long characterLevel;
            /// <summary>解放レベル </summary>
            public long CharacterLevel{get{return characterLevel;}}
            
            private long liberationLevel;
            /// <summary>解放レベル </summary>
            public long LiberationLevel{get{return liberationLevel;}}

            private long[] statusIdList;
            /// <summary>StatusIdList</summary>
            public long[] StatusIdList {get{return statusIdList; }}
            
            private bool canGrowth = false;
            /// <summary>レベル上げ出来るか</summary>
            public bool CanGrowth{get{return canGrowth;}}
            
            public ViewData(PracticeSkillInfo skillInfo, bool isSelected, long mCharaId, long uCharId, int index, bool isSpecialAttack, long characterLevel, long liberationLevel, long[] statusIdList, bool canGrowth)
            {
                this.skillInfo = skillInfo;
                this.isSelected = isSelected;
                this.mCharaId = mCharaId;
                this.uCharId = uCharId;
                this.index = index;
                this.isSpecialAttack = isSpecialAttack;
                this.characterLevel = characterLevel;
                this.liberationLevel = liberationLevel;
                this.statusIdList = statusIdList;
                this.canGrowth = canGrowth;
            }
        }
        
        [SerializeField]
        private ViewType viewType = ViewType.Character;

        [SerializeField]
        private PracticeSkillView view = null;
        
        [SerializeField]
        private GameObject selectedRoot = null;
        
        [SerializeField]
        private Image baseImage = null;

        protected override void OnSetView(object value)
        {
            ViewData data = (ViewData)value;
            // 選択中表示
            baseImage.color = data.IsSelected ? ColorValueAssetLoader.Instance["training.practice_skill.selected"] : Color.white;
            selectedRoot.SetActive(data.IsSelected);
            // 特攻表示
            view.SetSpecialAttackEnable(data.IsSpecialAttack);
            
            switch(viewType)
            {
                case ViewType.Character:
                {
                    view.SetSkillData(data.SkillInfo, data.MCharaId, data.UCharId, data.CharacterLevel, data.LiberationLevel, data.StatusIdList, data.CanGrowth);
                    break;
                }
                
                case ViewType.PracticeSkill:
                {
                    view.SetIcon(data.MCharaId);
                    view.SetNameValue(data.SkillInfo);
                    view.SetDescription(data.SkillInfo.GetDescription());
                    break;
                }
            }
        }
    }
}