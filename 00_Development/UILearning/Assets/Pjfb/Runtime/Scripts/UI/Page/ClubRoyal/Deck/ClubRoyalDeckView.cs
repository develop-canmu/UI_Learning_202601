using Pjfb.Deck;

namespace Pjfb.ClubRoyal
{
    public class ClubRoyalDeckView : DeckView
    {
        protected override string GetCoverText(DeckData data)
        {
            return StringValueAssetLoader.Instance["club_royal.deck.lock"];
        }
    }
}