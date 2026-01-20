using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pjfb.Character
{
    public class SuccessCharaSellPage : CharacterVariableListBasePage
    {
        #region SerializeFields

        [SerializeField] private UIButton sellButton;
        
        #endregion

        private Dictionary<long, CharacterVariableScrollData> selectingDictionary;
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            selectingDictionary = new ();
            SetSellButtonInteractable();
            await base.OnPreOpen(args, token);
        }

        protected override void OnOpened(object args)
        {
            base.OnOpened(args);
            Refresh();
        }


        #region EventListeners
        protected override void OnSelectCharacter(object value)
        {
            CharacterVariableScrollData data = value as CharacterVariableScrollData;
            if(data is null) return;
            if(data.DeckBadgeType != DeckBadgeType.None)  return;
            if(data.IsFavorite) return;
            
            
            
            if (selectingDictionary.ContainsKey(data.id))
            {
                data.IsSelecting = false;
                selectingDictionary.Remove(data.id);
            }
            else
            {
                selectingDictionary.Add(data.id, data);
                data.IsSelecting = true;
            }
            SetSellButtonInteractable();
            RefreshItemView();
        }

        private void SetSellButtonInteractable()
        {
            sellButton.interactable = selectingDictionary.Count > 0;
        }

        protected override void OnReverseCharacterOrder()
        {
            foreach (var characterVariableScrollData in GetItems())
            {
                if (!selectingDictionary.ContainsKey(characterVariableScrollData.id)) continue;

                selectingDictionary[characterVariableScrollData.id] = characterVariableScrollData;
                characterVariableScrollData.IsSelecting = true;
            }
            RefreshItemView();
            SetSellButtonInteractable();
        }

        protected override void OnSortFilter()
        {
            selectingDictionary = new Dictionary<long, CharacterVariableScrollData>();
            SetSellButtonInteractable();
        }

        public void OnClickSellButton()
        {
            SuccessCharaSellConfirmModalWindow.Open(new SuccessCharaSellConfirmModalWindow.WindowParams
            {
                idList = selectingDictionary.Keys.ToArray(),
                onCancel = null,
                onConfirm = AppManager.Instance.UIManager.PageManager.PrevPage
            });
        }

        public void OnClickFavoriteButton()
        {
            CharacterPage m = (CharacterPage)Manager;
            m.OpenPage(CharacterPageType.SuccessCharaFavorite, true, null);
        }
        #endregion
        
    }
}
