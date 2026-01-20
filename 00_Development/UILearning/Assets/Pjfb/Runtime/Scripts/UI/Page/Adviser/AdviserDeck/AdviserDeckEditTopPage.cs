using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Deck;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.SystemUnlock;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Adviser
{
    public class AdviserDeckEditTopPage : UCharaDeckEditTopPageBase
    {
        protected override DeckData CurrentDeckData => AdviserDeckPage.CurrentDeckData;
        
        [SerializeField]
        private TextMeshProUGUI descriptionText;
        
        [SerializeField]
        private Image[] lockImages;
        // デッキのロック状態
        private bool[] isLockList = new bool[DeckUtility.BattleDeckSlotCount];
        
        // 編成のロックオブジェクト
        [SerializeField]
        private GameObject lockCoverObject;
        
        // おまかせ編成ボタン
        [SerializeField]
        private UIButton recommendFormationButton;
        
        // リセットボタン
        [SerializeField]
        private UIButton resetButton;
        
        // おまかせ編成比較パラメータ
        private class RecommendSortParam
        {
            // レベル
            private long level;
            public long Level => level;
            // エールスキル所持数
            private int yellSkillCount;
            public int YellSkillCount => yellSkillCount;
            // エールスキル合計レベル
            private long yellSkillLevelSum;
            public long YellSkillLevelSum => yellSkillLevelSum;
            // サポートスキル所持数
            private int supportSkillCount;
            public int SupportSkillCount => supportSkillCount;
            // サポートスキル合計レベル
            private long supportSkillLevelSum;
            public long SupportSkillLevelSum => supportSkillLevelSum;
            // 現在のレアリティ
            private long currentRarityId;
            public long CurrentRarityId => currentRarityId;
            // レアリティID
            private long rarityId;
            public long RarityId => rarityId;
            // キャラID
            private long charaId;
            public long CharaId => charaId;
            public RecommendSortParam(UserDataChara charaData)
            {
                level = charaData.level;
                List<CharaAbilityInfo> yellSkillList = CharaAbilityUtility.GetAbilityAcquireList(BattleConst.AbilityType.GuildBattleManual, charaData.charaId, charaData.level, charaData.newLiberationLevel);
                yellSkillCount = yellSkillList.Count;
                yellSkillLevelSum = yellSkillList.Sum(x => x.SkillLevel);
                List<CharaAbilityInfo> supportSkillList = CharaAbilityUtility.GetAbilityAcquireList(BattleConst.AbilityType.GuildBattleAuto, charaData.charaId, charaData.level, charaData.newLiberationLevel);
                supportSkillCount = supportSkillList.Count;
                supportSkillLevelSum = supportSkillList.Sum(x => x.SkillLevel);
                currentRarityId = RarityUtility.GetRarityId(charaData.MChara.id, charaData.newLiberationLevel);
                rarityId = charaData.MChara.mRarityId;
                charaId = charaData.MChara.id;
            }
        }
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            // 編成そのもののロック
            DeckGetLockedListAPIRequest request = new DeckGetLockedListAPIRequest();
            await APIManager.Instance.Connect(request);
            DeckGetLockedListAPIResponse response = request.GetResponseData();
            // ロック状態の更新
            CurrentDeckData.IsLocked = response.lockedList.Any(data => CurrentDeckData.PartyNumber == data.partyNumber);
            bool isDeckLock = CurrentDeckData.IsLocked || CurrentDeckData.IsLockedPeriod;
            // ロックカバーの表示
            lockCoverObject.SetActive(isDeckLock);
            // 各種ボタンの有効化
            recommendFormationButton.interactable = !isDeckLock;
            resetButton.interactable = !isDeckLock;
            
            // デッキスロットのロック
            for (int i = 0; i < DeckUtility.BattleDeckSlotCount; i++)
            {
                bool isLocked = UserDataManager.Instance.IsUnlockSystem((long)SystemUnlockDataManager.SystemUnlockNumber.ClubRoyalAdviser + i) == false;
                isLockList[i] = isLocked;
                lockImages[i].gameObject.SetActive(isLocked);
            }
            
            // 説明文の設定
            descriptionText.text = DeckUtility.GetConditionMaster(DeckFormatIdType.Adviser).description;
            
            await base.OnPreOpen(args, token);
        }

        protected override UniTask OnOpen(object args)
        {
            // 初回のみ遊び方を表示
            CheckShowHowToPlay().Forget();
            return base.OnOpen(args);
        }

        public override void OnClickCharaThumbnail(int slotIndex)
        {
            PageType from = parameters?.OpenFrom ?? PageType.Home;
            // アドバイザー
            DeckEditUCharaSelectPage.PageParam args = new DeckEditUCharaSelectPage.PageParam(from, slotIndex);
        
            AdviserDeckPage m = (AdviserDeckPage)Manager;
            m.OpenPage(AdviserDeckPage.DeckPageType.Select, true, args);
        }
        
        protected override void SetConfirmButtonInteractable()
        {
            confirmButton.interactable = CurrentDeckData.IsDeckChanged && !CurrentDeckData.IsLocked && !CurrentDeckData.IsLockedPeriod;
        }

        public override void OnClickRandomFormationButton()
        {
            // おまかせ編成の取得
            CharacterDetailData[] recommendList = GetRecommendFormatData();

            if (recommendList == null || recommendList.Length == 0)
            {
                ConfirmModalData confirmData = new ConfirmModalData
                (
                    StringValueAssetLoader.Instance["character.deck_recommendations"],
                    StringValueAssetLoader.Instance["adviser.club_royal.recommend.no_target"],
                    string.Empty,
                    new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], (window => window.Close()))
                );
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.Confirm, confirmData);
                return;
            }
            
            RecommendConfirm(recommendList).Forget();
        }

        private async UniTask RecommendConfirm(CharacterDetailData[] recommendList)
        {
            // ランダム確認モーダルを開く
            UCharaDeckRecommendationsConfirmModalWindow.ModalParams data = new UCharaDeckRecommendationsConfirmModalWindow.ModalParams(CurrentMemberDetailList, recommendList); 
            ModalWindow window = (ModalWindow)await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.AdviserDeckRecommendationsConfirm, data); 
            if((bool)await window.WaitCloseAsync() == false) return; 
            
            OnApplyRecommendFormation(recommendList);
        }
        
        
        protected override void SetDeckImageActive(int slotNum,bool isActive)
        {
            if (isLockList[slotNum])
            {
                // ロックされている場合true固定にする
                isActive = true;    
            }
            base.SetDeckImageActive(slotNum, isActive);
        }
        
        protected override CharacterDetailData[] GetRecommendFormatData()
        {
            CharacterDetailData[] recommendList = new CharacterDetailData[DeckUtility.BattleDeckSlotCount];
            List<long> parentIdList = new List<long>();
            
            UserDataChara[] uCharList = UserDataManager.Instance.GetUserDataCharaListByType(CardType.Adviser);
            // アドバイザーデータがない場合はnullを返す
            if(uCharList == null || uCharList.Length == 0) return null;
            Dictionary<long, RecommendSortParam> recommendCharaSortParamDict = new Dictionary<long, RecommendSortParam>();
            foreach (UserDataChara uChara in uCharList)
            {
                recommendCharaSortParamDict.Add(uChara.charaId, new RecommendSortParam(uChara));
            }
            
            // レベル > エールスキル所持数 > エールスキル合計レベル > サポートスキル所持数 > サポートスキル合計レベル > 現在のレアリティ > レアリティID降順 > キャラID降順
            var sortList = uCharList.OrderByDescending(x => recommendCharaSortParamDict[x.charaId].Level)
                .ThenByDescending(x => recommendCharaSortParamDict[x.charaId].YellSkillCount)
                .ThenByDescending(x => recommendCharaSortParamDict[x.charaId].YellSkillLevelSum)
                .ThenByDescending(x => recommendCharaSortParamDict[x.charaId].SupportSkillCount)
                .ThenByDescending(x => recommendCharaSortParamDict[x.charaId].SupportSkillLevelSum)
                .ThenByDescending(x => recommendCharaSortParamDict[x.charaId].CurrentRarityId)
                .ThenByDescending(x => recommendCharaSortParamDict[x.charaId].RarityId)
                .ThenByDescending(x => recommendCharaSortParamDict[x.charaId].CharaId);
            
            int cnt = 0;
            foreach (UserDataChara charaData in sortList)
            {
                if (isLockList[cnt] == false)
                {
                    if(parentIdList.Contains(charaData.MChara.parentMCharaId)) continue;
                    recommendList[cnt] = new CharacterDetailData(charaData);
                    parentIdList.Add(charaData.MChara.parentMCharaId);
                }
                cnt++;
                if (cnt >= DeckUtility.BattleDeckSlotCount) break;
            }
            return recommendList;
        }
        
        /// <summary>スキル表示ボタン</summary>
        public override void OnClickSkillListViewButton()
        {
            AdviserSkillListModalWindow.ModalParam param = new AdviserSkillListModalWindow.ModalParam(CurrentMemberDetailList);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.AdviserSkillList, param);
        }
        
        // 初回のみ遊び方を表示
        private async UniTask CheckShowHowToPlay()
        {
            long tutorialId = (long)HowToPlayUtility.TutorialType.AdviserDeck;
            if (LocalSaveManager.saveData.tutorialIdConfirmList.Contains(tutorialId) == false)
            {
                CruFramework.Page.ModalWindow howToModal = await HowToPlayUtility.OpenHowToPlayModal(tutorialId, StringValueAssetLoader.Instance["character.detail_modal.adviser.title"]);
                await howToModal.WaitCloseAsync(this.GetCancellationTokenOnDestroy());
                LocalSaveManager.saveData.tutorialIdConfirmList.Add(tutorialId);
                LocalSaveManager.Instance.SaveData();
            }
        }
        
        protected override async UniTask<bool> TrySaveDeck()
        {
            // ロールの指定
            for (int i = 0; i < CurrentDeckData.SlotCount; i++)
            {
                if (CurrentDeckData.GetMemberId(i) != DeckUtility.EmptyDeckSlotId)
                {
                    CurrentDeckData.SetPosition(i, RoleNumber.Adviser);  
                }
                else
                {
                    CurrentDeckData.SetPosition(i, RoleNumber.None);
                }
            }
            return await base.TrySaveDeck();
        }
        
    }
}