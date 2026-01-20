using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Rivalry
{
    public class RivalryRewardLotteryConfirmPage : Page
    {
        readonly int startEffectInterval = 1000;

        #region Params
        public class Data
        {
            public HuntPrizeSet[] prizeSetList;
            public HuntPrizeCorrection[] prizeCorrectionList;
            public NativeApiAutoSell autoSell;
            public long mHuntEnemyId;
            public long mHuntTimetableId;
        }
        #endregion

        #region SerializeFields
        [SerializeField] private int prizeCountPerLine = 3;
        [SerializeField] private Transform content;
        [SerializeField] private RivalryRewardIcon prizeIcon;
        [SerializeField] private GameObject prizeLine;
        [SerializeField] private CharacterGetEffect getEffect = null;
        [SerializeField] GameObject skipTrigger = null;
        [SerializeField] private Animator effectBadgeAnimator;
        [SerializeField] private GameObject boostEffectBadge;
        [SerializeField] private GameObject passEffectBadge;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button nextStageButton;
        [SerializeField] private UIBadgeBalloonCrossFade balloonCrossFade;
        [SerializeField] private TextMeshProUGUI staminaCostText;
        [SerializeField] private TextMeshProUGUI staminaNextStageCostText;
        #endregion

        private Data data;
        private HuntEnemyMasterObject huntEnemyMasterObject;
        private HuntDifficultyMasterObject huntDifficultyMasterObject;
        private HuntMasterObject huntMasterObject;
        private HuntTimetableMasterObject huntTimetableMasterObject;
        private HuntEnemyMasterObject nextEnemyMasterObject;
        private List<PrizeJsonViewData> prizeList = null;
        private RivalryRewardIcon[] prizeIconList = null;
        private GameObject[] prizeLineList = null;
        private long effectBoostValue = 0;
        protected bool isTutorial = false;
        #region OverrideMethods
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            data = (Data) args;
            Init();
            await base.OnPreOpen(args, token);
        }

        protected override void OnOpened(object args)
        {
            RivalryPage m = (RivalryPage)Manager;
            m.CloseRewardTransition();
            PlayResultEffect().Forget();
            base.OnOpened(args);
        }

        protected override void OnEnablePage(object args)
        {
            AppManager.Instance.UIManager.Header.Hide();
            AppManager.Instance.UIManager.Footer.Hide();
            base.OnEnablePage(args);
        }
        
        protected override async UniTask<bool> OnPreClose(CancellationToken token)
        {
            AppManager.Instance.UIManager.Header.Show();
            AppManager.Instance.UIManager.Footer.Show();
            foreach (var item in prizeLineList)
            {
                Destroy(item);
            }
            return await base.OnPreClose(token);
        }

        protected override async UniTask<bool> OnPreLeave(CancellationToken token) {
            GC.Collect();
            await Resources.UnloadUnusedAssets().ToUniTask();
            GC.Collect();

            return true;
        }
        #endregion
        
        #region PrivateMethods

        protected virtual async UniTask Init()
        {
            huntEnemyMasterObject = MasterManager.Instance.huntEnemyMaster.FindData(data.mHuntEnemyId);
            huntMasterObject = MasterManager.Instance.huntMaster.FindData(huntEnemyMasterObject.mHuntId);
            huntDifficultyMasterObject = MasterManager.Instance.huntDifficultyMaster.FindData(huntEnemyMasterObject.difficulty);
            huntTimetableMasterObject = MasterManager.Instance.huntTimetableMaster.FindData(data.mHuntTimetableId);

            prizeList = new List<PrizeJsonViewData>();
            foreach (var huntPrize in data.prizeSetList)
            {
                foreach (var prize in huntPrize?.prizeJsonList)
                {
                    prizeList.Add(new PrizeJsonViewData(prize));
                }
            }

            skipTrigger.SetActive(true);

            // 報酬ブースト
            effectBoostValue = await RivalryManager.GetRewardBoostValueAsync(huntTimetableMasterObject?.id ?? 0);
            CreatePrize();
            
            // チュートリアルの場合はここで終了
             if (isTutorial) return;
             
            var isActivePassEffect = data.prizeCorrectionList?.FirstOrDefault(data => data.routeType == (long)HuntPrizeRouteType.Pass)?.rate > 0;
            var isActiveBoostEffect = data.prizeCorrectionList?.FirstOrDefault(data => data.routeType == (long)HuntPrizeRouteType.Chara)?.rate > 0;
            if (effectBadgeAnimator != null) effectBadgeAnimator.enabled = isActivePassEffect && isActiveBoostEffect;
            if (boostEffectBadge != null) boostEffectBadge.SetActive(isActiveBoostEffect);
            if (passEffectBadge != null) passEffectBadge.SetActive(isActivePassEffect);
            

            // リトライボタン
            var isEvent = RivalryManager.GetMatchType(huntTimetableMasterObject, huntMasterObject) == RivalryMatchType.Event;
            var huntResultList = await RivalryManager.Instance.GetHuntResultList();
            var usedLimit = huntResultList.FirstOrDefault(data => data.mHuntTimetableId == huntTimetableMasterObject.id)?.dailyPlayCount ?? 0;
            var remainingLimit = huntTimetableMasterObject?.dailyPlayCount - usedLimit;
            var canRetryLimit = (huntTimetableMasterObject?.dailyPlayCount > 0 && remainingLimit > 0) || huntTimetableMasterObject?.dailyPlayCount == 0;
            if (retryButton != null)
            {
                retryButton.gameObject.SetActive(isEvent && !huntMasterObject.isClosedOnceWin);
                retryButton.interactable = canRetryLimit;
            }

            // 「次のステージへ」ボタン
            if (nextStageButton != null)
            {
                nextEnemyMasterObject = MasterManager.Instance.huntEnemyMaster.FindNextStageHunt(huntEnemyMasterObject);
                //１日の制限回数があるかどうか
                bool isLimitDailyLeft = huntTimetableMasterObject?.dailyPlayCount > 0;
                //イベントであるかつ次のステージが存在するかつ選択報酬がない場合に表示
                nextStageButton.gameObject.SetActive(isEvent && nextEnemyMasterObject != null);
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

        /// <summary>
        /// 報酬作成
        /// </summary>
        /// <param name="pageView"></param>
        private void CreatePrize(){
            var mHuntSpecificChara = RivalryManager.GetRewardBoost(huntTimetableMasterObject?.id ?? 0);
            List<object> mPointIdList = mHuntSpecificChara != null ? (List<object>)MiniJSON.Json.Deserialize(mHuntSpecificChara.mPointIdList) : new List<object>();
            // 報酬表示の行数
            int contentLineCount = (prizeList.Count /  prizeCountPerLine) + 1;
            // 行を複製
            prizeLineList = new GameObject[contentLineCount];
            for(int i = 0; i < prizeLineList.Length; i++)
            {
                prizeLineList[i] = GameObject.Instantiate<GameObject>(prizeLine, content.transform);
                prizeLineList[i].gameObject.SetActive(true);
            }
            
            prizeIconList = new RivalryRewardIcon[prizeList.Count];

            var isFirstNewIds = new List<long>();

            // 報酬生成
            for(int i = 0; i < prizeList.Count; i++)
            {
                // どの行に配置するか
                int lineIndex = i / prizeCountPerLine;
                // アイコン複製
                prizeIconList[i] = GameObject.Instantiate<RivalryRewardIcon>(prizeIcon, prizeLineList[lineIndex].transform);
                // 初期表示
                var isFirstNew = false;
                if( prizeList[i].ItemIconType == ItemIconType.Character && prizeList[i].IsNew && !isFirstNewIds.Contains( prizeList[i].Id ) ) {
                    isFirstNew = true;
                    isFirstNewIds.Add( prizeList[i].Id );
                }
                // パス効果表示
                // 加算補正倍率と補正が掛かる前の値は補正がかかった場合のみ存在するため２つの値が0より大きい場合はパスの効果が発動しているとする
                var isActivePassEffectReward = prizeList[i].CorrectRate > 0 && prizeList[i].ValueOriginal > 0;
                // 選手設定の報酬ブースト
                var isActiveBoostEffectReward = mHuntSpecificChara != null && effectBoostValue > 0 && mPointIdList.Contains((long) prizeList[i].Id);
                prizeIconList[i].SetView(prizeList[i], isFirstNew, isActivePassEffectReward || isActiveBoostEffectReward);
                prizeIconList[i].gameObject.SetActive(true);
            }
        }

        
        /// <summary>
        /// 結果演出
        /// </summary>
        private async UniTask PlayResultEffect( ){
            var token = this.GetCancellationTokenOnDestroy();
            
            //Fadeアニメーションを待つために待機
            await UniTask.Delay(startEffectInterval, cancellationToken:token );
            //カード演出
            await PlayOpenCardsEffect();

            skipTrigger.SetActive(false);
            await TryShowAutoSellConfirmModal(data.autoSell);
            OnFinishedInit();
        }

        private async UniTask TryShowAutoSellConfirmModal(NativeApiAutoSell autoSell)
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

        /// <summary>
        /// カードオープン演出
        /// </summary>
        /// <returns></returns>
        private async UniTask PlayOpenCardsEffect( ){
            foreach(var icon in prizeIconList){
                await icon.Open( getEffect );
            }
        }
        #endregion

        #region ProtectedMethods
        protected virtual void OnFinishedInit()
        {
            
        }
        #endregion
        
        #region EventListeners
        /// <summary>
        /// スキップトリガーがタップされた
        /// </summary>
        public void OnClickSkipTrigger(){
            
            foreach(var icon in prizeIconList){
                if( icon.IsOpened ){
                    continue;
                }
                if( icon.IsPlayGetEffect ){
                    break;
                }
                icon.Skip();
            }
        }
        
        public async void OnClickBack()
        {
            var matchType = huntTimetableMasterObject.type == 1 && huntMasterObject.lotteryType == 1 ? RivalryMatchType.Event : RivalryMatchType.Regular;
            var nextParams = new RivalryPage.Data();
            var huntResultList = await RivalryManager.Instance.GetHuntResultList();
            var usedLimit = huntResultList.FirstOrDefault(data => data.mHuntTimetableId == huntTimetableMasterObject.id)?.dailyPlayCount ?? 0;
            if (huntTimetableMasterObject.dailyPlayCount > 0 && usedLimit >= huntTimetableMasterObject.dailyPlayCount)
            {
                // 挑戦できない場合はTOPに戻る
                // nextParams空のまま遷移する
            }
            else if (matchType == RivalryMatchType.Event)
            {
                var HuntEnemyMasterObjectList = MasterManager.Instance.huntEnemyMaster.values.Where(aData => aData.mHuntId == huntMasterObject.id).ToList();
                nextParams.pageType = RivalryPageType.RivalryEvent;
                nextParams.args = new RivalryEventPage.PageParams{huntMasterObject = huntMasterObject, huntTimetableMasterObject = huntTimetableMasterObject, HuntEnemyMasterObjectList = HuntEnemyMasterObjectList};
            }
            else
            {
                nextParams.pageType = RivalryPageType.RivalryTeamSelect;
                nextParams.args = new RivalryTeamSelectPage.PageParams(huntDifficultyMasterObject, huntMasterObject, huntTimetableMasterObject);
            }
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Rivalry, true, nextParams);
        }

        public void OnClickNextStage()
        {
            // ライバルリーのみ
            if (huntTimetableMasterObject.type != 1) return;
            //スタミナがあるかどうか
            bool isSuccess = KickOff(StaminaUtility.StaminaType.RivalryBattle, (long)StaminaUtility.StaminaType.RivalryBattle, huntMasterObject.useStaminaValue);
            if (!isSuccess) return;
            if (SupportEquipmentManager.ShowOverLimitModal())
            {
                return;
            }
            
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

