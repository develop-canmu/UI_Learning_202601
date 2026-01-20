using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameActivePartyListScrollItem : ScrollGridItem
    {
        [SerializeField] private ClubRoyalInGamePartyLeaderUI leaderUI;
        
        protected override void OnSetView(object value)
        {
            var partyModel = value as GuildBattlePartyModel;
            leaderUI.InitializePartyList(partyModel, false);
            leaderUI.SetOnClickCallback(OnClickIcon);
        }

        private void OnClickIcon(GuildBattlePartyModel partyModel)
        {
            if (!partyModel.IsOnMap())
            {
                return;
            }
            
            var args = new ClubRoyalInGameDissolvePartyModal.Arguments(partyModel.PartyId, !partyModel.IsDefendingAtAnySpot());
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ClubRoyalInGameDissolveParty, args);
        }
    }
}