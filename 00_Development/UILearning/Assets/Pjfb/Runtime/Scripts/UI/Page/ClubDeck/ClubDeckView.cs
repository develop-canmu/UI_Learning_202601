using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.ClubMatch;
using Pjfb.Deck;
using Pjfb.UserData;
using TMPro;

namespace Pjfb.ClubDeck
{
    public class ClubDeckView : DeckView
    {
        
        [SerializeField] private TextMeshProUGUI mCharaRestrictionText;
        [SerializeField] private GameObject powerUpObject;
        [SerializeField] private GameObject powerDownObject;
        private BigValue newTotalCombatPower = BigValue.Zero;
        public override void InitializeUI(DeckData data)
        {
            base.InitializeUI(data);
            mCharaRestrictionText.text = ClubDeckPage.MCharaRestrictionString;
            SetNewCombatPower();
        }

        private void SetNewCombatPower()
        {
            newTotalCombatPower = BigValue.Zero;
            rankPowerUI.SetCombatPowerTextColor(deckData.FixedClubConditionData.combatPowerTextColor);
            switch (deckData.FixedClubConditionData.condition)
            {
                case ClubDeckCondition.Awful:
                    powerUpObject.SetActive(false);
                    powerDownObject.SetActive(false);
                    return;
                case ClubDeckCondition.NotBad:
                    powerUpObject.SetActive(false);
                    powerDownObject.SetActive(true);
                    break;
                case ClubDeckCondition.Good:
                    powerUpObject.SetActive(false);
                    powerDownObject.SetActive(false);
                    break;
                case ClubDeckCondition.Best:
                case ClubDeckCondition.Extreme:
                    powerUpObject.SetActive(true);
                    powerDownObject.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            newTotalCombatPower = BigValue.MulRate(deckData.CombatPower, deckData.FixedClubConditionData.combatPowerAmplifier);
            
            rankPowerUI.InitializePartyCombatPowerOnly(newTotalCombatPower);
        }
    }
}