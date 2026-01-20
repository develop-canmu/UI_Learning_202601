using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Common;
using Pjfb.Master;
using Pjfb.Menu;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Rivalry;
using Pjfb.Story;
using Pjfb.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.MatchResult
{
    public class MatchResultWinPage : Page
    {
        #region Params
        public class Data
        {
            public PageType OpenFrom;
            public int BattleResult;
            public HuntFinishAPIResponse HuntFinishResponse;
            public long mvpMCharaId;

            public Data(PageType _OpenFrom, int _BattleResult, HuntFinishAPIResponse _HuntFinishResponse, long _mvpMCharaId)
            {
                OpenFrom = _OpenFrom;
                BattleResult = _BattleResult;
                HuntFinishResponse = _HuntFinishResponse;
                mvpMCharaId = _mvpMCharaId;
            }
        }
        #endregion

        #region SerializeFields
        [SerializeField] private CharacterCardImage charaImage;
        [SerializeField] private GameObject rewardPointRoot;
        [SerializeField] private PossessionItemUi rewardPointPossessionItemUi;
        [SerializeField] private ScrollGrid rewardItemGrid;
        [SerializeField] private Animator effectBadgeAnimator;
        [SerializeField] private GameObject boostEffectBadge;
        [SerializeField] private GameObject passEffectBadge;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button nextStageButton;
        [SerializeField] private UIBadgeBalloonCrossFade balloonCrossFade;
        [SerializeField] private TextMeshProUGUI staminaCostText;
        [SerializeField] private TextMeshProUGUI staminaNextStageCostText;
        [SerializeField] private TextMeshProUGUI backButtonText;
        #endregion

        protected Data data;
        private HuntEnemyMasterObject huntEnemyMasterObject;
        private HuntDifficultyMasterObject huntDifficultyMasterObject;
        private HuntMasterObject huntMasterObject;
        private HuntTimetableMasterObject huntTimetableMasterObject;
        private HuntSpecificCharaMasterObject huntSpecificCharaMasterObject;
        private HuntEnemyMasterObject nextEnemyMasterObject;
        private List<object> mPointIdList;
        private long effectBoostValue = 0;

        #region OverrideMethods
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            data = (Data) args;
            Init();
            await base.OnPreOpen(args, token);
        }

        protected override void OnEnablePage(object args)
        {
            AppManager.Instance.UIManager.Header.Hide();
            AppManager.Instance.UIManager.Footer.Hide();
            base.OnEnablePage(args);
        }
        #endregion
        
        #region PrivateMethods
        private async void Init()
        {
            huntEnemyMasterObject = MasterManager.Instance.huntEnemyMaster.FindData(data.HuntFinishResponse.mHuntEnemyId);
            huntMasterObject = MasterManager.Instance.huntMaster.FindData(huntEnemyMasterObject.mHuntId);
            huntDifficultyMasterObject = MasterManager.Instance.huntDifficultyMaster.FindData(huntEnemyMasterObject.difficulty);
            huntTimetableMasterObject = MasterManager.Instance.huntTimetableMaster.FindData(data.HuntFinishResponse.mHuntTimetableId);
            var huntTimetableMasterObjectId = huntTimetableMasterObject?.id ?? 0;
            huntSpecificCharaMasterObject = RivalryManager.GetRewardBoost(huntTimetableMasterObjectId);
            mPointIdList = huntSpecificCharaMasterObject != null ? (List<object>)MiniJSON.Json.Deserialize(huntSpecificCharaMasterObject.mPointIdList) : new List<object>();
            effectBoostValue = await RivalryManager.GetRewardBoostValueAsync(huntTimetableMasterObjectId);
            charaImage.SetTexture(data.mvpMCharaId);
            InitRewardPoint();
            InitRewardItem();
            InitEffectBadge();
            InitRetryButton();
            OnFinishedInit();
            InitBackButton();
        }

        private void InitRewardPoint()
        {
            var isEventMatch = huntTimetableMasterObject != null && huntTimetableMasterObject.type == 1 && huntTimetableMasterObject.mPointId >= 1;
            RectTransform rt = rewardItemGrid.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, isEventMatch ? 400 : 650);
            if (!isEventMatch)
            {
                rewardPointRoot.SetActive(false);
            }
            else
            {
                long rewardValue = 0;
                var isActivePassEffect = false;
                var isActiveBoostEffectReward = false;
                foreach (var prize in data.HuntFinishResponse.prizeSetList)
                {
                    foreach (var reward in prize.prizeJsonList)
                    {
                        if (reward.args.mPointId == huntTimetableMasterObject.mPointId)
                        {
                            rewardValue += reward.args.value;
                            // 加算補正倍率と補正が掛かる前の値は補正がかかった場合のみ存在するため２つの値が0より大きい場合はパスの効果が発動しているとする
                            if (!isActivePassEffect && reward.args.correctRate > 0 && reward.args.valueOriginal > 0)
                            {
                                isActivePassEffect = true;
                            }
                        isActiveBoostEffectReward = huntSpecificCharaMasterObject != null && effectBoostValue > 0 && mPointIdList.Contains((long) reward.args.mPointId);
                        }
                    }
                }
                rewardPointRoot.SetActive(true);
                rewardPointPossessionItemUi.SetCount(huntTimetableMasterObject.mPointId, rewardValue);
                if (isActivePassEffect || isActiveBoostEffectReward)
                {
                    rewardPointPossessionItemUi.SetColor(ColorValueAssetLoader.Instance["highlight.orange"]);
                }
            }
        }

        private void InitRewardItem()
        {
            if (data.HuntFinishResponse.prizeSetList is null ||
                data.HuntFinishResponse.prizeSetList.Length <= 0)
            {
                return;
            }
            
            var itemList = new List<PrizeJsonGridItem.Data>();
            foreach (var huntPrize in data.HuntFinishResponse.prizeSetList)
            {
                // 非表示報酬
                if (huntPrize.type == (int)HuntEnemyPrizeMasterObject.Type.HiddenGeneral) continue;
                
                foreach (var prize in huntPrize?.prizeJsonList)
                {
                    var isActiveBoostEffectReward = huntSpecificCharaMasterObject != null && effectBoostValue > 0 && mPointIdList.Contains((long) prize.args.mPointId);
                    itemList.Add(new PrizeJsonGridItem.Data(prize, isActiveBoostEffectReward));
                }
            }
            rewardItemGrid.SetItems(itemList);
            TryShowAutoSellConfirmModal(data.HuntFinishResponse.autoSell);
        }

        private async void InitRetryButton()
        {
            if (huntTimetableMasterObject == null) return;

            // リトライボタン
            var isEvent = RivalryManager.GetMatchType(huntTimetableMasterObject, huntMasterObject) == RivalryMatchType.Event;
            var huntResultList = await RivalryManager.Instance.GetHuntResultList();
            var usedLimit = huntResultList.FirstOrDefault(data => data.mHuntTimetableId == huntTimetableMasterObject.id)?.dailyPlayCount ?? 0;
            var remainingLimit = huntTimetableMasterObject?.dailyPlayCount - usedLimit;
            var canRetryLimit = (huntTimetableMasterObject?.dailyPlayCount > 0 && remainingLimit > 0) || huntTimetableMasterObject?.dailyPlayCount == 0;
            if (retryButton != null) 
            {
                retryButton.gameObject.SetActive(isEvent && !huntMasterObject.isClosedOnceWin && !huntMasterObject.hasChoicePrize);
                retryButton.interactable = canRetryLimit;
            }
            
            // 「次のステージへ」ボタン
            if (nextStageButton != null)
            {
                nextEnemyMasterObject = MasterManager.Instance.huntEnemyMaster.FindNextStageHunt(huntEnemyMasterObject);
                //１日の制限回数があるかどうか
                bool isLimitDailyLeft = huntTimetableMasterObject?.dailyPlayCount > 0;
                //イベントであるかつ次のステージが存在するかつ選択報酬がない場合に表示
                nextStageButton.gameObject.SetActive(isEvent && nextEnemyMasterObject != null && !huntMasterObject.hasChoicePrize);
                //消費スタミナの表示
                staminaNextStageCostText.text = string.Format(StringValueAssetLoader.Instance["common.stamina_usage"], huntMasterObject.useStaminaValue);
                if (isLimitDailyLeft)
                {
                    nextStageButton.interactable = remainingLimit > 0;
                }
            }


            // スタミナ消費なしの吹き出し
            if (balloonCrossFade != null)
            {
                var staminaId = (long)StaminaUtility.StaminaType.RivalryBattle;
                var staminaCost = huntMasterObject.useStaminaValue;
                var freeStamina = StaminaUtility.GetFreeStaminaRemainingUse(staminaId, staminaCost);

                // 吹き出しバッジ表示
                // 1. スタミナ消費なし
                // 2. 回数制限
                var staminaFreeCondition = freeStamina > 0;
                var staminaFreeString = string.Format(StringValueAssetLoader.Instance["stamina.free"], freeStamina);
                var limitCondition = remainingLimit > 0;
                var limitString = string.Format(StringValueAssetLoader.Instance[huntTimetableMasterObject?.playCountType == (long)HuntPlayCountType.Win ? "rivalry.match_limit.win" : "rivalry.match_limit.challenge"], remainingLimit);
                balloonCrossFade.SetView(staminaFreeCondition, limitCondition, staminaFreeString, limitString);
                staminaCostText.text = string.Format(StringValueAssetLoader.Instance["common.stamina_usage"], huntMasterObject.useStaminaValue);
            
            }
        }

        private void InitBackButton()
        {
            if (huntMasterObject.hasChoicePrize)
            {
                backButtonText.text = StringValueAssetLoader.Instance["common.next"];
            }
            else
            {
                backButtonText.text = StringValueAssetLoader.Instance["common.back"];
            }
        }

        private void InitEffectBadge()
        {
            // 加算補正倍率と補正が掛かる前の値は補正がかかった場合のみ存在するため２つの値が0より大きい場合はパスの効果が発動しているとする
            var isActivePassEffect = data.HuntFinishResponse.prizeCorrectionList?.FirstOrDefault(data => data.routeType == (long)HuntPrizeRouteType.Pass)?.rate > 0;
            var isActiveBoostEffect = data.HuntFinishResponse.prizeCorrectionList?.FirstOrDefault(data => data.routeType == (long)HuntPrizeRouteType.Chara)?.rate > 0;
            if (effectBadgeAnimator != null) effectBadgeAnimator.enabled = isActivePassEffect && isActiveBoostEffect;
            if (boostEffectBadge != null) boostEffectBadge.SetActive(isActiveBoostEffect);
            if (passEffectBadge != null) passEffectBadge.SetActive(isActivePassEffect);
        }

        private async void TryShowAutoSellConfirmModal(NativeApiAutoSell autoSell)
        {
            // 自動売却があるかどうかチェック
            if (autoSell?.prizeListGot != null && autoSell.prizeListSold != null &&
                (autoSell.prizeListGot.Length > 0 || autoSell.prizeListSold.Length > 0))
            {
                var autoSellModalData = new AutoSellConfirmModal.Data(autoSell);
                var modal = await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.AutoSellConfirm, autoSellModalData);
                await modal.WaitCloseAsync();
            }
        }
        #endregion

        #region ProtectedMethods
        protected void OnFinishedInit()
        {

        }
        #endregion
        
        #region EventListeners
        public async void OnClickNext()
        {
            switch (data.OpenFrom)
            {
                case PageType.Story:
                    await StoryManager.Instance.OnStoryBattleFinish(data.BattleResult, data.HuntFinishResponse);
                    break;
                case PageType.Rivalry:
                    var nextParams = new RivalryPage.Data();
                    var matchType = huntTimetableMasterObject.type == 1 && huntMasterObject.lotteryType == 1 ? RivalryMatchType.Event : RivalryMatchType.Regular;
                    var usedLimit = RivalryManager.Instance.huntResultList.FirstOrDefault(data => data.mHuntTimetableId == huntTimetableMasterObject.id)?.dailyPlayCount ?? 0;
                    bool isLimit = huntTimetableMasterObject.dailyPlayCount > 0 && usedLimit >= huntTimetableMasterObject.dailyPlayCount;
                    bool isStack = true;
                    
                    if (huntMasterObject.hasChoicePrize)
                    {
                        isStack = false;
                        nextParams.pageType = RivalryPageType.RivalryRewardStealChara;
                        nextParams.showHeaderAndFooter = false;
                        RivalryRewardStealCharaPage.Data postData = new RivalryRewardStealCharaPage.Data();
                        postData.huntFinishResponse = data.HuntFinishResponse;
                        nextParams.args = postData;
                    }
                    else if (matchType == RivalryMatchType.Event)
                    {
                        var HuntEnemyMasterObjectList = MasterManager.Instance.huntEnemyMaster.values.Where(aData => aData.mHuntId == huntMasterObject.id).ToList();
                        nextParams.pageType = RivalryPageType.RivalryEvent;
                        nextParams.args = new RivalryEventPage.PageParams{huntMasterObject = huntMasterObject, huntTimetableMasterObject = huntTimetableMasterObject, HuntEnemyMasterObjectList = HuntEnemyMasterObjectList, 
                            autoTransitToEventTop = isLimit};
                    }
                    else
                    {
                        if(isLimit == false)
                        {
                            nextParams.pageType = RivalryPageType.RivalryTeamSelect;
                            nextParams.args = new RivalryTeamSelectPage.PageParams(huntDifficultyMasterObject, huntMasterObject, huntTimetableMasterObject);
                        }
                    }
                    await AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.Rivalry, isStack, nextParams);
                    break;
                default: // failsafe
                    await AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.Home, true, null);
                    break;
            }
        }

        public void OnClickNextStage()
        {
            // ライバルリーのみ
            if (huntTimetableMasterObject.type != 1) return;
            
            //スタミナがあるかどうか
            bool isSuccess = KickOff(StaminaUtility.StaminaType.RivalryBattle, (long)StaminaUtility.StaminaType.RivalryBattle, huntMasterObject.useStaminaValue);
            if (!isSuccess) return;
            
            // 次のステージでの編成画面を開く
            AppManager.Instance.UIManager.PageManager.OpenPage(
                PageType.TeamConfirm,
                true,
                new TeamConfirmPage.PageParams(
                    PageType.Rivalry,
                    new RivalryPage.Data(RivalryPageType.RivalryEvent, nextEnemyMasterObject),
                    huntMasterObject,
                    huntTimetableMasterObject,
                    nextEnemyMasterObject
                )
            );
        }

        public void OnClickRetry()
        {
            // 現状ライバルリーのみ
            if (huntTimetableMasterObject.type != 1) return;

            var isSuccess = KickOff(StaminaUtility.StaminaType.RivalryBattle, (long)StaminaUtility.StaminaType.RivalryBattle, huntMasterObject.useStaminaValue);
            if (!isSuccess) return;
            if (SupportEquipmentManager.ShowOverLimitModal())
            {
                return;
            }
            RivalryManager.Instance.OnRivalryBattleStart(huntEnemyMasterObject.id, huntTimetableMasterObject.id);
        }

        private bool KickOff(StaminaUtility.StaminaType staminaType, long staminaId, long staminaValue)
        {
            var enoughStamina = CheckEnoughStamina(staminaId, staminaValue);
            
            // スタミナ不足かチェック
            if (!enoughStamina && staminaType != StaminaUtility.StaminaType.Colosseum)
            {
                var staminaData = new StaminaInsufficientModal.Data();
                staminaData.staminaType = staminaType;
                staminaData.mStaminaId = staminaId;
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.StaminaInsufficient, staminaData);
                return false;
            }

            return true;
        }

        private bool CheckEnoughStamina(long staminaId, long staminaValue)
        {
            var currentStamina = StaminaUtility.GetStamina(staminaId);
            var currentAdditionStamina = StaminaUtility.GetStaminaAddition(staminaId);
            return currentStamina >= staminaValue　|| currentAdditionStamina >= staminaValue;
        }
        #endregion
    }
}
