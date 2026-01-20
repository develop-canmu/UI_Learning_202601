using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Deck
{
    public class DeckEditCharaSelectPage : CharacterVariableListBasePage
    {
        public class DeckEditCharaSelectData
        {
            public PageType OpenFrom;
            public int SelectedCharaSlotIndex;
        }
        

        private DeckEditCharaSelectData deckEditCharaSelectData;
        
        private CharacterVariableScrollData selectingScrollData;
        private UserDataCharaVariable currentSettingChara;
        protected UserDataCharaVariable selectingChara;
        private CharacterRankPowerChangesView.HighlightState currentHighlightState;

        protected HashSet<long> formattingIdSet;
        protected long currentEditingId;
        
        private UserDataCharaVariable ShowingCharacter => currentHighlightState switch
        {
            CharacterRankPowerChangesView.HighlightState.Current => currentSettingChara,
            CharacterRankPowerChangesView.HighlightState.After => selectingChara,
            _ => null
        };

        protected virtual DeckData CurrentDeckData => DeckPage.CurrentDeckData;
        
        #region SerializeFields

        [SerializeField] private CharacterCardBackgroundImage characterCardBackgroundImage;
        [SerializeField] private SuccessCharacterNameView nameView;
        [SerializeField] private CharacterStatusValuesView statusValueView;
        [SerializeField] private ScrollGrid skillScroll;
        [SerializeField] private CharacterRankPowerChangesView rankPowerChangesView;
        [SerializeField] private UIButton changeHighlightButton;
        [SerializeField] private GameObject characterDetailRoot;
        [SerializeField] protected UIButton confirmButton;
        
        #endregion

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            await OnPreOpen(args);
            await base.OnPreOpen(args, token);
        }
        
        protected virtual async UniTask OnPreOpen(object args)
        {
            deckEditCharaSelectData = (DeckEditCharaSelectData)args;
            currentEditingId = 0;
            formattingIdSet = new HashSet<long>();
            selectingChara = null;
            selectingScrollData = null;
            
            var idList = CurrentDeckData.GetMemberIds();
            for (int i = 0; i < idList.Count(); i++)
            {
                long id = idList.ElementAt(i);
                if (i == deckEditCharaSelectData.SelectedCharaSlotIndex)
                    currentEditingId = id;
                else
                    formattingIdSet.Add(id);
            }

            var excludeIdSet = idList.Where(x => x != DeckUtility.EmptyDeckSlotId).Select(x => (long)x);
            
            SetFilterExcludeIdSet(excludeIdSet);

            currentSettingChara = UserDataManager.Instance.charaVariable.Find(currentEditingId);
            
            currentHighlightState = CharacterRankPowerChangesView.HighlightState.After;
            rankPowerChangesView.SetHighlight(currentHighlightState);
            changeHighlightButton.interactable = currentSettingChara is not null;
            
            scroll.SetUserCharacterVariableList();
            scroll.SetCharacterVariableList();
            Refresh();
            SetBadge();

            await OnSelectCharacterAsync(currentSettingChara is null
                ? GetItems().FirstOrDefault()
                : GetItemById(currentSettingChara.id));
            
            
            rankPowerChangesView.InitializeUI(currentSettingChara, selectingChara);
        }

        protected override async void OnEnablePage(object args)
        {
            base.OnEnablePage(args);
            if (deckEditCharaSelectData != null && deckEditCharaSelectData.OpenFrom == PageType.TeamConfirm)
            {
                await BGMManager.PlayBGMAsync(BGM.bgm_gvg_top);
            }
            else
            {
                await BGMManager.PlayBGMAsync(BGM.bgm_home);
            }
        }

        #region EventListeners

        protected override async UniTask OnSelectCharacterAsync(object value)
        {
            if (value is not CharacterVariableScrollData data)
            {
                SetCharacterViewActive(false);
                selectingChara = null;
                rankPowerChangesView.InitializeUI(currentSettingChara, selectingChara);
                return;
            }

            if ((selectingChara?.id ?? -1) == data.id) return;
            
            selectingChara = UserDataManager.Instance.charaVariable.Find(data.id);
            SetCharacterViewActive(true);            
                
            

            if (selectingScrollData != null)
                selectingScrollData.IsSelecting = false;

            selectingScrollData = data;
            selectingScrollData.IsSelecting = true;
            
            rankPowerChangesView.InitializeUI(currentSettingChara, selectingChara);
            await RefreshItemViewAsync();
        }


        
        public void OnClickConfirmButton()
        {
            CurrentDeckData.SetMemberId(deckEditCharaSelectData.SelectedCharaSlotIndex, selectingChara.id);
            AppManager.Instance.UIManager.PageManager.PrevPage();
        }
        

        public void ChangeHighlight()
        {
            currentHighlightState = (currentHighlightState == CharacterRankPowerChangesView.HighlightState.Current)
                ? CharacterRankPowerChangesView.HighlightState.After
                : CharacterRankPowerChangesView.HighlightState.Current;
            
            rankPowerChangesView.SetHighlight(currentHighlightState);
            SetView().Forget();
        }

        public void OnClickCharacterDetail()
        {
            var chara = (currentHighlightState == CharacterRankPowerChangesView.HighlightState.Current)
                ? currentSettingChara
                : selectingChara;
            var scrollData = GetItemById(chara.id);
            SuccessCharaDetailModalWindow.Open(ModalType.SuccessCharaDetail,
                new SuccessCharaDetailModalParams(scrollData.SwipeableParams, true));
        }
        
     
        #endregion

        private async UniTask RefreshItemViewAsync()
        {
            RefreshItemView();
            await SetView();
        }


        private async UniTask SetView()
        {
            var chara = ShowingCharacter;
            if(chara is null) return;
            CharacterVariableDetailData viewData = new CharacterVariableDetailData(chara);
            await nameView.InitializeUIAsync(viewData);
            await characterCardBackgroundImage.SetTextureAsync(chara.ParentMCharaId);
            statusValueView.SetCharacterVariable(viewData);
            skillScroll.SetItems(viewData.AbilityList);
        }

        protected override void OnReverseCharacterOrder()
        {
            if (selectingScrollData == null) return;
            foreach (var scrollData in GetItems().Where(scrollData => scrollData.id == selectingScrollData.id))
            {
                selectingScrollData = scrollData;
                selectingScrollData.IsSelecting = true;
            }

            SetBadge();
            RefreshItemView();
        }

        protected override void OnSortFilter()
        {
            var cellList = GetItems();

            
            selectingScrollData = cellList.FirstOrDefault(x => x.id == (selectingScrollData?.id ?? -1));
            if (selectingScrollData is null)
            {
                selectingChara = null;
                if (cellList.Count > 0)
                {
                    SetBadge();
                    OnSelectCharacter(cellList.FirstOrDefault());
                    SetCharacterViewActive(true);
                }
                else
                {
                    SetCharacterViewActive(false);
                    rankPowerChangesView.InitializeUI(currentSettingChara, selectingChara);
                }
                
                return;
            }
            
            SetCharacterViewActive(true);
            selectingScrollData.IsSelecting = true;
            SetBadge();
            RefreshItemView();
        }

        protected virtual void SetBadge()
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
            }
        }

        private void SetCharacterViewActive(bool hasCharacter)
        {
            characterCardBackgroundImage.gameObject.SetActive(hasCharacter);
            characterDetailRoot.gameObject.SetActive(hasCharacter);
            SetConfirmButtonInteractable();
        }
        
        protected virtual void SetConfirmButtonInteractable()
        {
            confirmButton.interactable = selectingChara is not null;
        }

        protected override void OnSwipeDetailModal(CharacterVariableScrollData scrollData)
        {
            OnSelectCharacter(scrollData);
        }
    }
}
