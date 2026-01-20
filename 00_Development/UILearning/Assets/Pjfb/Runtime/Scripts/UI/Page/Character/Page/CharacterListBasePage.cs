using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Pjfb.Character
{
    public abstract class CharacterListBasePage : Page
    {
        private const string PossessionCountStringValueKey = "character.list.possession_count";
        
        #region SerializeFields
        
        [SerializeField] protected UserCharacterScroll characterScroll;
        [SerializeField] protected TextMeshProUGUI possessionCountText;
        
        #endregion

        protected int possessionCount = 0;
        protected int maxCount = 0;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            characterScroll.OnSelectedItem -= OnSelectCharacter;
            characterScroll.OnSelectedItem += OnSelectCharacter;
            characterScroll.OnReverseCharacterOrder = OnReverseCharacterOrder;
            characterScroll.OnSortFilter = OnSortFilter;
            characterScroll.OnSwipeDetailModal = OnSwipeDetailModal;
            SetPossessionText();
            return base.OnPreOpen(args, token);
        }


        protected virtual void OnSelectCharacter(object value)
        {
            OnSelectCharacterAsync(value).Forget();
        }

        protected virtual UniTask OnSelectCharacterAsync(object value)
        {
            return default;
        }

        protected void Refresh()
        {
            characterScroll.Refresh();
        }
        
        protected void RefreshItemView()
        {
            characterScroll.Scroll.RefreshItemView();
        }

        protected List<CharacterScrollData> GetItems()
        {
            return characterScroll.Scroll.GetItems().Cast<CharacterScrollData>().ToList();
        }
        
        protected CharacterScrollData GetItemById(long charaId)
        {
            return GetItems().FirstOrDefault(x => x.CharacterId == charaId);
        }
        
        protected void SetPossessionText()
        {
            if(possessionCountText is null)  return;
            possessionCountText.text = string.Format(StringValueAssetLoader.Instance[PossessionCountStringValueKey], possessionCount , maxCount);
        }

        protected virtual void OnReverseCharacterOrder(){}
        protected virtual void OnSortFilter(){}

        protected void SetFilterExcludeIdSet(IEnumerable<long> idList)
        {
            characterScroll.SetFilterExcludeIdSet(idList);
        }

        protected virtual void OnSwipeDetailModal(CharacterScrollData scrollData) { }

    }
}
