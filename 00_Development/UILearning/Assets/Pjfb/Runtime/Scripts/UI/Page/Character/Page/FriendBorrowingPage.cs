using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pjfb.Character
{
    public class FriendBorrowingPage : CharacterListBasePage
    {
        [SerializeField] private BaseCharacterNameView nameView;
     
        
        private DeckData friendDeck;
        private CharacterScrollData selectingScrollData;
        private long currentSettingId => friendDeck.GetMemberId(0);
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            friendDeck = await DeckUtility.GetFriendDeck();
            SetFilterExcludeIdSet(new[] { currentSettingId });
            characterScroll.SetFriendSettingId(currentSettingId);
            characterScroll.SetUserCharacterList();
            characterScroll.SetCharacterList();
            Refresh();
            var settingData = GetItems().FirstOrDefault(x => x.UserCharacterId == currentSettingId);
            if(settingData is not null) settingData.IsSelecting = true;
            await OnSelectCharacterAsync(settingData);
            await base.OnPreOpen(args, token);
        }



        #region EventListeners

        protected override async UniTask OnSelectCharacterAsync(object value)
        {
            if (value is not CharacterScrollData data) return;

            if(selectingScrollData == data) return;
            if (selectingScrollData != null) selectingScrollData.IsSelecting = false;
            selectingScrollData = data;
            selectingScrollData.IsSelecting = true;
            await nameView.InitializeByIdAsync(selectingScrollData.UserCharacterId);
            RefreshItemView();
        }

    
        
        public async void OnClickDecideButton()
        {
            if (selectingScrollData != null)
            {
                friendDeck.SetMemberId(0, selectingScrollData.UserCharacterId);
                await friendDeck.SaveDeckAsync(false);
            }
            
            AppManager.Instance.UIManager.PageManager.PrevPage();
        }

        public void OpenBaseCharacterDetail()
        {
            var detailData = new CharacterDetailData(selectingScrollData.CharacterId, selectingScrollData.CharacterLv, selectingScrollData.LiberationLv);
            var modalParam = new BaseCharaDetailModalParams(new SwipeableParams<CharacterDetailData>(new List<CharacterDetailData>(){detailData}, 0));
            BaseCharacterDetailModal.Open(ModalType.BaseCharacterDetail, modalParam);
        }

        protected override void OnSortFilter()
        {
            CharacterScrollData newScrollData = ((selectingScrollData is not null)
                ? GetItems().FirstOrDefault(x => x.UserCharacterId == selectingScrollData.UserCharacterId)
                : GetItems().FirstOrDefault()) ?? GetItems().FirstOrDefault();

            selectingScrollData = null;
            characterScroll.Scroll.DeselectAllItems();
            OnSelectCharacter(newScrollData);
        }

        protected override void OnReverseCharacterOrder()
        {
            OnSortFilter();
        }

        protected override void OnSwipeDetailModal(CharacterScrollData scrollData)
        {
            OnSelectCharacter(scrollData);
        }

        #endregion
        
    }
}
