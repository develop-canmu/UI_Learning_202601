using System.Linq;
using CruFramework.UI;
using MagicOnion;
using Pjfb.Character;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using Logger = CruFramework.Logger;

namespace Pjfb.InGame.ClubRoyal
{ 
    public class ClubRoyalInGameAdviserSkillDescriptionGroupScrollItemScrollData
    {
        public GuildBattleAbilityData guildBattleAbilityData;

        public ClubRoyalInGameAdviserSkillDescriptionGroupScrollItemScrollData(GuildBattleAbilityData abilityData)
        {
            guildBattleAbilityData = abilityData;
        }
    }
    
    public class ClubRoyalInGameAdviserSkillDescriptionGroupScrollItem : ScrollGridItem
    {
        // スキル説明View
        [SerializeField]
        private ClubRoyalInGameAdviserSkillDescriptionView skillDescriptionView = null;

        // アドバイザーアイコン
        [SerializeField]
        private AdviserIcon adviserIcon = null; 
        
        private ClubRoyalInGameAdviserSkillDescriptionGroupScrollItemScrollData scrollData;
        
        protected override void OnSetView(object value)
        {
            scrollData = (ClubRoyalInGameAdviserSkillDescriptionGroupScrollItemScrollData)value;
            if (scrollData.guildBattleAbilityData == null)
            {
                CruFramework.Logger.LogError("AdviserSkillDescriptionGroupScrollItem: guildBattleAbilityData is null");
                return;
            }
            BattleV2Ability battleV2Ability = PjfbGuildBattleDataMediator.Instance.OriginalBattleData.abilityList.FirstOrDefault(val => val.id == scrollData.guildBattleAbilityData.AbilityId);
            if (battleV2Ability == null)
            {
                CruFramework.Logger.LogError($"AdviserSkillDescriptionGroupScrollItem: BattleV2Ability not found for AbilityId: {scrollData.guildBattleAbilityData.AbilityId}");
                return;
            }

            // アドバイザーのキャラデータを取得
            UserDataChara targetAdviser = UserDataManager.Instance.userAdviserList.FirstOrDefault(a => a.charaId == scrollData.guildBattleAbilityData.MCharaId);
            if (targetAdviser == null)
            {
                Logger.LogError($"AdviserSkillDescriptionGroupScrollItem: UserDataChara not found for MCharaId: {scrollData.guildBattleAbilityData.MCharaId}");
                return;
            }

            if (!CharaAbilityInfo.Builder(out CharaAbilityInfo abilityInfo, scrollData.guildBattleAbilityData, (BattleConst.AbilityType)battleV2Ability.abilityType))
            {
                Logger.LogError("Failed to build CharaAbilityInfo. Please check the ability data.");
                return;
            }
            
            skillDescriptionView.SetView(abilityInfo, targetAdviser.level, targetAdviser.newLiberationLevel);
            adviserIcon.SetImage(targetAdviser.charaId);
        }
    }
    
    
    
}