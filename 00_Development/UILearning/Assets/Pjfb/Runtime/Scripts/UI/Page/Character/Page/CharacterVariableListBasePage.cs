using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.UserData;
using TMPro;
using UnityEngine;

namespace Pjfb.Character
{
    public abstract class CharacterVariableListBasePage : Page
    {
        private const string PossessionCountStringValueKey = "character.list.possession_count";
        #region SerializeFields
        [SerializeField] protected UserCharacterVariableScroll scroll;
        [SerializeField] protected TextMeshProUGUI possessionCountText;
        #endregion

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            if (scroll.ShowDeckFormattingBadge)
            {
                DeckListData deck = await DeckUtility.GetBattleDeck();
                DeckListData clubMatchDeck = await DeckUtility.GetClubBattleDeck();
                DeckListData leagueMatchDeck = await DeckUtility.GetLeagueMatchDeck();
                DeckListData clubRoyalDeck = await DeckUtility.GetClubRoyalDeck();
                scroll.SetDeckListData(deck, clubMatchDeck, leagueMatchDeck, clubRoyalDeck);
                scroll.SetUserCharacterVariableList();
                scroll.SetCharacterVariableList();
                Refresh();
            }
            scroll.OnSelectedItem -= OnSelectCharacter;
            scroll.OnSelectedItem += OnSelectCharacter;
            scroll.OnSortFilter = OnSortFilter;
            scroll.OnReverseCharacterOrder = OnReverseCharacterOrder;
            scroll.OnSwipeDetailModal = OnSwipeDetailModal;
            SetPossessionText();
            await base.OnPreOpen(args, token);
        }
        

        protected virtual void OnSelectCharacter(object value)
        {
            OnSelectCharacterAsync(value).Forget();
        }

        protected virtual UniTask OnSelectCharacterAsync(object value)
        {
            return default;
        }

        protected virtual void OnSortFilter(){}
        protected virtual void OnReverseCharacterOrder(){}
        
        
        protected virtual void Refresh()
        {
            scroll.Refresh();
            SetPossessionText();
        }
        
        protected CharacterVariableScrollData GetItemById(long id)
        {
            return GetItems().FirstOrDefault(x => x.id == id);
        }
        
        protected int GetItemIndexById(long id)
        {
            return GetItems().FindIndex(x => x.id == id);
        }
        
        protected virtual void RefreshItemView()
        {
            scroll.Scroll.RefreshItemView();
        }

        protected List<CharacterVariableScrollData> GetItems()
        {
            return scroll.Scroll.GetItems().Cast<CharacterVariableScrollData>().ToList();
        }

        private void SetPossessionText()
        {
            if(possessionCountText is null)  return;
            int possessionCount = UserDataManager.Instance.charaVariable.data.Values.Count;
            possessionCountText.text = string.Format(StringValueAssetLoader.Instance[PossessionCountStringValueKey], possessionCount , ConfigManager.Instance.uCharaVariableCountMax);
        }

        protected void SetFilterExcludeIdSet(IEnumerable<long> idList)
        {
            scroll.SetFilterExcludeIdSet(idList);
        }
        
        protected virtual void OnSwipeDetailModal(CharacterVariableScrollData scrollData) { }

    }

}
