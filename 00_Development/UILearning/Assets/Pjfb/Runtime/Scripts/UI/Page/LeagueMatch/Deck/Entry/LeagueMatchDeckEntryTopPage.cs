using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pjfb.Deck;
using Pjfb.UserData;
using UnityEngine;
using TMPro;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Master;
using Pjfb.Extensions;

namespace Pjfb.LeagueMatch
{
    public enum Progress
    {
        None = 0,
        EntryOpen = 1, // エントリー受付
        EntryClose = 2, //エントリー受付終了
        InSession = 3, //開催中
        Aggregating = 4, //集計中
        MatchEnd = 5, //試合終了状態
    }

    public class LeagueMatchDeckEntryTopPage : DeckEditTopPage
    {
        private LeagueMatchDeckEntryPage.PageParams param;

        [SerializeField] private UIButton cancelButton;
        [SerializeField] private UIButton entryButton;
        [SerializeField] private TextMeshProUGUI entryText;

        protected override DeckListData DeckListData => LeagueMatchDeckEntryPage.DeckListData;
        protected override DeckData CurrentDeckData => LeagueMatchDeckEntryPage.CurrentDeckData;
        protected override long[] CurrentMemberIds => CurrentDeckData.GetMemberIds();
        protected override bool IsDeckChanged => CurrentDeckData.IsDeckChanged;
        protected override bool CanSaveEmptyDeck => true;
        protected override int CurrentDeckIndex
        {
            get => LeagueMatchDeckEntryPage.CurrentDeckIndex; 
            set => LeagueMatchDeckEntryPage.CurrentDeckIndex = value;
        }
        
        private BattleReserveFormationMasterObject mBattleReserveFormationMasterObject = null;
        private long remainingEntryCount = 0;

        protected async override UniTask OnPreOpen(object args, CancellationToken token)
        {
            param = (LeagueMatchDeckEntryPage.PageParams)args;
            LeagueMatchUtility.SetSelectedRoundNumber(param.RoundNumber);
            LeagueMatchInfo leagueMatchInfo = LeagueMatchUtility.GetLeagueMatchInfo(param.ColosseumEventMaster.clientHandlingType);
            
            if (leagueMatchInfo.SeasonData == null)
            {
                return;
            }
            
            mBattleReserveFormationMasterObject = MasterManager.Instance.battleReserveFormationMaster.FindData(leagueMatchInfo.SeasonData.MColosseumEvent.inGameSystemId);
            // ID未指定の場合、machInfoの取得先APIを切り替える
            if (param.Id == 0)
            {
                var shiftBattleStartTime = leagueMatchInfo.ShiftBattleStartAt;
                if (leagueMatchInfo.CanShiftBattle && DateTimeExtensions.IsFuture(AppTime.Now, shiftBattleStartTime))
                {
                    // データ確認
                    await LeagueMatchUtility.GetMatchInfoByEventInfo(
                        mBattleReserveFormationMasterObject.eventType,
                        leagueMatchInfo.SeasonData.UserSeasonStatus.sColosseumEventId
                    );
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (leagueMatchInfo.IsOnShiftBattle)
                {
                    if (leagueMatchInfo.CanShiftBattle)
                    {
                        await LeagueMatchUtility.GetMatchInfo(param.Id);
                    }
                    else
                    {
                        CruFramework.Logger.LogError("入れ替え戦にマッチングされていないグループ");
                        return;
                    }
                }
                else
                {
                    await LeagueMatchUtility.GetMatchInfo(param.Id);
                }
            }
            UpdateUI();
            await base.OnPreOpen(args, token);
        }

        public async void OnClickEntryButton()
        {
            BattleReserveFormationSetDeckAPIRequest request = new BattleReserveFormationSetDeckAPIRequest();
            BattleReserveFormationSetDeckAPIPost post = new()
            {
                id = param.Id,
                roundNumber = param.RoundNumber,
                partyNumber = CurrentDeckData.Deck.partyNumber,
            };
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            LeagueMatchUtility.SetMatchInfo(response.matchInfo);
            
            ConfirmModalWindow.Open(new ConfirmModalData(StringValueAssetLoader.Instance["league.match.entry_completion_title"],
                StringValueAssetLoader.Instance["league.match.entry_completion"], "",
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"],
                    (window => window.Close()))));
            
            remainingEntryCount = LeagueMatchUtility.MatchInfoCache.matchLineupList.Count(data => data.playerInfo.player.playerId == UserDataManager.Instance.user.uMasterId);
            UpdateUI();
            deckScrollGrid.RefreshItemView();
        }

        public async void OnClickCancelButton()
        {
            BattleReserveFormationSetDeckAPIRequest request = new BattleReserveFormationSetDeckAPIRequest();
            BattleReserveFormationSetDeckAPIPost post = new()
            {
                id = param.Id,
                roundNumber = param.RoundNumber,
                partyNumber = 0,
            };
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            LeagueMatchUtility.SetMatchInfo(response.matchInfo);
            ConfirmModalWindow.Open(new ConfirmModalData(StringValueAssetLoader.Instance["league.match.entry_cancel_title"],
                StringValueAssetLoader.Instance["league.match.entry_cancel"], "",
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"],
                    (window => window.Close()))));
            
