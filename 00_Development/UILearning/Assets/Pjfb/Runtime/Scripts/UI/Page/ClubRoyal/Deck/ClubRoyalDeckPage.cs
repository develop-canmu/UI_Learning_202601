using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Deck;
using Pjfb.Master;

namespace Pjfb.ClubRoyal
{
    public class ClubRoyalDeckPage : DeckPageBase<ClubRoyalDeckPage.DeckPageType>
    {
        public enum DeckPageType
        {
            DeckEditTop,
            DeckEditCharaSelect,
        }
        
        public static long MaxDuplicateMCharaCount { get; private set; }
        
        protected override string GetAddress(DeckPageType page)
        {
            return $"Prefabs/UI/Page/ClubRoyalDeck/ClubRoyal{page}Page.prefab";
        }

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            DeckPageParameters parameters = (DeckPageParameters)args;
            
            DeckFormatConditionMasterObject mDeckFormatConditionMaster = MasterManager.Instance.deckFormatConditionMaster.values.FirstOrDefault(x =>
                x.mDeckFormatId == (long)DeckFormatIdType.ClubRoyal && x.operatorType == DeckOperatorType.MAX_DUPLICATE);
                
            // 制限の数がない場合もありそうなので、その場合は-1を入れておく
            MaxDuplicateMCharaCount = mDeckFormatConditionMaster?.charaCount ?? -1;
            
            DeckListData = await DeckUtility.GetClubRoyalDeck();
            
            CurrentDeckIndex = parameters != null ? DeckListData.PartyNumberToIndex(DeckType.ClubRoyal, parameters.InitialPartyNumber) : DeckListData.SelectingIndex;
            
            // ページのスタックが破棄されないので開く際にページのスタックを消しておく。
            ClearPageStack();
            
            await OpenPageAsync(DeckPageType.DeckEditTop, true, args, token);
        }
    }
}