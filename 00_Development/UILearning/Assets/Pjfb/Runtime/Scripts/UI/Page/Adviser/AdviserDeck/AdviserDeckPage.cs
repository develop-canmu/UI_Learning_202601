using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Deck;
using Pjfb.Master;

namespace Pjfb.Adviser
{
    public class AdviserDeckPage : DeckPageBase<AdviserDeckPage.DeckPageType>
    {
        
        public enum DeckPageType
        {
            DeckEditTop,
            Select,
        }

        protected override string GetAddress(DeckPageType page)
        {
            return $"Prefabs/UI/Page/AdviserDeck/Adviser{page}Page.prefab";
        }
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            DeckPageParameters parameters = (DeckPageParameters)args;
            
            DeckListData = await DeckUtility.GetDeckList(DeckType.Adviser);
            
            CurrentDeckIndex = parameters != null ? DeckListData.PartyNumberToIndex(DeckType.Adviser, parameters.InitialPartyNumber) : DeckListData.SelectingIndex;
            
            // ページのスタックが破棄されないので開く際にページのスタックを消しておく。
            ClearPageStack();
            
            await OpenPageAsync(DeckPageType.DeckEditTop, true, args, token);
        }
    }
}