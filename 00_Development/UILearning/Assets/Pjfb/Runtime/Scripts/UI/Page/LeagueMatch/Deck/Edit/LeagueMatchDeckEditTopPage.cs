using System;
using System.Threading;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pjfb.Deck;
using Pjfb.Extensions;
using Pjfb.UserData;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.LeagueMatch
{
    public class LeagueMatchDeckEditTopPage : DeckEditTopPage
    {
        protected override DeckListData DeckListData => LeagueMatchDeckPage.DeckListData;
        protected override DeckData CurrentDeckData => LeagueMatchDeckPage.CurrentDeckData;
        protected override long[] CurrentMemberIds => CurrentDeckData.GetMemberIds();
        protected override bool IsDeckChanged => CurrentDeckData.IsDeckChanged;
        protected override bool CanSaveEmptyDeck => true;

        protected override int CurrentDeckIndex
        {
            get => LeagueMatchDeckPage.CurrentDeckIndex; 
            set => LeagueMatchDeckPage.CurrentDeckIndex = value;
        }

        protected async override UniTask OnPreOpen(object args, CancellationToken token)
        {
            DeckListData.DeckDataList.ForEach(x => x.UpdateFatigueValue());
            
            // デッキロック情報取得
            DeckGetLockedListAPIRequest request = new DeckGetLockedListAPIRequest();
            await APIManager.Instance.Connect(request);
            DeckGetLockedListAPIResponse response = request.GetResponseData();
            // デッキリスト
            for(int i = 0; i < DeckListData.DeckDataList.Length; i++)
            {
                // 一度ロック解除する
                DeckListData.DeckDataList[i].IsLocked = false;
                // API結果
                for(int n = 0; n < response.lockedList.Length; n++)
                {
                    // ロックされてるか
                    bool isLocked = DeckListData.DeckDataList[i].PartyNumber == response.lockedList[n].partyNumber;
                    // ロック状態にする
                    if(isLocked)
                    {
                        DeckListData.DeckDataList[i].IsLocked = isLocked;
                        break;
                    }
                }
            }
            
            base.OnPreOpen(args);
        }

        public void OnClickTeamSummaryButton()
        {
            // TODO:ポップアップ必要か確認
            // ClubTeamSummaryWindow.Open(new ClubTeamSummaryWindow.WindowParams
            // {
            //     SelectingIndex = CurrentDeckIndex,
            //     DeckList = DeckListData.DeckDataList,
            //     OnClosed = OnChangedDeck,
            // });
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
        }

        protected override async UniTask<bool> TrySaveDeck()
        {
            bool saveSuccess = await base.TrySaveDeck();
            if (saveSuccess)
            {
                deckScrollGrid.RefreshItemView();
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
            
            LeagueMatchDeckPage m = (LeagueMatchDeckPage)Manager;
            m.OpenPage(LeagueMatchDeckPageType.LeagueMatchDeckEditCharaSelect, true, args);
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
                    if (count >= LeagueMatchDeckPage.MaxDuplicateMCharaCount)
                        excludeList.Add(uChara.id);
                }
            }
            
            return excludeList;
        }
        
        protected override void SetConfirmButtonInteractable()
        {
            confirmButton.interactable = CurrentDeckData.IsDeckChanged && !CurrentDeckData.IsLocked && !CurrentDeckData.IsLockedPeriod;
        }
    }
}
