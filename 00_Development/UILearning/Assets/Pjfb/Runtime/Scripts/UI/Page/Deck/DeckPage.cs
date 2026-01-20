using System.Threading;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pjfb.Deck
{
    public enum DeckPageType
    {
        DeckEditTop,
        DeckEditCharaSelect,
    }
    
    public class DeckPage : DeckPageBase<DeckPageType>
    {
        protected override string GetAddress(DeckPageType page)
        {
            return $"Prefabs/UI/Page/Deck/{page}Page.prefab";
        }

        /// <param name="args">deck index</param>
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            DeckPageParameters parameters = (DeckPageParameters)args;
            if (TransitionType == PageTransitionType.Back)
            {
                await OpenPageAsync(DeckPageType.DeckEditTop, true, args, token);
                return;
            }

            ClearPageStack();
            DeckListData = await DeckUtility.GetBattleDeck();

            CurrentDeckIndex = parameters != null ? DeckListData.PartyNumberToIndex(DeckType.Battle, parameters.InitialPartyNumber) : DeckListData.SelectingIndex;

            await OpenPageAsync(DeckPageType.DeckEditTop, true, args, token);
        }
    }
}


