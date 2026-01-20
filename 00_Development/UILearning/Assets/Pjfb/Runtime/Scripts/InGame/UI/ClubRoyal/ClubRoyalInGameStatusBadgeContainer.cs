using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Pjfb.InGame;
using Pjfb.Networking.App.Request;

namespace Pjfb
{
    public class ClubRoyalInGameStatusBadgeContainer : MonoBehaviour
    {
        [SerializeField] private List<ClubRoyalInGameStatusBadgePage> guildBattleStatusBadgePage;

        #region Fields
        public static int currentPageCounter = 0;
        #endregion

        public static void OnUpdateTurnCounter()
        {
            currentPageCounter += 1;
        }

        public void Initialize(List<BattleV2AbilityEffect> activatedAbilityEffectList)
        {
            foreach (var badgePage in guildBattleStatusBadgePage)
            {
                badgePage.Clear();
            }

            if (currentPageCounter == 0 || activatedAbilityEffectList == null)
            {
                return;
            }

            if (activatedAbilityEffectList.Count == 0)
            {
                return;
            }

            int pageMaxCount = (activatedAbilityEffectList.Count + 2) / ClubRoyalInGameStatusBadgePage.BadgeMaxCount;
            int pageStartIndex = currentPageCounter % pageMaxCount;
            int badgeCount = 0;

            for (int i = pageStartIndex * ClubRoyalInGameStatusBadgePage.BadgeMaxCount; i < Mathf.Min(activatedAbilityEffectList.Count, (pageStartIndex + 1) * ClubRoyalInGameStatusBadgePage.BadgeMaxCount); i++)
            {
                AddBadge(badgeCount, activatedAbilityEffectList[i]);
                badgeCount += 1;
            }
        }

        private void AddBadge(int badgeCount, BattleV2AbilityEffect abilityEffect)
        {
            int pageIndex = badgeCount / ClubRoyalInGameStatusBadgePage.BadgeMaxCount; // 一旦定数で3にしているが、後で変更する可能性がある
            int badgeIndex = badgeCount % ClubRoyalInGameStatusBadgePage.BadgeMaxCount;

            int effectType = (int)abilityEffect.effectType;

            guildBattleStatusBadgePage[pageIndex].AddBadge(badgeIndex, effectType);
        }

        private bool IsPartyTargetedByAbilityEffect(GuildBattlePartyModel partyModel, BattleV2AbilityEffect abilityEffect)
        {
            return GuildBattleAbilityLogic.IsPartyTargetedByAbilityEffect(partyModel, abilityEffect);
        }
    }
}