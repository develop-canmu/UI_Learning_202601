using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using MagicOnion;
using TMPro;
using UnityEngine.UI;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameSpotModalAllTeamItem : ScrollGridItem
    {
        [SerializeField] private Image nameBaseImage;
        [SerializeField] private CharacterIcon characterIcon;
        [SerializeField] private TMP_Text partyIdText;
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private TMP_Text totalPowerText;
        [SerializeField] private OmissionTextSetter totalPowerOmissionTextSetter;
        [SerializeField] private ClubRoyalInGameBallCountUI ballCountUI;

        private GuildBattlePartyModel party;
        
        protected override void OnSetView(object value)
        {
            party = value as GuildBattlePartyModel;
            var leader = party.GetLeaderCharacterData();
            characterIcon.SetIconTextureWithEffectAsync(leader.mCharaId).Forget();
            partyIdText.text = party.PlayerPartyId.ToString();
            playerNameText.text = PjfbGuildBattleDataMediator.Instance.GetBattlePlayer(party.PlayerId)?.Name ?? string.Empty;
            BigValue combatPower = BigValue.Zero;
            foreach (var uCharaId in party.UCharaIds)
            {
                var chara = PjfbGuildBattleDataMediator.Instance.BattleCharaData[uCharaId];
                combatPower += chara.combatPower;
            }

            totalPowerText.text = combatPower.ToDisplayString(totalPowerOmissionTextSetter.GetOmissionData());
            ballCountUI.SetActiveBallCount(party.StartBallCount, party.GetBallCount());
            ballCountUI.SetOutlineColor(party.Side == PjfbGuildBattleDataMediator.Instance.PlayerSide ? GuildBattleCommonConst.GuildBattleTeamSide.Left : GuildBattleCommonConst.GuildBattleTeamSide.Right);
            var isMyParty = party.PlayerIndex == PjfbGuildBattleDataMediator.Instance.PlayerIndex;
            nameBaseImage.color = ColorValueAssetLoader.Instance[isMyParty ? "clubroyalingame.self_party_name_base" : "clubroyalingame.other_party_name_base"];
            playerNameText.color = isMyParty ? ColorValueAssetLoader.Instance["white"] : ColorValueAssetLoader.Instance["default"];
        }

        public void OnLongTap()
        {
            var args = new ClubRoyalInGameDissolvePartyModal.Arguments(party.PartyId, true);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubRoyalInGameDissolveParty, args);
        }
    }
}