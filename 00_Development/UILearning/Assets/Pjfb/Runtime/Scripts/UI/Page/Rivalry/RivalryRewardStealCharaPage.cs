using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Menu;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Storage;
using Pjfb.UI;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Rivalry
{
    public class RivalryRewardStealCharaPage : Page
    {
        #region Params
        public class Data
        {
            public HuntFinishAPIResponse huntFinishResponse;
            public HuntUserPending huntUserPending;
        }
        #endregion

        #region SerializeFields
        [SerializeField] private ScrollGrid charaStealGrid;
        [SerializeField] private ScrollGrid lotteryGrid;
        [SerializeField] private CharacterCardImage charaImage;
        [SerializeField] private TMP_Text charaNameText;
        [SerializeField] private UIButton lotteryButton;
        [SerializeField] private Animator effectBadgeAnimator;
        [SerializeField] private GameObject boostEffectBadge;
        [SerializeField] private GameObject passEffectBadge;
        #endregion

        protected Data data;
        private HuntEnemyMasterObject mHuntEnemy;
        private HuntTimetableMasterObject huntTimetableMasterObject;
        private HuntSpecificCharaMasterObject huntSpecificCharaMasterObject;
        private List<object> mPointIdList;
        private List<CharacterScrollData> charaList;
        protected HuntPrizeSet[] prizeSetList;
        protected HuntPrizeCorrection[] prizeCorrectionList;
        private NativeApiAutoSell autoSell;
        private int selectedIndex;
        private long effectBoostValue;

        #region OverrideMethods
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            data = (Data) args;
            Init();
            await base.OnPreOpen(args, token);
        }

        protected override UniTask<bool> OnPreClose(CancellationToken token)
        {
            AppManager.Instance.UIManager.System.TouchGuard.Hide();
            return base.OnPreClose(token);
        }
        #endregion
        
        #region PrivateMethods
        private async void Init()
        {
            charaList = new List<CharacterScrollData>();
            var mHuntEnemyId = data.huntFinishResponse != null ? data.huntFinishResponse.mHuntEnemyId : data.huntUserPending.mHuntEnemyId;
            mHuntEnemy = MasterManager.Instance.huntEnemyMaster.FindData(mHuntEnemyId);
            huntTimetableMasterObject = MasterManager.Instance.huntTimetableMaster.FindOpenRivalryDataWithHuntId(mHuntEnemy.mHuntId);
            var huntTimetableMasterObjectId = huntTimetableMasterObject?.id ?? 0;
            huntSpecificCharaMasterObject = RivalryManager.GetRewardBoost(huntTimetableMasterObjectId);
            mPointIdList = huntSpecificCharaMasterObject != null ? (List<object>)MiniJSON.Json.Deserialize(huntSpecificCharaMasterObject.mPointIdList) : new List<object>();
            effectBoostValue = await RivalryManager.GetRewardBoostValueAsync(huntTimetableMasterObjectId);
            foreach (var mCharaId in mHuntEnemy?.mCharaNpcIdList)
            {
                var mCharaNpc = MasterManager.Instance.charaNpcMaster.FindData(mCharaId);
                var charaData = new CharacterScrollData(mCharaNpc.mCharaId, 1, 0, -1);
                charaList.Add(charaData);
            }
            charaStealGrid.SetItems(charaList);
            
            selectedIndex = charaList.FindIndex(data => data.CharacterId == LocalSaveManager.saveData.stealCharaEnemyDataContainer.GetStealCharaId(mHuntEnemy.id));
            if (selectedIndex < 0 || selectedIndex >= charaList.Count) selectedIndex = 0;
            SetSelectedChara();
            InitLotteryGrid();
            InitCharaInfo();
            OnFinishedInit();
        }

        private void InitCharaInfo()
        {
            var chara = MasterManager.Instance.charaMaster.FindData(charaList[selectedIndex].CharacterId);
            charaImage.SetTextureAsync(chara.id).Forget();
            charaNameText.text = chara.name;
        }

        private void InitLotteryGrid()
        {
            var itemList = new List<PrizeJsonGridItem.Data>();
            var choiceNumber = mHuntEnemy.choiceNumberList[selectedIndex];
            // 選手ブースト
            var mHuntEnemyPrizeList = MasterManager.Instance.huntEnemyPrizeMaster.FindChoiceReward(mHuntEnemy.mHuntId, mHuntEnemy.difficulty, mHuntEnemy.rarity, choiceNumber);
            var isActiveBoostEffect = false;
            // サブスク効果
            var activePassEffectList = UserDataManager.Instance.GetActivePointGetBonus();
            var isActivePass = false;
            foreach (var mHuntEnemyPrize in mHuntEnemyPrizeList)
            {
                foreach (var prize in mHuntEnemyPrize?.prizeJson)
                {
                    long totalBoostValue = 0;
                    if (prize.args.valueOriginal <= 0) prize.args.valueOriginal = prize.args.value;
                    // 選手ブースト
                    var isActiveBoostEffectReward = huntSpecificCharaMasterObject != null && effectBoostValue > 0 && mPointIdList.Contains((long) prize.args.mPointId);
                    if (isActiveBoostEffectReward) 
                    {
                        isActiveBoostEffect = true;
                        totalBoostValue += effectBoostValue;
                    }
                    // サブスク効果
                    var isActivePassReward = false;
                    foreach (var activePass in activePassEffectList)
                    {
                        if (activePass.mPointId == prize.args.mPointId)
                        {
                            isActivePass = true;
                            isActivePassReward = true;
                            totalBoostValue += (activePass.rate / 100);
                        }
                    }
                    prize.args.value = prize.args.valueOriginal + (long)Math.Round((float)totalBoostValue * prize.args.valueOriginal / 100, 0, MidpointRounding.AwayFromZero);
                    itemList.Add(new PrizeJsonGridItem.Data(prize, isActiveBoostEffectReward || isActivePassReward));
                }
            }
            lotteryGrid.SetItems(itemList);
            lotteryButton.interactable = true;
            if (effectBadgeAnimator != null) effectBadgeAnimator.enabled = isActivePass && isActiveBoostEffect;
            if (boostEffectBadge != null) boostEffectBadge.SetActive(isActiveBoostEffect);
            if (passEffectBadge != null) passEffectBadge.SetActive(isActivePass);
        }

        private void SetSelectedChara()
        {
            for (int i = 0; i < charaList.Count; i++)
            {
                var chara = charaList[i];
                var isSelected = i == selectedIndex;
                CharacterScrollDataOptions options = CharacterScrollDataOptions.StealReward;
                if(isSelected)options |= CharacterScrollDataOptions.Selected;
                charaList[i] = new CharacterScrollData(chara.CharacterId, chara.CharacterLv, chara.LiberationLv,
                    chara.UserCharacterId, null, options);
                charaList[i].IsSelecting = isSelected;
            }
            charaStealGrid.SetItems(charaList);
        }

        private void NextTransition()
        {
            var nextPageData = new RivalryRewardLotteryConfirmPage.Data();
            nextPageData.prizeSetList = prizeSetList;
            nextPageData.prizeCorrectionList = prizeCorrectionList;
            nextPageData.autoSell = autoSell;
            nextPageData.mHuntEnemyId = data.huntFinishResponse != null ? data.huntFinishResponse.mHuntEnemyId : data.huntUserPending.mHuntEnemyId;
            nextPageData.mHuntTimetableId = data.huntFinishResponse != null ? data.huntFinishResponse.mHuntTimetableId : data.huntUserPending.mHuntTimetableId;
            RivalryPage m = (RivalryPage)Manager;
            m.OpenPage(RivalryPageType.RivalryRewardLotteryConfirm, true, nextPageData);
        }
        #endregion

        #region ProtectedMethods
        protected void OnFinishedInit()
        {

        }
        #endregion
        
        #region API
        private async UniTask GetHuntChoicePrizeAPI(long choiceNumber)
        {
            HuntChoicePrizeAPIRequest request = new HuntChoicePrizeAPIRequest();
            HuntChoicePrizeAPIPost post = new HuntChoicePrizeAPIPost();
            post.choiceNumber = choiceNumber;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            HuntChoicePrizeAPIResponse response = request.GetResponseData();
            prizeSetList = response.prizeSetList;
            prizeCorrectionList = response.prizeCorrectionList;
            autoSell = response.autoSell;
        }

        #endregion
        
        #region EventListeners
        public async void OnClickLottery()
        {
            lotteryButton.interactable = false;
            AppManager.Instance.UIManager.System.TouchGuard.Show();
            var choiceNumber = mHuntEnemy.choiceNumberList[selectedIndex];
            await GetHuntChoicePrizeAPI(choiceNumber);
            //選択を保存
            LocalSaveManager.saveData.stealCharaEnemyDataContainer.UpdateStealCharaId(mHuntEnemy.id, charaList[selectedIndex].CharacterId);

            RivalryPage m = (RivalryPage)Manager;
            m.InitRewardTransition(charaList[selectedIndex].CharacterId, NextTransition, prizeSetList);
            m.OpenRewardTransition();
        }

        public void OnClickStealChara(CharacterScrollItem item)
        {
            selectedIndex = charaList.FindIndex(chara => chara.CharacterId == item.characterData.CharacterId);
            SetSelectedChara();
            InitLotteryGrid();
            InitCharaInfo();
        }
        
        #endregion
    }
}
