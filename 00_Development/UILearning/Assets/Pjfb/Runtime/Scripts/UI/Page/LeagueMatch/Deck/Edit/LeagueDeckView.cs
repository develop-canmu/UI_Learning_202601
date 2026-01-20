using UnityEngine;
using Pjfb.Deck;
using Pjfb.UserData;

namespace Pjfb.LeagueMatch
{
    public class LeagueDeckView : DeckView
    {
        protected override string GetCoverText(DeckData data)
        {
            if(data.IsLocked)
            {
                return StringValueAssetLoader.Instance["league.match.deck.locked_entried"];
            }
            else
            {
                return StringValueAssetLoader.Instance["league.match.deck.locked_period"];
            }
        }
    }
}