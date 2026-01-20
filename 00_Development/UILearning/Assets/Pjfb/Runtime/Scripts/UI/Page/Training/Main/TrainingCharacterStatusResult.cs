using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CruFramework.UI;
using Pjfb.Master;
using Pjfb.Networking.App.Request;

namespace Pjfb.Training
{
    public class TrainingCharacterStatusResult : MonoBehaviour
    {
        [SerializeField]
        private TMPro.TMP_Text nameText = null;
        [SerializeField]
        private TMPro.TMP_Text nickNameText = null;
        
        [SerializeField]
        private CharacterStatusValuesView statusView = null;
        
        [SerializeField]
        private ScrollGrid skillScroll = null;
        
        public void SetEmpty()
        {
            nameText.text = string.Empty;
            nickNameText.text = string.Empty;
            statusView.SetStatus(new CharacterStatus());
            skillScroll.SetItems(new SkillData[]{});
        }
        
        public void SetStatus(long mCharId, CharacterStatus status, TrainingAbility[] skills)
        {
            // スキル表示
            SkillData[] skillList = new SkillData[skills.Length];
            for(int i=0;i<skillList.Length;i++)
            {
                skillList[i] = new SkillData(skills[i].id, skills[i].level);
            }

            // ステータス
            SetStatus(mCharId, status, skillList);
        }
        
        public void SetStatus(long mCharId, CharacterStatus status, SkillData[] skills)
        {
            // MChar
            CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(mCharId);
            // 名前
            nameText.text = mChar.name;
            nickNameText.text = mChar.nickname;
            // ステータス
            statusView.SetStatus( status );
            // スクロールにセット
            skillScroll.SetItems(skills);
        }
        
        public void SetNpcStatus(BattleV2Chara battleChar)
        {
            // mChar
            CharaMasterObject mChar = MasterManager.Instance.charaMaster.FindData(battleChar.mCharaId);
            
            SkillData[] skillDataList = new SkillData[battleChar.abilityList.Length];
            for(int i=0;i<battleChar.abilityList.Length;i++)
            {
                skillDataList[i] = new SkillData(battleChar.abilityList[i].l[0],battleChar.abilityList[i].l[1] );
            }
            
            // スキル表示
            skillScroll.SetItems(skillDataList);
            // 名前
            nameText.text = mChar.name;
            nickNameText.text = mChar.nickname;                    
            // ステータス
            statusView.SetStatus( StatusUtility.ToCharacterStatus(battleChar) );
        }

    }
}