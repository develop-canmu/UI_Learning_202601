using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using MagicOnion;
using Pjfb.Master;
using TMPro;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameAdviserCoolTimeGroupUI : MonoBehaviour
    {
        [SerializeField] private CharacterIcon adviserIcon = null;
        [SerializeField] private AdviserSkillImage adviserSkillIcon = null;
        [SerializeField] private ClubRoyalInGameDisplayNumberSpriteUI displayNumberSpriteUI = null;
        
        public void Initialize(GuildBattleAbilityData abilityData )
        {
            if (abilityData == null)
            {
                CruFramework.Logger.LogError("ClubRoyalInGameAdviserCoolTimeIcon: abilityData is null.");
                return;
            }

            if (adviserIcon == null || adviserSkillIcon == null || displayNumberSpriteUI == null)
            {
                CruFramework.Logger.LogError("ClubRoyalInGameAdviserCoolTimeIcon: Missing required components.");
                return;
            }

            AbilityMasterObject abilityMaster = MasterManager.Instance.abilityMaster.FindData(abilityData.AbilityId);
            if (abilityMaster == null)
            {
                CruFramework.Logger.LogError($"ClubRoyalInGameAdviserCoolTimeIcon: AbilityMasterObject not found for ID {abilityData.AbilityId}");
                return;
            }

            adviserIcon.SetIcon(abilityData.MCharaId);
            adviserIcon.SetActiveCharacterTypeIcon(false);
            adviserIcon.SetActiveRarity(false);
            adviserIcon.SetActiveLv(false);
            
            adviserSkillIcon.SetTextureAsync(abilityMaster.iconId).Forget();
            displayNumberSpriteUI.Display(abilityData.CoolTime);
        }
    }
}