            remainingEntryCount = LeagueMatchUtility.MatchInfoCache.matchLineupList.Count(data => data.playerInfo.player.playerId == UserDataManager.Instance.user.uMasterId);
            UpdateUI();
            deckScrollGrid.RefreshItemView();
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
            UpdateUI();
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
                    if (count >= LeagueMatchDeckEntryPage.MaxDuplicateMCharaCount)
                        excludeList.Add(uChara.id);
                }
            }
            
            return excludeList;
        }

        protected override void SetConfirmButtonInteractable()
        {
            confirmButton.interactable = !CurrentDeckData.LeagueMatchAlreadyEntered && CurrentDeckData.IsDeckChanged && !CurrentDeckData.HasEmptySlot;
        }

        private void UpdateUI()
        {
            if (LeagueMatchUtility.MatchInfoCache == null)
            {
                return;
            }
            
            mBattleReserveFormationMasterObject = MasterManager.Instance.battleReserveFormationMaster.FindData(LeagueMatchUtility.GetLeagueMatchInfo(param.ColosseumEventMaster.clientHandlingType).SeasonData.MColosseumEvent.inGameSystemId);
            
            // 自身のエントリー数取得
            var entryCount = 0;
            foreach (var matchLineup in LeagueMatchUtility.MatchInfoCache.matchLineupList)
            {
                if (matchLineup.playerInfo.partyNumber == 0) continue;
                if (matchLineup.playerInfo.player.playerId == UserDataManager.Instance.user.uMasterId)
                {
                    entryCount++;
                }
            }
            // 残りの登録可能数
            remainingEntryCount = mBattleReserveFormationMasterObject.oneUserReserveCount - entryCount;
            entryText.SetText(string.Format("{0}/{1}", remainingEntryCount, mBattleReserveFormationMasterObject.oneUserReserveCount));
            
            // 登録済みの自分のデッキか？
            bool myRegisteredDeck = false;
            foreach (var matchLineup in LeagueMatchUtility.MatchInfoCache.matchLineupList)
            {
                if (matchLineup.playerInfo.partyNumber == 0) continue;
                if (matchLineup.roundNumber == CurrentDeckData.Deck.partyNumber) continue;
                myRegisteredDeck = true;
            }
            if (remainingEntryCount == 0 && !myRegisteredDeck)
            {
                // 最大数エントリー済み
                cancelButton.gameObject.SetActive(false);
                cancelButton.interactable = false;
                entryButton.gameObject.SetActive(true);
                entryButton.interactable = false;
                
                return;
            }
            
            // 別スロットに登録済みか
            bool registeredAnotherSlot = false;
            foreach (var matchLineup in LeagueMatchUtility.MatchInfoCache.matchLineupList)
            {
                if (matchLineup.playerInfo.partyNumber == 0) continue;
                if (matchLineup.roundNumber == param.RoundNumber) continue;
                if (matchLineup.playerInfo.partyNumber != CurrentDeckData.Deck.partyNumber) continue;
                if (matchLineup.playerInfo.player.playerId != UserDataManager.Instance.user.uMasterId) continue;
                
                registeredAnotherSlot = true;
                break;
            }
            if (registeredAnotherSlot)
            {
                // 別スロット登録済み
                cancelButton.gameObject.SetActive(false);
                cancelButton.interactable = false;
                entryButton.gameObject.SetActive(true);
                entryButton.interactable = false;

                return;
            }
            
            // 既にこのスロットに登録済みか
            bool alreadyEntered = false;
            foreach (var matchLineup in LeagueMatchUtility.MatchInfoCache.matchLineupList)
            {
                if (matchLineup.playerInfo.partyNumber == 0) continue;
                if (matchLineup.playerInfo.player.playerId != UserDataManager.Instance.user.uMasterId) continue;
                if (matchLineup.roundNumber != param.RoundNumber) continue;
                if (matchLineup.playerInfo.partyNumber != CurrentDeckData.Deck.partyNumber) continue;
                alreadyEntered = true;
                break;
            }
            if (alreadyEntered)
            {
                // 既に登録済み
                cancelButton.gameObject.SetActive(true);
                cancelButton.interactable = true;
                entryButton.gameObject.SetActive(false);
                entryButton.interactable = true;

                return;
            }
            
            //エントリー可能か
            bool canEntry;
            if (LeagueMatchUtility.MatchInfoCache.progress == (long)Progress.EntryOpen && 
                remainingEntryCount > 0)
            {
                canEntry = true; 
            }
            else
            {
                canEntry = false;
            }
            if (canEntry)
            {
                // エントリー可能
                cancelButton.gameObject.SetActive(false);
                cancelButton.interactable = false;
                entryButton.gameObject.SetActive(true);
                entryButton.interactable = !CurrentDeckData.HasEmptySlot;

                return;
            }
            
            cancelButton.gameObject.SetActive(false);
            cancelButton.interactable = false;
            entryButton.gameObject.SetActive(true);
            entryButton.interactable = false;
        }
    }
}
