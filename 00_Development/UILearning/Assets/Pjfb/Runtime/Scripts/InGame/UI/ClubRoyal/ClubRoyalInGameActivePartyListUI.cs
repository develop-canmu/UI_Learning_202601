using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameActivePartyListUI : MonoBehaviour
    {
        [SerializeField] private ClubRoyalInGamePartyLeaderUI[] leaderUIs;
        private bool isEntryUpdateCoolTimeParty = false;

        private void Awake()
        {
            foreach (var ui in leaderUIs)
            {
                ui.SetOnClickCallback(OnClickIcon);
            }
        }

        public void UpdateUI()
        {
            if (!PjfbGuildBattleDataMediator.Instance.AllPjfbBattlePartyListByPlayerIndex.TryGetValue(PjfbGuildBattleDataMediator.Instance.PlayerIndex, out var parties))
            {
                return;
            }

            foreach (var party in parties)
            {
                var leaderUI = leaderUIs.FirstOrDefault(ui => ui.GetPartyIdentifier() == party.Identifier || ui.GetPartyIdentifier() == -1);
                // ありえないけど.
                if (leaderUI == null)
                {
                    continue;
                }
                
                leaderUI.InitializePartyList(party, false);
            }
            ClubRoyalInGameStatusBadgeContainer.OnUpdateTurnCounter();
        }

        private void OnClickIcon(GuildBattlePartyModel party)
        {
            if (!party.IsOnMap())
            {
                return;
            }
            
            var args = new ClubRoyalInGameDissolvePartyModal.Arguments(party.PartyId, !party.IsDefendingAtAnySpot());
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubRoyalInGameDissolveParty, args);
        }

        public void GrowEffectAtCoolTimePartyEntry(GuildBattlePlayerData playerData)
        {
            if (isEntryUpdateCoolTimeParty)
            {
                return;
            }
            
            isEntryUpdateCoolTimeParty = true;
            GrowEffectAtCoolTimePartyDraw();
            ClubRoyalInGameUIMediator.Instance.OnPlayerDataUpdated.AddListener(GrowEffectAtCoolTimeParty);
        }
        
        private void GrowEffectAtCoolTimePartyDraw()
        {
            // 起動中はクールタイムのパーティーが自発的にエフェクトを表示する
            foreach (ClubRoyalInGamePartyLeaderUI leader in leaderUIs)
            {
                leader.GrowEffectAtCoolTimeParty();
            }
        }

        private void GrowEffectAtCoolTimeParty(GuildBattlePlayerData playerData)
        {
            // クールタイム系スキルが発動しなくなったらクリアと解除処理を行う
            if (!GuildBattleAbilityLogic.IsCoolTimeAbilityActivating(playerData))
            {
                ClubRoyalInGameUIMediator.Instance.OnPlayerDataUpdated.RemoveListener(GrowEffectAtCoolTimeParty);
                isEntryUpdateCoolTimeParty = false;
                foreach (ClubRoyalInGamePartyLeaderUI leader in leaderUIs)
                {
                    leader.ClearGrowEffectRoot().Forget();
                }
            }
            else
            {
                GrowEffectAtCoolTimePartyDraw();
            }
        }
    }
}