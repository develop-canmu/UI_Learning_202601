using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Combination;
using Pjfb.Master;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Deck
{
    public class DeckEditTopPage : Page
    {
        #region SerializeFields

        [SerializeField] private List<CharacterDeckImage> characterDeckImages;
        [SerializeField] protected ScrollGrid deckScrollGrid;
        [SerializeField] private Transform pagerContent;
        [SerializeField] private GameObject pager;
        [SerializeField] protected GameObject combinationMatchLockObject;
        [SerializeField] protected GameObject activatingCombinationMatchCountRoot;
        [SerializeField] protected TextMeshProUGUI activatingCombinationMatchCountText;

        [SerializeField] protected UIButton confirmButton;
        #endregion

        private List<Toggle> pagerList = new();
        protected DeckPageParameters parameters;

        protected virtual DeckListData DeckListData => DeckPage.DeckListData;
        protected virtual DeckData CurrentDeckData => DeckPage.CurrentDeckData;
        protected virtual long[] CurrentMemberIds => CurrentDeckData.GetMemberIds();
        protected virtual bool IsDeckChanged => CurrentDeckData.IsDeckChanged;
        protected virtual bool CanSaveEmptyDeck => false;
        
        protected virtual int CurrentDeckIndex
        {
            get => DeckPage.CurrentDeckIndex;
            set => DeckPage.CurrentDeckIndex = value;
        }

        
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            OnPreOpen(args);
            return base.OnPreOpen(args, token);
        }

        protected virtual void OnPreOpen(object args)
        {
            parameters = (DeckPageParameters)args;
            List<DeckScrollData> deckScrollDataList = new List<DeckScrollData>();
            foreach (var deckData in DeckListData.DeckDataList)
            {
                DeckScrollData data = new DeckScrollData
                {
                    DeckData = deckData,
                    GetRecommendationExcludedIdList = GetRecommendationExcludedIdList,
                    OnApplyDeckRecommendations = RefreshDeck,
                    OnClickReset = RefreshDeck,
                    OnClickChara = OnClickCharaThumbnail,
                    OnClickEditPosition = OnClickEditPosition,
                    OnStrategyChanged = RefreshDeck,
                };
                deckScrollDataList.Add(data);
            }

            deckScrollGrid.OnChangedPage -= OnChangeDeck;
            deckScrollGrid.OnChangedPage += OnChangeDeck;
            deckScrollGrid.SetItems(deckScrollDataList);

            deckScrollGrid.SetPage(CurrentDeckIndex, false);

            for (int i = pagerList.Count; i < deckScrollDataList.Count; i++) 
            {
                GameObject pagerObject = Instantiate(pager, pagerContent);
                pagerObject.SetActive(true);
                pagerList.Add(pagerObject.GetComponentInChildren<Toggle>());
            }

            SetCharacterDeckImage();
            // ページを開く音と重複するためOnValueChangedが発火しないようにする
            SetPager(true);
            SetConfirmButtonInteractable();
            SetCombinationMatchUi();
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
        

        #region EventListeners

        protected virtual void SetConfirmButtonInteractable()
        {
            confirmButton.interactable =
                (CurrentDeckData.IsDeckChanged || CurrentDeckIndex != DeckListData.SelectingIndex) && !CurrentDeckData.IsLocked && !CurrentDeckData.IsLockedPeriod;
        }

        protected void SetCombinationMatchUi()
        {
            var unlockCombination = CombinationManager.IsUnLockCombination();
            combinationMatchLockObject.SetActive(!unlockCombination);
            SetActivatingCombinationMatchCount(unlockCombination);
        }
        
        private void SetActivatingCombinationMatchCount(bool unlockCombination)
        {
            var mCharaIdList = CurrentMemberIds.Where(x => x != DeckUtility.EmptyDeckSlotId)
                .Select(x => (long)UserDataManager.Instance.charaVariable.Find(x).charaId).ToList();
            int activatingCount = CombinationManager.ActivatingCombinationMatchCount(mCharaIdList);
            activatingCombinationMatchCountRoot.SetActive(unlockCombination && activatingCount > 0);
            activatingCombinationMatchCountText.text = string.Format(StringValueAssetLoader.Instance["common.combination.current_active"], activatingCount);
        }

        protected virtual void OnClickCharaThumbnail(int id, int slotIndex)
        {
            DeckEditCharaSelectPage.DeckEditCharaSelectData args = new DeckEditCharaSelectPage.DeckEditCharaSelectData
            {
                OpenFrom = parameters != null ? parameters.OpenFrom : PageType.Home,
                SelectedCharaSlotIndex = slotIndex
            };


            DeckPage m = (DeckPage)Manager;
            m.OpenPage(DeckPageType.DeckEditCharaSelect, true, args);
        }
        
        public void OnClickConfirmEditButton()
        {
            TrySaveDeck().Forget();
        }

        protected virtual void OnClickEditPosition(int order)
        {
            PositionChangeModalWindow.Open(new PositionChangeModalWindow.WindowParams
            {
                CurrentRole = CurrentDeckData.GetMemberPosition(order),
                onChanged = (position) => SetRole(order, position),
                onClosed = null,
            });
        }

        protected virtual async void OnChangeDeck(int deckIndex)
        {
            if (CurrentDeckIndex == deckIndex) return;
            if (IsDeckChanged)
            {
                bool response = await OnLeaveCurrentDeck();

                if (!response)
                {
                    deckScrollGrid.SetPage(CurrentDeckIndex);
                    return;
                }
                DiscardCurrentDeckChanges();
            }
            
            CurrentDeckIndex = deckIndex;
            SetCharacterDeckImage();
            SetPager();
            SetConfirmButtonInteractable();
            SetCombinationMatchUi();
        }

        protected void SetCharacterDeckImage()
        {
            long[] idList = CurrentMemberIds;
            for (int i = 0; i < DeckUtility.BattleDeckSlotCount; i++)
            {
                if (i >= idList.Length || idList[i] == DeckUtility.EmptyDeckSlotId)
                {
                    characterDeckImages[i].gameObject.SetActive(false);
                    continue;
                }
                
                characterDeckImages[i].gameObject.SetActive(true);
                long mCharaId = UserDataManager.Instance.charaVariable.Find(idList[i])?.charaId ?? -1;
                characterDeckImages[i].SetTextureAsync(mCharaId).Forget();
            }
        }
        
        public async void NextDeck()
        {
            if (IsDeckChanged)
            {
                bool response = await OnLeaveCurrentDeck();
                if(!response)   return;
                DiscardCurrentDeckChanges();
            }
            deckScrollGrid.NextPage();
        }

        public async void PrevDeck()
        {
            if (IsDeckChanged)
            {
                bool response = await OnLeaveCurrentDeck();
                if(!response)   return;
                DiscardCurrentDeckChanges();
            }
            deckScrollGrid.PrevPage();
        }

        protected async UniTask<bool> OnLeaveCurrentDeck()
        {
            return await DeckUtility.OnLeaveCurrentDeck(this.GetCancellationTokenOnDestroy());
        }

        protected void DiscardCurrentDeckChanges()
        {
            CurrentDeckData.DiscardChanges();
            deckScrollGrid.RefreshItemView();
        }
        
        public void OnClickCombinationMatchButton()
        {
            if (!CombinationManager.IsUnLockCombination())
            {
                var systemLock = MasterManager.Instance.systemLockMaster.FindDataBySystemNumber(CombinationManager.CombinationLockId);            
                if(systemLock != null && !string.IsNullOrEmpty(systemLock.description))
                {
                    string description = systemLock.description;
                    ConfirmModalButtonParams button = new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], (m)=>m.Close());
                    ConfirmModalData data = new ConfirmModalData(StringValueAssetLoader.Instance["special_support.release_condition"], description, string.Empty, button);
                    AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, data);
                    return;
                }
            }
            
            CombinationMatchModal.Open(new CombinationMatchModal.WindowParams()
            {
                IdList = CurrentDeckData.GetMemberIds().Where(x => x != DeckUtility.EmptyDeckSlotId).Select(x => (long)UserDataManager.Instance.charaVariable.Find(x).charaId).ToList(),
            });
        }
        #endregion

        
        /// <summary>
        /// Pagerの更新, isOnWithoutNotifyがtrueの場合はOnValueChangedを発火させない
        /// </summary>
        protected void SetPager(bool isOnWithoutNotify = false)
        {
            if (isOnWithoutNotify == true)
            {
                pagerList[CurrentDeckIndex].SetIsOnWithoutNotify(true);
            }
            else
            {
                pagerList[CurrentDeckIndex].isOn = true;
            }
        }
        
        private void SetRole(int order, RoleNumber role)
        {
            CurrentDeckData.SetPosition(order, role);
            deckScrollGrid.RefreshItemView();
            SetConfirmButtonInteractable();
        }



        
        
        private void RefreshDeck()
        {
            SetCharacterDeckImage();
            SetConfirmButtonInteractable();
            SetCombinationMatchUi();
            deckScrollGrid.RefreshItemView();
        }
        
        protected virtual HashSet<long> GetRecommendationExcludedIdList(long deckIndex)
        {
            return new HashSet<long>();
        } 
        
        
        protected virtual async UniTask<bool> TrySaveDeck()
        {
            string deckFailedTitle = StringValueAssetLoader.Instance["character.deck_save_failed"];

            bool isFirstDeck = CurrentDeckData.Index == 0;

            bool skipRule = true;
            // Apply the rule if it is the first deck or if the deck is not empty.
            if (!CanSaveEmptyDeck || isFirstDeck || !CurrentDeckData.IsAllEmpty)
            {
                skipRule = false;
                if (CurrentDeckData.HasEmptySlot)
                {
                    ConfirmModalWindow.Open(new ConfirmModalData(deckFailedTitle,
                        StringValueAssetLoader.Instance["deck.empty_slot_caution"], "",
                        new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"],
                            (window => window.Close()))));
                    return false;
                }

                if (CurrentDeckData.HasEmptyRoleNumber)
                {
                    ConfirmModalWindow.Open(new ConfirmModalData(deckFailedTitle,
                        StringValueAssetLoader.Instance["deck.empty_position_caution"], "",
                        new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"],
                            (window => window.Close()))));
                    return false;
                }

                if (CurrentDeckData.HasDuplicateMCharaParentId)
                {
                    ConfirmModalWindow.Open(new ConfirmModalData(deckFailedTitle,
                        StringValueAssetLoader.Instance["deck.same_chara_caution"], "",
                        new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"],
                            (window => window.Close()))));
                    return false;
                }
            }
            


            string errorMessage = await CurrentDeckData.SaveDeckAsync(selectDeck: !skipRule,skipRule: skipRule);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ConfirmModalWindow.Open(new ConfirmModalData(deckFailedTitle, errorMessage, "",
                    new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"], (window => window.Close()))));
                return false;
            }
            
            

            UserDataManager.Instance.user.UpdateMaxDeckCombatPower(CurrentDeckData.CombatPower);
            
            SetConfirmButtonInteractable();
            SetCombinationMatchUi();
            ConfirmModalWindow.Open(new ConfirmModalData(StringValueAssetLoader.Instance["character.deckedit.leave_deck_confirm_title"],
                StringValueAssetLoader.Instance["deck.edit_confirmed"], "",
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"],
                    (window => window.Close()))));
            return true;
        }
        
    }
}
