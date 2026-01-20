using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Character;
using Pjfb.Master;
using TMPro;

namespace Pjfb
{
    public class ClubRoyalInGameAdviserSkillDescriptionView : MonoBehaviour
    {
        [SerializeField] private AdviserSkillView adviserSkillView;
        [SerializeField] private TMP_Text activateTypeText;
        [SerializeField] private TMP_Text activateCountText;
        [SerializeField] private TMP_Text coolTimeCountText;
        [SerializeField] private TMP_Text abilityDescriptionText;

        public void SetView(CharaAbilityInfo abilityInfo, long level, long liberationLevel)
        {
            AbilityMasterObject master = abilityInfo.GetAbilityMaster();
            adviserSkillView.SetSkillView(abilityInfo, level, liberationLevel, false);
            
            activateTypeText.text = StringValueAssetLoader.Instance[$"ability.activation_type_{(int) abilityInfo.AbilityType}"];

            activateCountText.text = master.maxInvokeCount.ToString();
            coolTimeCountText.text = master.coolDownTurnCount.ToString();
            abilityDescriptionText.text = master.description;
        }
    }
}