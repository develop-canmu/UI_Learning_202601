using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Deck;
using Pjfb.UserData;

namespace Pjfb
{
    public class UCharaDeckEditTopPageBase : Page
    {
        [SerializeField]
        private UCharaDeckView deckView;
        
        [SerializeField] 
        private List<CharacterDeckImage> characterDeckImages;
        
        [SerializeField] 
        protected UIButton confirmButton;
        
        protected DeckPageParameters parameters;
        protected virtual DeckData CurrentDeckData => DeckPage.CurrentDeckData;
        protected virtual long[] CurrentMemberIds => CurrentDeckData.GetMemberIds();
        private CharacterDetailData[] currentMemberDetailList = new CharacterDetailData[DeckUtility.BattleDeckSlotCount];
        protected CharacterDetailData[] CurrentMemberDetailList => currentMemberDetailList;
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            OnPreOpen(args);
            return base.OnPreOpen(args, token);
        }

        protected virtual void OnPreOpen(object args)
        {
            parameters = (DeckPageParameters)args;
            UpdateDeckView();
        }
        
        
        protected override async void OnEnablePage(object args)
        {
            base.OnEnablePage(args);
            if (parameters is { OpenFrom: PageType.TeamConfirm })
            {
                await BGMManager.PlayBGMAsync(BGM.bgm_gvg_top);
            }
            else
            {
                await BGMManager.PlayBGMAsync(BGM.bgm_home);
            }
        }

        // 確定ボタンの表示
        protected virtual void SetConfirmButtonInteractable()
        {
        }

        
        public virtual void OnClickCharaThumbnail(int slotIndex)
        {
            throw new System.NotImplementedException();
        }

        // 上部のキャラ画像の表示
        private void SetCharacterDeckImage()
        {
            long[] idList = CurrentMemberIds;
            for (int i = 0; i < DeckUtility.BattleDeckSlotCount; i++)
            {
                if (i >= idList.Length || idList[i] == DeckUtility.EmptyDeckSlotId)
                {
                    SetDeckImageActive(i, false);
                    continue;
                }
                
                SetDeckImageActive(i, true);
                characterDeckImages[i].SetTextureAsync(currentMemberDetailList[i].MCharaId).Forget();
            }
        }

        protected virtual void SetDeckImageActive(int slotNum,bool isActive)
        {
            characterDeckImages[slotNum].SetImageEnable(isActive);
        }
        
        public void OnClickDeckResetButton()
        {
            CurrentDeckData.SetDeckEmpty();
            UpdateDeckView();
        }

        private void UpdateDeckView()
        {
            currentMemberDetailList = new CharacterDetailData[DeckUtility.BattleDeckSlotCount];
            for (int i = 0; i < CurrentMemberIds.Length; i++)
            {
                UserDataChara charaData = UserDataManager.Instance.chara.Find(CurrentMemberIds[i]);
                if (charaData == null)
                {
                    currentMemberDetailList[i] = null;
                    continue;
                }
                CharacterDetailData detailData = new CharacterDetailData(charaData);
                currentMemberDetailList[i] = detailData;
            }
            deckView.InitView(currentMemberDetailList);
            SetCharacterDeckImage();
            SetConfirmButtonInteractable();
        }
        
        /// <summary>編成確定ボタン</summary>
        public void OnClickConfirmEditButton()
        {
            TrySaveDeck().Forget();
        }
        
        protected virtual async UniTask<bool> TrySaveDeck()
        {
            string deckFailedTitle = StringValueAssetLoader.Instance["character.deck_save_failed"];

            string errorMessage = await CurrentDeckData.SaveDeckAsync(selectDeck: true,skipRule: true, emptySlot: true);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ConfirmModalWindow.Open(new ConfirmModalData(deckFailedTitle, errorMessage, "",
                    new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], (window => window.Close()))));
                return false;
            }

            UserDataManager.Instance.user.UpdateMaxDeckCombatPower(CurrentDeckData.CombatPower);
            
            SetConfirmButtonInteractable();
            ConfirmModalWindow.Open(new ConfirmModalData(StringValueAssetLoader.Instance["character.deckedit.leave_deck_confirm_title"],
                StringValueAssetLoader.Instance["deck.edit_confirmed"], "",
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"],
                    (window => window.Close()))));
            return true;
        }
        
        public virtual void OnClickRandomFormationButton()
        {
        }
        
        // おまかせ編成確定
        protected void OnApplyRecommendFormation(CharacterDetailData[] recommendList)
        {
            for (int i = 0; i < CurrentDeckData.SlotCount; i++)
            {
                long uCharaId = recommendList[i] != null ? recommendList[i].UCharId : DeckUtility.EmptyDeckSlotId;
                CurrentDeckData.SetMemberId(i,uCharaId);
            }
            UpdateDeckView();
        }

        // おまかせ編成
        protected virtual CharacterDetailData[] GetRecommendFormatData()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>スキル表示ボタン</summary>
        public virtual void OnClickSkillListViewButton()
        {
            throw new System.NotImplementedException();
            //todo: スキル表示
        }
    }
}