using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Deck;
using Pjfb.UserData;

namespace Pjfb
{
    public class DeckEditUCharaSelectPage : CharacterListBasePage
    {
        public class PageParam
        {
            private PageType openFrom;
            public PageType OpenFrom => openFrom;
            private int selectedCharaSlotIndex;
            public int SelectedCharaSlotIndex => selectedCharaSlotIndex;
            public PageParam(PageType openFrom, int selectedCharaSlotIndex)
            {
                this.openFrom = openFrom;
                this.selectedCharaSlotIndex = selectedCharaSlotIndex;
            }
        }

        // 背景
        [SerializeField] 
        private CharacterCardBackgroundImage characterCardBackgroundImage;
        // スキル欄
        [SerializeField] 
        private ScrollGrid skillScroll;
        // 名前
        [SerializeField]
        private BaseCharacterNameView nameView;
      
        // 画面上部キャラの詳細親オブジェクト
        [SerializeField] 
        private GameObject characterDetailRoot;
        // 決定ボタン
        [SerializeField] 
        protected UIButton confirmButton;

        private PageParam deckEditCharaSelectData;
        
        private CharacterScrollData selectingScrollData;
        protected CharacterScrollData SelectingScrollData => selectingScrollData;
        
        private UserDataChara currentSettingChara;
        protected UserDataChara selectingChara;

        protected List<long> formattingIdList;
        protected long currentEditingId;
        protected virtual DeckData CurrentDeckData => DeckPage.CurrentDeckData;
        protected virtual DeckListData DeckListData => DeckPage.DeckListData; 
        private List<long> disableCharacterIdList = new List<long>();
        private long[] currentDeckMemberIdList;

        protected UserDataChara ShowingCharacter => selectingChara;
        
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            await Init(args);
            await base.OnPreOpen(args, token);
        }
        
        protected virtual async UniTask Init(object args)
        {
            deckEditCharaSelectData = (PageParam)args;
            currentEditingId = 0;
            formattingIdList = new List<long>();
            selectingChara = null;
            selectingScrollData = null;
            currentDeckMemberIdList = CurrentDeckData.GetMemberIds();
            
            for (int i = 0; i < currentDeckMemberIdList.Length; i++)
            {
                long id = currentDeckMemberIdList[i];
                if (i == deckEditCharaSelectData.SelectedCharaSlotIndex)
                {
                    currentEditingId = id;
                }
                else
                {
                    formattingIdList.Add(id);
                }
            }

            IEnumerable<long> excludeIdSet = currentDeckMemberIdList.Where(x => x != DeckUtility.EmptyDeckSlotId).Select(x => (long)x);
            
            SetFilterExcludeIdSet(excludeIdSet);
            currentSettingChara = UserDataManager.Instance.chara.Find(currentEditingId);
        
            characterScroll.SetUserCharacterList();
            SetLabel();
            characterScroll.SetCharacterList();
            Refresh();

            await OnSelectCharacterAsync(currentSettingChara == null ? GetItems().FirstOrDefault() : GetItemById(currentSettingChara.charaId));
        }
        
        protected override async UniTask OnSelectCharacterAsync(object value)
        {
            if (value is not CharacterScrollData data)
            {
                SetCharacterViewActive(false);
                selectingChara = null;
                return;
            }

            if ((selectingChara?.id ?? -1) == data.UserCharacterId) return;
            
            selectingChara = UserDataManager.Instance.chara.Find(data.UserCharacterId);
            SetCharacterViewActive(true);
            
            if (selectingScrollData != null)
            {
                selectingScrollData.IsSelecting = false;
            }

            selectingScrollData = data;
            selectingScrollData.IsSelecting = true;
            await RefreshItemViewAsync();
        }


        
        public void OnClickConfirmButton()
        {
            CurrentDeckData.SetMemberId(deckEditCharaSelectData.SelectedCharaSlotIndex, selectingChara.id);
            AppManager.Instance.UIManager.PageManager.PrevPage();
        }
        
        /// <summary> 詳細モーダルを開く </summary>
        public virtual void OnClickCharacterDetail()
        {
        }

        private async UniTask RefreshItemViewAsync()
        {
            RefreshItemView();
            await SetView();
        }
        
        // キャラ情報の表示
        private async UniTask SetView()
        {
            UserDataChara chara = ShowingCharacter;
            if(chara == null) return;
            SetSkillView();
            // 名前表示
            await nameView.InitializeUIAsync(chara);
            await characterCardBackgroundImage.SetTextureAsync(chara.ParentMCharaId);
        }

        protected virtual void SetSkillView()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnReverseCharacterOrder()
        {
            if (selectingScrollData == null) return;
            List<CharacterScrollData> charaList = GetItems();
            foreach (CharacterScrollData charaData in charaList)
            {
                if(charaData.CharacterId != selectingScrollData.CharacterId) return;
                selectingScrollData = charaData;
                selectingScrollData.IsSelecting = true;
                break;
            }

            SetLabel();
            RefreshItemView();
        }

        protected override void OnSortFilter()
        {
            List<CharacterScrollData> cellList = GetItems();
            
            selectingScrollData = cellList.FirstOrDefault(x => x.CharacterId == (selectingScrollData?.CharacterId ?? -1));
            if (selectingScrollData == null)
            {
                selectingChara = null;
                if (cellList.Count > 0)
                {
                    SetLabel();
                    OnSelectCharacter(cellList.FirstOrDefault());
                    SetCharacterViewActive(true);
                }
                else
                {
                    SetCharacterViewActive(false);
                }
                
                return;
            }
            
            SetCharacterViewActive(true);
            selectingScrollData.IsSelecting = true;
            SetLabel();
            RefreshItemView();
        }

        protected virtual void SetLabel()
        {
            // 指定の枠に現在編成しているキャラは別のラベルを出力する
            if (currentSettingChara != null)
            {
                characterScroll.SetCurrentSelectCharacterId(currentSettingChara.charaId);
            }
            else
            {
                // 編成しているキャラがいない場合は存在しないID(-1)を設定
                characterScroll.SetCurrentSelectCharacterId(-1);
            }
            characterScroll.SetSelectedCharacterIds(formattingIdList);
            // 他の編成に編成されているキャラは編成不可
            disableCharacterIdList = new List<long>();
            foreach (DeckData data in DeckListData.DeckDataList)
            {
                if(data == CurrentDeckData) continue;
                disableCharacterIdList.AddRange(data.GetMemberIds().Where(x => x != DeckUtility.EmptyDeckSlotId));
            }
            characterScroll.SetDisableDeckSlotCharacterIds(disableCharacterIdList);
        }

        private void SetCharacterViewActive(bool hasCharacter)
        {
            characterCardBackgroundImage.gameObject.SetActive(hasCharacter);
            characterDetailRoot.gameObject.SetActive(hasCharacter);
            SetConfirmButtonInteractable();
        }
        
        protected virtual void SetConfirmButtonInteractable()
        {
            // キャラが選択されているかつ編成不可じゃないとき
            confirmButton.interactable = selectingChara != null && disableCharacterIdList.Contains(selectingChara.id) == false;
        }

        protected override void OnSwipeDetailModal(CharacterScrollData scrollData)
        {
            OnSelectCharacter(scrollData);
        }
    }
}