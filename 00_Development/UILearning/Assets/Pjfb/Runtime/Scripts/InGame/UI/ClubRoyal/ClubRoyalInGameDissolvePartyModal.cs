using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using MagicOnion;
using Pjfb.Networking.App.Request;
using TMPro;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameDissolvePartyModal : ModalWindow
    {
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private TMP_Text attentionText;
        [SerializeField] private ClubRoyalInGameSelectPartyDeckView deckView;
        [SerializeField] private UIButton closeButton;
        [SerializeField] private UIButton okButton;

        private GuildBattlePartyModel party;
        private string targetSpotName;
        private string teamNameAttribute;
        
        public class Arguments
        {
            public int PartyId;
            public bool IsDetail;
            
            public Arguments(int partyId, bool isDetail)
            {
                PartyId = partyId;
                IsDetail = isDetail;
            }
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            var openArgs = (Arguments)args;

            if (!PjfbGuildBattleDataMediator.Instance.OnMapPjfbBattleParties.TryGetValue(openArgs.PartyId, out party))
            {
                Close();
                return base.OnPreOpen(args, token);
            }
            
            var characters = new List<BattleV2Chara>();
            foreach (var uCharaId in party.UCharaIds)
            {
                var chara = PjfbGuildBattleDataMediator.Instance.BattleCharaData[uCharaId];
                characters.Add(chara);
            }
            
            deckView.SetCharacterCell(characters);
            deckView.SetDeckName(party);
            deckView.SetBallCountText(party.GetBallCount());
            attentionText.text = StringValueAssetLoader.Instance[PjfbGuildBattleDataMediator.Instance.GameState == GuildBattleCommonConst.GuildBattleGameState.BeforeFight ? "clubroyalingame.dissolution_modal_attention" : "clubroyalingame.dissolution_modal_attention_infight"];

            targetSpotName = PjfbGuildBattleDataMediator.Instance.BattleField.MapSpotsDictionary[party.TargetSpotId].GetSpotName();
            teamNameAttribute = party.IsDefendingAtAnySpot()
                ? StringValueAssetLoader.Instance["clubroyalingame.defence_team"] : StringValueAssetLoader.Instance["clubroyalingame.attack_team"];

            if (openArgs.IsDetail)
            {
                bodyText.gameObject.SetActive(false);
                attentionText.gameObject.SetActive(false);
                okButton.gameObject.SetActive(false);
            }
            
            return base.OnPreOpen(args, token);
        }

        public void OnClickOKButton()
        {
            closeButton.interactable = false;
            okButton.interactable = false;
            RequestDissolveParty().Forget();
        }

        private async UniTask RequestDissolveParty()
        {
            var ret = await PjfbGameHubClient.Instance.RequestDissolutionParty(party.PartyId);
            if (ret)
            {
                AppManager.Instance.UIManager.ModalManager.CloseAllModalWindow();
                AppManager.Instance.UIManager.System.UINotification.ShowNotification(StringValueAssetLoader
                    .Instance["clubroyalingame.dissolve_party_notification"].Format(targetSpotName, teamNameAttribute));
                PjfbGuildBattleDataMediator.Instance.AllPjfbBattleParties[party.Identifier].LocalUpdateFlag = true;
                if (PjfbGuildBattleDataMediator.Instance.OnMapPjfbBattleParties.TryGetValue(party.PartyId, out var onMapParty))
                {
                    onMapParty.LocalUpdateFlag = true;
                }
            }

            Close();
        }
    }
}