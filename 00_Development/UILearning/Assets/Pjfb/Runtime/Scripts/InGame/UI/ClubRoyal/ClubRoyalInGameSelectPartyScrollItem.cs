using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Deck;
using Pjfb.Master;
using Pjfb.Networking.App.Request;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameSelectPartyScrollItem : ScrollGridItem
    {
        public class Arguments
        {
            public GuildBattlePartyModel PartyModel;
            public List<BattleV2Chara> CharaList;
            public bool IsSelectPartyView;
            public Arguments(GuildBattlePartyModel partyModel, bool isSelectPartyView)
            {
                PartyModel = partyModel;
                CharaList = new List<BattleV2Chara>();
                foreach (var uCharaId in partyModel.UCharaIds)
                {
                    var chara = PjfbGuildBattleDataMediator.Instance.BattleCharaData[uCharaId];
                    CharaList.Add(chara);
                }

                IsSelectPartyView = isSelectPartyView;
            }
        }

        [SerializeField] private ClubRoyalInGameSelectPartyDeckView deckView;
        private GuildBattlePartyModel partyModel;
        private Arguments arguments;
        
        protected override void OnSetView(object value)
        {
            arguments = (Arguments)value;
            partyModel = arguments.PartyModel;
            
            deckView.SetSelectPositionAction(OnClickSelectPositionButton);
            deckView.SetCharacterCell(arguments.CharaList);
            deckView.SetDeckName(arguments.PartyModel);
            deckView.SetStrategy(arguments.PartyModel.TacticsId);
            deckView.SetCoverObjectActive(false);

            if (arguments.IsSelectPartyView)
            {
                deckView.SetDissolveButtonActive(false);
                if (partyModel.IsOnMap() || partyModel.IsDead() || partyModel.LocalUpdateFlag)
                {
                    deckView.SetCoverObjectActive(true);
                    deckView.SetCantSelectText(partyModel);
                }
            }
            else
            {
                deckView.SetDissolveButtonActive(partyModel.IsDefendingAtAnySpot());
                deckView.SetBallCountText(partyModel.GetBallCount());
            }
        }
        
        public void OnClickDissolution()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubRoyalInGameDissolveParty, new ClubRoyalInGameDissolvePartyModal.Arguments(partyModel.PartyId, false));
        }

        private void OnClickSelectPositionButton(int index)
        {
            var charaId = partyModel.UCharaIds[index];
            var chara = PjfbGuildBattleDataMediator.Instance.BattleCharaData[charaId];
            var args = new PositionChangeModalWindow.WindowParams();
            args.CurrentRole = (RoleNumber)chara.roleNumber;
            args.onChanged = role =>
            {
                chara.roleNumber = (long)role;
                partyModel.RoleOperationIds[index] = (long)role;
                deckView.SetCharacterCell(arguments.CharaList);
            };
            args.onClosed = null;
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.PositionChange, args);
        }

        public void OnClickSelectTacticsButton()
        {
            var args = new StrategyChoiceModalWindow.WindowParams();
            args.strategy = (BattleConst.DeckStrategy)partyModel.TacticsId;
            args.onStrategyChanged = strategy =>
            {
                partyModel.TacticsId = (long)strategy;
                deckView.SetStrategy((long)strategy);
            };

            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.StrategyChoice, args);
        }

    }
}