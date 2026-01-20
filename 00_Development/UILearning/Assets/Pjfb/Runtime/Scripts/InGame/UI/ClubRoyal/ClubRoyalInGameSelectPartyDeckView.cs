using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Deck;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using TMPro;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameSelectPartyDeckView : DeckView
    {
        [SerializeField] private TMP_Text ballCountText;
        [SerializeField] private GameObject dissolveButtonRoot;
        
        public void SetCharacterCell(List<BattleV2Chara> characters)
        {
            var combatPower = new BigValue();
            for (var i = 0; i < characters.Count; i++)
            {
                var chara = characters[i];
                var status = new CharaVariableProfileStatus();
                status.mCharaId = chara.mCharaId;
                status.rank = chara.rank;
                status.combatPower = chara.combatPower.ToString();

                combatPower += chara.combatPower;
                deckCharacterCells[i].InitializeUI(i, status, (RoleNumber)chara.roleNumber);
            }
            
            rankPowerUI.InitializePartyRankUI(combatPower);
        }

        public void SetDeckName(GuildBattlePartyModel party)
        {
            var partyName = party.DeckName;
            if (party.IsOnMap())
            {
                partyName = $"{StringValueAssetLoader.Instance[party.IsDefendingAtAnySpot() ? "clubroyalingame.defence_team" : "clubroyalingame.attack_team"]}{party.PlayerPartyId}";
            }
            deckNameView.SetDeckName(partyName);
        }

        public void SetBallCountText(int count)
        {
            ballCountText.text = count.ToString();
        }

        public void SetDissolveButtonActive(bool isActive)
        {
            if (dissolveButtonRoot != null)
            {
                dissolveButtonRoot.SetActive(isActive);                
            }
        }

        public void SetSelectPositionAction(Action<int> onSelect)
        {
            foreach (var characterCell in deckCharacterCells)
            {
                characterCell.SetActions(onSelect, null);
            }
        }
        
        public void SetStrategy(long strategyId)
        {
            if (strategyText == null) return;
            strategyText.text = (BattleConst.DeckStrategy)strategyId switch
            {
                BattleConst.DeckStrategy.Aggressive => StringValueAssetLoader.Instance["deck.strategy.aggressive"],
                BattleConst.DeckStrategy.Dribble => StringValueAssetLoader.Instance["deck.strategy.dribble"],
                BattleConst.DeckStrategy.Pass => StringValueAssetLoader.Instance["deck.strategy.pass"],
                _ => "-"
            };
        }

        public void SetCantSelectText(GuildBattlePartyModel party)
        {
            if (party.IsOnMap() || party.LocalUpdateFlag)
            {
                SetCoverText(StringValueAssetLoader.Instance["clubroyalingame.already_onmap_party"]);
                return;
            }
            
            if (party.IsDead())
            {
                SetCoverText(StringValueAssetLoader.Instance["clubroyalingame.remain_turn_to_re_send"].Format(party.RevivalTurn));
            }
        }
    }
}