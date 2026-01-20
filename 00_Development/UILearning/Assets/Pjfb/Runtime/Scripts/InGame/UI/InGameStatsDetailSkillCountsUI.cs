using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Pjfb.InGame;
using Pjfb.Networking.App.Request;

namespace Pjfb.InGame
{
    public class InGameStatsDetailSkillCountsUI : MonoBehaviour
    {
        [SerializeField] private ScrollGrid skillCountsScroll;
        [SerializeField] private InGameStatsDetailSkillCountUI originalSkillCountUI;

        private void Awake()
        {
            originalSkillCountUI.gameObject.SetActive(false);
        }

        public void SetData(BattleCharacterModel characterModel)
        {
            var list = new List<Tuple<BattleV2Ability, long, long>>();
            for (var i = 0; i < characterModel.Stats.ActivatedSkillIds.Count; i++)
            {
                var abilityId = characterModel.Stats.ActivatedSkillIds[i];
                var count = characterModel.Stats.SkillActivatedCounts[i];
                var abilityData = characterModel.AbilityList.FirstOrDefault(ability => ability.BattleAbilityMaster.id == abilityId);
                if (abilityData == null)
                {
                    continue;
                }
                var skillCountData = new Tuple<BattleV2Ability, long, long>(abilityData.BattleAbilityMaster, abilityData.AbilityLevel, count);
                list.Add(skillCountData);
            }

            skillCountsScroll.SetItems(list);
        }
    }
}