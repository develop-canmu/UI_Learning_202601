using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pjfb.ClubMatch;
using Pjfb.Combination;
using Pjfb.Deck;
using Pjfb.Extensions;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.ClubDeck
{
    public class ClubDeckEditTopPage : DeckEditTopPage
    {
        [SerializeField] private ConditionView conditionView;

        protected override DeckListData DeckListData => ClubDeckPage.DeckListData;
        protected override DeckData CurrentDeckData => ClubDeckPage.CurrentDeckData;
        protected override long[] CurrentMemberIds => CurrentDeckData.GetMemberIds();
        protected override bool IsDeckChanged => CurrentDeckData.IsDeckChanged;
        protected override bool CanSaveEmptyDeck => true;
        protected override int CurrentDeckIndex
        {
            get => ClubDeckPage.CurrentDeckIndex; 
            set => ClubDeckPage.CurrentDeckIndex = value;
        }

        protected override void OnPreOpen(object args)
        {
            DeckListData.DeckDataList.ForEach(x => x.UpdateFatigueValue());
            SetConditionView();
            base.OnPreOpen(args);
        }
        
        protected override void OnOpened(object args)
        {
            base.OnOpened(args);
            AppManager.Instance.TutorialManager.OpenClubDeckTutorialAsync().Forget();
        }

        public void OnClickTeamSummaryButton()
        {
            ClubTeamSummaryWindow.Open(new ClubTeamSummaryWindow.WindowParams
            {
                SelectingIndex = CurrentDeckIndex,
                DeckList = DeckListData.DeckDataList,
                OnClosed = OnChangedDeck,
            });
        }
        
        private void OnChangedDeck(long index)
        {
            deckScrollGrid.SetPage((int)index, true);
        }

        protected override async void OnChangeDeck(int deckIndex)
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
            SetConditionView();
        }

        private void SetConditionView()
        {
            conditionView.SetCondition(ClubDeckPage.CurrentDeckData.FixedClubConditionData);
        }

        protected override async UniTask<bool> TrySaveDeck()
        {

            if (CurrentDeckData.FixedClubConditionData.condition is not ClubDeckCondition.Awful)
            {
                DateTime expireTime = CurrentDeckData.Deck.tiredness.expireAt.TryConvertToDateTime();
                if (expireTime.IsFuture(AppTime.Now) && CurrentDeckData.HasNewCharacterFormed)
                {
                    bool result = false;
                    var window = await ConfirmModalWindow.OpenAsync(new ConfirmModalData(
                            StringValueAssetLoader.Instance["club.deck.condition_change_title"], StringValueAssetLoader.Instance["club.deck.condition_change_content"],"",
                            new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"],
                                window =>
                                {
                                    result = true;
                                    window.Close();
                                }),
                            new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.cancel"],
                                window =>
                                {
                                    result = false;
                                    window.Close();
                                })
                        ),
                        this.GetCancellationTokenOnDestroy());

                    await window.WaitCloseAsync();
                    if (!result)
                    {
                        return false;
                    }
                }
            }
            
            bool saveSuccess = await base.TrySaveDeck();
            if (saveSuccess)
            {
                deckScrollGrid.RefreshItemView();
                SetConditionView();    
            }

            return saveSuccess;
        }

        protected override void OnClickCharaThumbnail(int id, int slotIndex)
        {
            DeckEditCharaSelectPage.DeckEditCharaSelectData args = new DeckEditCharaSelectPage.DeckEditCharaSelectData
            {
                OpenFrom = parameters?.OpenFrom ?? PageType.Home,
                SelectedCharaSlotIndex = slotIndex
            };
            
            ClubDeckPage m = (ClubDeckPage)Manager;
            m.OpenPage(ClubDeckPageType.ClubDeckEditCharaSelect, true, args);
        }
        
        protected override HashSet<long> GetRecommendationExcludedIdList(long deckIndex)
        {
            Dictionary<long, long> mCharaCountDictionary= new Dictionary<long, long>();     // Dictionary<mCharaId, count>

            HashSet<long> excludeList = new();
            
            
            foreach (var deckData in DeckListData.DeckDataList)
            {
                if(deckData.Index == deckIndex) continue;
                foreach (var id in deckData.GetMemberIds())
                {
                    if(id == DeckUtility.EmptyDeckSlotId)   continue;
                    var uChara = UserDataManager.Instance.charaVariable.Find(id);
                    if (uChara is null) continue;
                    long mCharaId = uChara.charaId;
                    
                    // add editing id to exclude list
                    excludeList.Add(uChara.id);
                    if (mCharaCountDictionary.ContainsKey(mCharaId))
                    {
                        mCharaCountDictionary[mCharaId] += 1;
                    }
                    else
                    {
                        mCharaCountDictionary[mCharaId] = 1;
                    }
                }
            }

            // add id which reaches mChara limit count
            foreach (var uChara in UserDataManager.Instance.charaVariable.data.Values)
            {
                if (mCharaCountDictionary.TryGetValue(uChara.charaId, out long count))
                {
                    if (count >= ClubDeckPage.MaxDuplicateMCharaCount)
                        excludeList.Add(uChara.id);
                }
            }
            
            
            return excludeList;
        }

        protected override void SetConfirmButtonInteractable()
        {
            confirmButton.interactable = CurrentDeckData.IsDeckChanged && !CurrentDeckData.IsLocked && !CurrentDeckData.IsLockedPeriod;
        }

        public void SetCombinationMatchUi(List<long> mCharaIds)
        {
            var unlockCombination = CombinationManager.IsUnLockCombination();
            combinationMatchLockObject.SetActive(!unlockCombination);
            int activatingCount = CombinationManager.ActivatingCombinationMatchCount(mCharaIds);
            activatingCombinationMatchCountRoot.SetActive(unlockCombination && activatingCount > 0);
            activatingCombinationMatchCountText.text = string.Format(StringValueAssetLoader.Instance["common.combination.current_active"], activatingCount);
        }
    }
}
