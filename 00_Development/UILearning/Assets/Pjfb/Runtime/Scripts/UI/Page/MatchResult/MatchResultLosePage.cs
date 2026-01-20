using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.ClubMatch;
using Pjfb.Colosseum;
using Pjfb.Common;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.Rivalry;
using Pjfb.Story;
using Pjfb.UserData;
using Pjfb.ClubDeck;
using Pjfb.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.MatchResult
{
    public class MatchResultLosePage : Page
    {
        #region Params
        public class Data
        {
            public PageType OpenFrom;
            public int BattleResult;
            public HuntFinishAPIResponse HuntFinishResponse;
            public ColosseumAttackAPIResponse ColosseumAttackAPIResponse;
            public long useDeckId;

            public Data(PageType _OpenFrom, int _BattleResult, HuntFinishAPIResponse _HuntFinishResponse)
            {
                OpenFrom = _OpenFrom;
                BattleResult = _BattleResult;
                HuntFinishResponse = _HuntFinishResponse;
                ColosseumAttackAPIResponse = null;
            }
            
            public Data(PageType _OpenFrom, int _BattleResult, ColosseumAttackAPIResponse _colosseumAttackAPIResponse, long _useDeckId)
            {
                OpenFrom = _OpenFrom;
                BattleResult = _BattleResult;
                HuntFinishResponse = null;
                ColosseumAttackAPIResponse = _colosseumAttackAPIResponse;
                useDeckId = _useDeckId;
            }
        }
        #endregion

        #region SerializeFields
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject scoreRoot;
        [SerializeField] private GameObject rewardPointRoot;
        [SerializeField] private PossessionItemUi rewardPointPossessionItemUi;
        [SerializeField] private TMP_Text rewardPointText;
        [SerializeField] private Image headlineImage;
        [SerializeField] private Sprite loseSprite;
        [SerializeField] private Sprite drawSprite;
        [SerializeField] private UIBadgeBalloon passEffectBadge;
        [SerializeField] private Button retryButton;
        [SerializeField] private UIBadgeBalloonCrossFade balloonCrossFade;
        [SerializeField] private TextMeshProUGUI staminaCostText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private ConditionView conditionView;
        #endregion

        private Data data;
        private HuntEnemyMasterObject huntEnemyMasterObject;
        private HuntDifficultyMasterObject huntDifficultyMasterObject;
        private HuntMasterObject huntMasterObject;
        private HuntTimetableMasterObject huntTimetableMasterObject;

        #region OverrideMethods
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            data = (Data) args;
            Init();
            await base.OnPreOpen(args, token);
        }

        protected override void OnOpened(object args)
        {
            // ライバルリーの場合
            if (data.HuntFinishResponse != null)
            {
                // シークレットセール表示
                ShopManager.TryShowSaleIntroduction(SaleIntroductionDisplayType.RivalryLose);
            }
            
            base.OnOpened(args);
        }

        protected override void OnEnablePage(object args)
        {
            AppManager.Instance.UIManager.Header.Hide();
            AppManager.Instance.UIManager.Footer.Hide();
            base.OnEnablePage(args);
        }

        
        protected override UniTask OnMessage(object value)
        {
            if(value is PageManager.MessageType type) 
            {
                switch(type) 
                {
                    case PageManager.MessageType.EndFade:
                        animator.SetTrigger("Open");
                        break;
                }
            }
            return base.OnMessage(value);
        }
        #endregion
        
        #region PrivateMethods
        private void Init()
        {
            if (data.HuntFinishResponse != null)
            {
                huntEnemyMasterObject = MasterManager.Instance.huntEnemyMaster.FindData(data.HuntFinishResponse.mHuntEnemyId);
                huntMasterObject = MasterManager.Instance.huntMaster.FindData(huntEnemyMasterObject.mHuntId);
                huntDifficultyMasterObject = MasterManager.Instance.huntDifficultyMaster.FindData(huntEnemyMasterObject.difficulty);
                huntTimetableMasterObject = MasterManager.Instance.huntTimetableMaster.FindOpenRivalryDataWithHuntId(huntMasterObject.id);
            }
            InitHeadlineImage();
            InitRewardPoint();
            InitPassEffect();
            InitRetryButton();
        }

        private void InitHeadlineImage()
        {
            headlineImage.sprite = data.BattleResult == 3 ? drawSprite : loseSprite;
        }
        
        private void InitRewardPoint()
        {
            if (data.HuntFinishResponse != null)
            {
                InitHuntRewardPoint();
            }
            else if (data.ColosseumAttackAPIResponse != null)
            {
                if (data.OpenFrom == PageType.Colosseum)
                {
                    InitColosseumRewardPoint();
                }
                else if (data.OpenFrom == PageType.ClubMatch)
                {
                    InitClubMatchRewardPoint();   
                }
            }
        }

        private void InitHuntRewardPoint()
        {
            var mHuntEnemy = MasterManager.Instance.huntEnemyMaster.FindData(data.HuntFinishResponse.mHuntEnemyId);
            var mHuntTimetable = MasterManager.Instance.huntTimetableMaster.FindOpenRivalryDataWithHuntId(mHuntEnemy.mHuntId);
            var isEventMatch = mHuntTimetable != null && mHuntTimetable.type == 1  && mHuntTimetable.mPointId >= 1;
            scoreRoot.SetActive(false);
            if (!isEventMatch)
            {
                rewardPointRoot.SetActive(false);
            }
            else
            {
                long rewardValue = 0;
                var isActivePassEffect = false;
                foreach (var prize in data.HuntFinishResponse.prizeSetList)
                {
                    foreach (var reward in prize.prizeJsonList)
                    {
                        if (reward.args.mPointId == mHuntTimetable.mPointId)
                        {
                            rewardValue += reward.args.value;
                            // 加算補正倍率と補正が掛かる前の値は補正がかかった場合のみ存在するため２つの値が0より大きい場合はパスの効果が発動しているとする
                            if (!isActivePassEffect && reward.args.correctRate > 0 && reward.args.valueOriginal > 0)
                            {
                                isActivePassEffect = true;
                            }
                        }
                    }
                }
                rewardPointRoot.SetActive(true);
                rewardPointPossessionItemUi.SetCount(mHuntTimetable.mPointId, rewardValue);
                if (isActivePassEffect)
                {
                    rewardPointPossessionItemUi.SetColor(ColorValueAssetLoader.Instance["highlight.orange"]);
                }
            }
            rewardPointText.text = StringValueAssetLoader.Instance["rivalry.result.point"];
        }
        
        private void InitColosseumRewardPoint()
        {
            var targetPointId = ColosseumManager.RankMatchRewardPointId;;
            long rewardValue = 0;
            var isActivePassEffect = false;
            foreach (var reward in data.ColosseumAttackAPIResponse.prizeJsonList)
            {
                if (reward.args.mPointId == targetPointId)
                {
                    rewardValue += reward.args.value;
                    // 加算補正倍率と補正が掛かる前の値は補正がかかった場合のみ存在するため２つの値が0より大きい場合はパスの効果が発動しているとする
                    if (!isActivePassEffect && reward.args.correctRate > 0 && reward.args.valueOriginal > 0)
                    {
                        isActivePassEffect = true;
                    }
                }
            }
            rewardPointRoot.SetActive(true);
            rewardPointPossessionItemUi.SetCount(targetPointId, rewardValue);
            if (isActivePassEffect)
            {
                rewardPointPossessionItemUi.SetColor(ColorValueAssetLoader.Instance["highlight.orange"]);
            }
            rewardPointText.text = StringValueAssetLoader.Instance["pvp.rankmatch.reward"];
        }
        
        private async void InitClubMatchRewardPoint()
        {
            scoreRoot.SetActive(true);
            rewardPointRoot.SetActive(false);
            var playerResult = data.ColosseumAttackAPIResponse.battleChangeResult.player;
            var getScore = playerResult.after.score - playerResult.before.score;
            scoreText.text = getScore.ToString();
            conditionView.gameObject.SetActive(true);
            
            var clubBattleDeck = await DeckUtility.GetClubBattleDeck();
            var useDeck = clubBattleDeck.DeckDataList.FirstOrDefault(deck => deck.PartyNumber == data.useDeckId);
            useDeck?.UpdateFatigueValue();
            conditionView.SetCondition(useDeck?.FixedClubConditionData ?? ClubMatchUtility.GetConditionData(ClubDeckCondition.Best));
        }
        
        private void InitPassEffect()
        {
            var isActivePassEffect = false;
            // ライバルリーの場合
            if (data.HuntFinishResponse != null)
            {
                // HuntPrizeCorrection.routeTypeがHuntPrizeRouteType.Passであればパス効果発動中
                isActivePassEffect = data.HuntFinishResponse.prizeCorrectionList?.FirstOrDefault(data => data.routeType == (long)HuntPrizeRouteType.Pass)?.rate > 0;
            }
            // PvPの場合
            else if (data.ColosseumAttackAPIResponse != null)
            {
                if (data.OpenFrom == PageType.Colosseum)
                {
                    isActivePassEffect = data.ColosseumAttackAPIResponse.prizeJsonList.Any(prize => prize.args.correctRate > 0 && prize.args.valueOriginal > 0);   
                }
            }
            passEffectBadge.SetActive(isActivePassEffect);
        }

        private void InitRetryButton()
        {
            if (data.HuntFinishResponse == null) 
            {
                if (retryButton != null) retryButton.gameObject.SetActive(false);
                return;
            }
            var isEvent = RivalryManager.GetMatchType(huntTimetableMasterObject, huntMasterObject) == RivalryMatchType.Event;
            var usedLimit = RivalryManager.Instance.huntResultList.FirstOrDefault(data => data.mHuntTimetableId == huntTimetableMasterObject.id)?.dailyPlayCount ?? 0;
            var remainingLimit = huntTimetableMasterObject.dailyPlayCount - usedLimit;
            var canRetryLimit = (huntTimetableMasterObject.dailyPlayCount > 0 && remainingLimit > 0) || huntTimetableMasterObject.dailyPlayCount == 0;
            if (retryButton != null) 
            {
                retryButton.gameObject.SetActive(isEvent);
                retryButton.interactable = canRetryLimit;
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
                var limitString = string.Format(StringValueAssetLoader.Instance[huntTimetableMasterObject.playCountType == (long)HuntPlayCountType.Win ? "rivalry.match_limit.win" : "rivalry.match_limit.challenge"], remainingLimit);
                balloonCrossFade.SetView(staminaFreeCondition, limitCondition, staminaFreeString, limitString);
                staminaCostText.text = string.Format(StringValueAssetLoader.Instance["common.stamina_usage"], huntMasterObject.useStaminaValue);
            
            }
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
                    var matchType = huntTimetableMasterObject.type == 1 && huntMasterObject.lotteryType == 1 ? RivalryMatchType.Event : RivalryMatchType.Regular;
                    var nextParams = new RivalryPage.Data();
                    var usedLimit = RivalryManager.Instance.huntResultList.FirstOrDefault(data => data.mHuntTimetableId == huntTimetableMasterObject.id)?.dailyPlayCount ?? 0;
                    bool isLimit = huntTimetableMasterObject.dailyPlayCount > 0 && usedLimit >= huntTimetableMasterObject.dailyPlayCount;
                    
                    if (matchType == RivalryMatchType.Event)
                    {
                        var HuntEnemyMasterObjectList = MasterManager.Instance.huntEnemyMaster.values.Where(aData => aData.mHuntId == huntMasterObject.id).ToList();
                        nextParams.pageType = RivalryPageType.RivalryEvent;
                        nextParams.args = new RivalryEventPage.PageParams{huntMasterObject = huntMasterObject, huntTimetableMasterObject = huntTimetableMasterObject, HuntEnemyMasterObjectList = HuntEnemyMasterObjectList, 
                            autoTransitToEventTop = isLimit };
                    }
                    else
                    {
                        if(isLimit == false)
                        {
                            nextParams.pageType = RivalryPageType.RivalryTeamSelect;
                            nextParams.args = new RivalryTeamSelectPage.PageParams(huntDifficultyMasterObject, huntMasterObject, huntTimetableMasterObject);   
                        }
                    }
                    await AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.Rivalry, true, nextParams);
                    break;
                case PageType.Colosseum:
                    ColosseumPage.OpenPage(true);
                    break;
                case PageType.ClubMatch:
                    var sColosseumEventId = data.ColosseumAttackAPIResponse.userSeasonStatus.sColosseumEventId;
                    var param = new ClubMatchTopPage.Data(UserDataManager.Instance.GetColosseumSeasonData(sColosseumEventId)){callerPage = PageType.MatchResult};
                    ClubMatchPage.OpenPage(true,param);
                    break;
                default: // failsafe
                    await AppManager.Instance.UIManager.PageManager.OpenPageAsync(PageType.Home, true, null);
                    break;
            }
        }

        public void OnClickRetry()
        {
            // 現状ライバルリーのみ
            if (huntTimetableMasterObject.type != 1) return;

            var isSuccess = KickOff(StaminaUtility.StaminaType.RivalryBattle, (long)StaminaUtility.StaminaType.RivalryBattle, huntMasterObject.useStaminaValue);
            if (!isSuccess) return;
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

        public void OnClickGachaButton()
        {
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Gacha, true, null);
        }

        public void OnClickTeamButton()
        {
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Character, true, null);
        }

        public void OnClickTrainingButton()
        {
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.TrainingPreparation, true, null);
        }
        #endregion
    }
}
