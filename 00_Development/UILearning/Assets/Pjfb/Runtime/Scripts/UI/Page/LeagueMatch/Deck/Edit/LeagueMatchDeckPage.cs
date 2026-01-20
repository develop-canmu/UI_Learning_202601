using System.Linq;
using System.Threading;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using Pjfb.Deck;
using Pjfb.Master;

namespace Pjfb.LeagueMatch
{
    public enum LeagueMatchDeckPageType
    {
        //デッキ編成
        LeagueMatchDeckEditTop,
        //キャラ選択
        LeagueMatchDeckEditCharaSelect,
    }
    
    public class LeagueMatchDeckPage : DeckPageBase<LeagueMatchDeckPageType>
    {
        public static long MaxDuplicateMCharaCount { get; private set; }
        public static string MCharaRestrictionString { get; private set; }
        
        protected override string GetAddress(LeagueMatchDeckPageType page)
        {
            return $"Prefabs/UI/Page/LeagueMatchDeck/{page}Page.prefab";
        }
        
        /// <param name="args">deck index</param>
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            DeckPageParameters parameters = (DeckPageParameters)args;

            var mDeckExtraMaster = MasterManager.Instance.deckExtraMaster.values.FirstOrDefault(x => x.useType == (int)(DeckType.LeagueMatch));
            var mDeckFormatConditionMaster = MasterManager.Instance.deckFormatConditionMaster.values.FirstOrDefault(x =>
                x.mDeckFormatId == (int)(mDeckExtraMaster?.mDeckFormatIdInGroup ?? (int)DeckFormatIdType.LeagueMatch) &&
                x.operatorType == DeckOperatorType.MAX_DUPLICATE);
                
            MaxDuplicateMCharaCount = mDeckFormatConditionMaster?.charaCount ?? 1;
            MCharaRestrictionString = string.Format(StringValueAssetLoader.Instance["league.match.deck.mchara_restriction"], MaxDuplicateMCharaCount); 

            if (TransitionType == PageTransitionType.Back)
            {
                await OpenPageAsync(LeagueMatchDeckPageType.LeagueMatchDeckEditTop, true, args, token);
                return;
            }
            ClearPageStack();
            DeckListData = await DeckUtility.GetLeagueMatchDeck();

            CurrentDeckIndex = parameters != null ? DeckListData.PartyNumberToIndex(DeckType.LeagueMatch, parameters.InitialPartyNumber) : DeckListData.SelectingIndex;

            await OpenPageAsync(LeagueMatchDeckPageType.LeagueMatchDeckEditTop, true, args, token);
        }
    }
}
