using CruFramework.UI;
using UnityEngine;

namespace Pjfb.LeagueMatchTournament
{
    public class LeagueMatchTournamentListScrollDynamicItemSelector : ScrollDynamicItemSelector
    {
        [SerializeField] 
        private ScrollDynamicItem item = null;

        public override ScrollDynamicItem GetItem(object item)
        {
            return this.item;
        }
    }
}