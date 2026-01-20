using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Deck;
using Pjfb.UserData;
using TMPro;
using UnityEngine;

namespace Pjfb.ClubDeck
{
    public class ClubDeckEditCharaSelectPage : DeckEditCharaSelectPage
    {
        [Header("Club Deck")]
        [SerializeField] private TextMeshProUGUI mCharaRestrictionText;
        protected override DeckData CurrentDeckData => ClubDeckPage.CurrentDeckData;
        private bool canEdit;

        protected override async UniTask OnPreOpen(object args)
        {
            canEdit = CurrentDeckData.CanEditDeck;
            SetDictionary();
            mCharaRestrictionText.text = ClubDeckPage.MCharaRestrictionString;
            await base.OnPreOpen(args);
        }
        
        protected override void SetConfirmButtonInteractable()
        {
            confirmButton.interactable =
                selectingChara is not null && 
                (!canEdit && formattingIdSet.Contains(selectingChara.id) || 
                 (canEdit && !AllDeckEditingUCharaIdHashSet.Contains(selectingChara.id)) && AllDeckMCharaIdCountDictionary.GetValueOrDefault(selectingChara.charaId, 0) < ClubDeckPage.MaxDuplicateMCharaCount);
        }

        protected override void SetBadge()
        {
            foreach (var scrollData in GetItems())
            {
                if (scrollData.id == currentEditingId)
                {
                    scrollData.DeckBadgeType = DeckBadgeType.CurrentEditing;
                }
                else if(formattingIdSet.Contains(scrollData.id))
                {
                    scrollData.DeckBadgeType = DeckBadgeType.Formatting;
                }
                else if (!canEdit)
                {
                    scrollData.DeckBadgeType = DeckBadgeType.CannotEdit;
                }
                else if (AllDeckEditingUCharaIdHashSet.Contains(scrollData.id))
                {
                    scrollData.DeckBadgeType = DeckBadgeType.AssignedByOtherTeam;
                }
                else if (AllDeckMCharaIdCountDictionary.GetValueOrDefault(scrollData.MCharaId, 0) >= ClubDeckPage.MaxDuplicateMCharaCount)
                {
                    scrollData.DeckBadgeType = DeckBadgeType.ReachMCharaLimit;
                }
            }
        }
        
        private readonly Dictionary<long, long> AllDeckMCharaIdCountDictionary= new Dictionary<long, long>();
        private readonly HashSet<long> AllDeckEditingUCharaIdHashSet = new HashSet<long>();
        private void SetDictionary()
        {
            AllDeckMCharaIdCountDictionary.Clear();
            AllDeckEditingUCharaIdHashSet.Clear();
            foreach (var deckData in ClubDeckPage.DeckListData.DeckDataList)
            {
                if(deckData.Index == ClubDeckPage.CurrentDeckIndex)  continue;
                foreach (var id in deckData.GetMemberIds())
                {
                    if(id == DeckUtility.EmptyDeckSlotId)   continue;
                    var uChara = UserDataManager.Instance.charaVariable.Find(id);
                    if (uChara is null) continue;
                    long mCharaId = uChara.charaId;
                    AllDeckEditingUCharaIdHashSet.Add(uChara.id);
                    if (AllDeckMCharaIdCountDictionary.ContainsKey(mCharaId))
                    {
                        AllDeckMCharaIdCountDictionary[mCharaId] += 1;
                    }
                    else
                    {
                        AllDeckMCharaIdCountDictionary[mCharaId] = 1;
                    }
                }
            }
        }

    }
}
