using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Deck;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;

namespace Pjfb.ClubRoyal
{
    public class ClubRoyalDeckEditTopPage : DeckEditTopPage
    {
        protected override DeckListData DeckListData => ClubRoyalDeckPage.DeckListData;
        
        protected override int CurrentDeckIndex
        {
            get => ClubRoyalDeckPage.CurrentDeckIndex; 
            set => ClubRoyalDeckPage.CurrentDeckIndex = value;
        }
        
        protected override DeckData CurrentDeckData => ClubRoyalDeckPage.CurrentDeckData;
        protected override bool IsDeckChanged => CurrentDeckData.IsDeckChanged;
        protected override bool CanSaveEmptyDeck => true;
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            // デッキロック情報取得
            DeckGetLockedListAPIRequest request = new DeckGetLockedListAPIRequest();
            await APIManager.Instance.Connect(request);
            DeckGetLockedListAPIResponse response = request.GetResponseData();
            // デッキリスト
            foreach (var deckData in DeckListData.DeckDataList)
            {
                // ロック状態の更新
                deckData.IsLocked = response.lockedList.Any(data => deckData.PartyNumber == data.partyNumber);
            }
            await base.OnPreOpen(args, token);
        }
        
        protected override void OnClickCharaThumbnail(int id, int slotIndex)
        {
            ClubRoyalDeckEditCharaSelectPage.PageParam args = new ClubRoyalDeckEditCharaSelectPage.PageParam
            {
                OpenFrom = parameters?.OpenFrom ?? PageType.Home,
                SelectedCharaSlotIndex = slotIndex
            };

            ClubRoyalDeckPage m = (ClubRoyalDeckPage)Manager;
            m.OpenPage(ClubRoyalDeckPage.DeckPageType.DeckEditCharaSelect, true, args);
        }

        protected override void SetConfirmButtonInteractable()
        {
            confirmButton.interactable = CurrentDeckData.IsDeckChanged && !CurrentDeckData.IsLocked && !CurrentDeckData.IsLockedPeriod;
        }
        
        /// <summary>編成済みキャラの取得</summary>
        protected override HashSet<long> GetRecommendationExcludedIdList(long deckIndex)
        {
            Dictionary<long, long> mCharaCountDictionary= new Dictionary<long, long>();

            HashSet<long> excludeList = new();
            
            foreach (DeckData deckData in DeckListData.DeckDataList)
            {
                if(deckData.Index == deckIndex) continue;
                foreach (long id in deckData.GetMemberIds())
                {
                    if(id == DeckUtility.EmptyDeckSlotId)   continue;
                    UserDataCharaVariable uChara = UserDataManager.Instance.charaVariable.Find(id);
                    if (uChara == null) continue;
                    long mCharaId = uChara.charaId;
                    
                    excludeList.Add(uChara.id);
                    if (!mCharaCountDictionary.TryAdd(mCharaId, 1))
                    {
                        mCharaCountDictionary[mCharaId] += 1;
                    }
                }
            }

            foreach (UserDataCharaVariable uChara in UserDataManager.Instance.charaVariable.data.Values)
            {
                if (mCharaCountDictionary.TryGetValue(uChara.charaId, out long count))
                {
                    if (count >= ClubRoyalDeckPage.MaxDuplicateMCharaCount)
                    {
                        excludeList.Add(uChara.id);
                    }
                }
            }
            
            return excludeList;
        }
    }
}