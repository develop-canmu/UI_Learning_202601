using System.Linq;
using System.Threading;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using Pjfb.Deck;
using Pjfb.Master;

namespace Pjfb.LeagueMatch
{
    public enum LeagueMatchDeckEntryPageType
    {
        LeagueMatchDeckEntryTop,
    }
    
    public class LeagueMatchDeckEntryPage : PageManager<LeagueMatchDeckEntryPageType>
    {
        public class PageParams : DeckPageParameters
        {
            public long Id;
            public long RoundNumber;
            public ColosseumEventMasterObject ColosseumEventMaster; 
        }
        
        public static DeckListData DeckListData { get; private set; } 
        public static int CurrentDeckIndex;
        public static DeckData CurrentDeckData => DeckListData.DeckDataList[CurrentDeckIndex];
        public static long MaxDuplicateMCharaCount { get; private set; }
        public static string MCharaRestrictionString { get; private set; }
        
        protected override string GetAddress(LeagueMatchDeckEntryPageType page)
        {
            return $"Prefabs/UI/Page/LeagueMatchDeckEntry/{page}Page.prefab";
        }
        
        /// <param name="args">deck index</param>
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            PageParams parameters = (PageParams)args;
            // デッキ未指定の場合は、最初のデッキを指定
            if (parameters.InitialPartyNumber == 0)
            {
                parameters.InitialPartyNumber = (int)DeckType.LeagueMatch;
            }

            var mDeckExtraMaster = MasterManager.Instance.deckExtraMaster.values.FirstOrDefault(x => x.useType == (int)(DeckType.LeagueMatch));
            var mDeckFormatConditionMaster = MasterManager.Instance.deckFormatConditionMaster.values.FirstOrDefault(x =>
                x.mDeckFormatId == (int)(mDeckExtraMaster?.mDeckFormatIdInGroup ?? (int)DeckFormatIdType.LeagueMatch) &&
                x.operatorType == DeckOperatorType.MAX_DUPLICATE);
                
            MaxDuplicateMCharaCount = mDeckFormatConditionMaster?.charaCount ?? 1;
            MCharaRestrictionString = string.Format(StringValueAssetLoader.Instance["league.match.deck.mchara_restriction"], MaxDuplicateMCharaCount); 

            if (TransitionType == PageTransitionType.Back)
            {
                await OpenPageAsync(LeagueMatchDeckEntryPageType.LeagueMatchDeckEntryTop, true, args, token);
                return;
            }
            ClearPageStack();
            DeckListData = await DeckUtility.GetLeagueMatchDeck();

            CurrentDeckIndex = parameters != null ? DeckListData.PartyNumberToIndex(DeckType.LeagueMatch, parameters.InitialPartyNumber) : DeckListData.SelectingIndex;

            await OpenPageAsync(LeagueMatchDeckEntryPageType.LeagueMatchDeckEntryTop, true, args, token);
        }
        
        protected override async UniTask<bool> OnPreLeave(CancellationToken token)
        {
            bool result = true;
            if (IsDeckChanged())
            {
                result = await DeckUtility.OnLeaveCurrentDeck(this.GetCancellationTokenOnDestroy());
                if (result)
                {
                    CurrentDeckData.DiscardChanges();
                }
            }
            return result;
        }
        
        private bool IsDeckChanged()
        {
            return CurrentDeckData.IsDeckChanged;
        }
    }
}
