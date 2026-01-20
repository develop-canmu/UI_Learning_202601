using CruFramework.UI;
using UnityEngine;

namespace Pjfb.Character
{
    public class AdviserSkillCharacterScrollGridItem : ScrollGridItem
    {
        public class ItemParam
        {
            private CharacterDetailData userDataChara;
            public CharacterDetailData UserDataChara => userDataChara;
         
            private CharaAbilityInfo abilityInfo;
            public CharaAbilityInfo AbilityInfo => abilityInfo;
            
            public ItemParam(CharacterDetailData userDataChara, CharaAbilityInfo abilityInfo)
            {
                this.userDataChara = userDataChara;
                this.abilityInfo = abilityInfo;
            }
            
        }
        
        [SerializeField]
        private AdviserSkillView adviserSkillView;
        
        [SerializeField]
        private AdviserIcon adviserIcon;

        protected override void OnSetView(object value)
        {
            ItemParam param = (ItemParam)value;
            adviserSkillView.SetSkillView(param.AbilityInfo,param.UserDataChara.Lv, param.UserDataChara.LiberationLevel, false);
            adviserIcon.SetIcon(param.UserDataChara);
        }
    }
}