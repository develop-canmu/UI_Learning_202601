using System.Collections.Generic;
using CruFramework.Page;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Master;

namespace Pjfb.InGame.ClubRoyal
{
    public class ClubRoyalInGameSpotModalSheetAllTeam : Sheet
    {
        [SerializeField] 
        private ScrollGrid scrollGrid;
        [SerializeField]
        private GameObject noPartyMessage = null;

        public void SetData(List<GuildBattlePartyModel> parties)
        {
            if (parties.Count == 0)
            {
                noPartyMessage.SetActive(true);
                return;
            }
            
            noPartyMessage.SetActive(false);
            scrollGrid.SetItems(parties);
        }
    }
}