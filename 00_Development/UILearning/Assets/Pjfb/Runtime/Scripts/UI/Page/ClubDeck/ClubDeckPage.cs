using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using Pjfb.Deck;
using Pjfb.Master;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.ClubDeck
{
    public enum ClubDeckPageType
    {
        ClubDeckEditTop,
        ClubDeckEditCharaSelect,
    }
    
    public class ClubDeckPage : DeckPageBase<ClubDeckPageType>
    {
        public static long MaxDuplicateMCharaCount { get; private set; }
        public static string MCharaRestrictionString { get; private set; }
        public static string FatigueRestrictionString { get; private set; }
        
        protected override string GetAddress(ClubDeckPageType page)
        {
            return $"Prefabs/UI/Page/ClubDeck/{page}Page.prefab";
        }
        
        
        /// <param name="args">deck index</param>
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            DeckPageParameters parameters = (DeckPageParameters)args;

            var mDeckExtraMaster = MasterManager.Instance.deckExtraMaster.values.FirstOrDefault(x => x.useType == (int)(DeckType.Club));
            var mDeckFormatConditionMaster = MasterManager.Instance.deckFormatConditionMaster.values.FirstOrDefault(x =>
                x.mDeckFormatId == (int)(mDeckExtraMaster?.mDeckFormatIdInGroup ?? (int)DeckFormatIdType.Club) &&
                x.operatorType == DeckOperatorType.MAX_DUPLICATE); 
                
            MaxDuplicateMCharaCount = mDeckFormatConditionMaster?.charaCount ?? 1;
            MCharaRestrictionString = string.Format(StringValueAssetLoader.Instance["club.deck.mchara_restriction"], MaxDuplicateMCharaCount); 
            FatigueRestrictionString = string.Format(StringValueAssetLoader.Instance["club.deck.fatigue_restriction_content"], GetFatigueRestrictionPercentage());
            if (TransitionType == PageTransitionType.Back)
            {
                await OpenPageAsync(ClubDeckPageType.ClubDeckEditTop, true, args, token);
                return;
            }
            ClearPageStack();
            DeckListData = await DeckUtility.GetClubBattleDeck();

            CurrentDeckIndex = parameters != null ? DeckListData.PartyNumberToIndex(DeckType.Club, parameters.InitialPartyNumber) : DeckListData.SelectingIndex;
            
            await OpenPageAsync(ClubDeckPageType.ClubDeckEditTop, true, args, token);
        }

        public static int GetFatigueRestrictionPercentage()
        {
            var mDeckExtraTiredness = MasterManager.Instance.deckExtraTirednessMaster.values.FirstOrDefault(x => x.useType == (int)DeckType.Club);
            if (mDeckExtraTiredness is null) return 0;
            return Mathf.FloorToInt((float)mDeckExtraTiredness.valueMaxToAction * 100 / mDeckExtraTiredness.valueMax);
        }
        
        public static void ShowFatigueRestrictionPopup()
        {
            if (string.IsNullOrEmpty(FatigueRestrictionString))
            {
                FatigueRestrictionString = StringValueAssetLoader.Instance["club.deck.fatigue_restriction_content"].Format(GetFatigueRestrictionPercentage());
            }
            ConfirmModalWindow.Open(new ConfirmModalData(
                StringValueAssetLoader.Instance["club.deck.fatigue_restriction_title"],
                FatigueRestrictionString, string.Empty,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"],
                    (window => window.Close()))));
        }
        
        public static void ShowFatigueDeckEditRestrictionPopup()
        {
            ConfirmModalWindow.Open(new ConfirmModalData(
                StringValueAssetLoader.Instance["club.deck.condition_deck_edit_restriction_title"],
                StringValueAssetLoader.Instance["club.deck.condition_deck_edit_restriction_content"], 
                string.Empty,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"],
                    (window => window.Close()))));
        }
    }
}


