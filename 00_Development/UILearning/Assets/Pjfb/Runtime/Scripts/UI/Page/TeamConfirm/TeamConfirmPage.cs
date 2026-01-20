using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Pjfb.Character;
using Pjfb.Deck;
using Pjfb.Home;
using Pjfb.Master;
using Pjfb.Menu;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Rivalry;
using Pjfb.Storage;
using Pjfb.Training;
using Pjfb.UI;
using Pjfb.UserData;
using Pjfb.Voice;
using Pjfb.Colosseum;
using Pjfb.Combination;
using Pjfb.InGame;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Pjfb.Voice.VoiceResourceSettings;
using Pjfb.ClubDeck;
using Pjfb.ClubMatch;
using Pjfb.Extensions;
using Pjfb.SystemUnlock;

namespace Pjfb
{
    public class TeamConfirmPage : Page
    {
        private const int TOP_RANKING_BORDER = 3;

        [Serializable]
        enum KickOffCostType
        {
            Stamina,
            Point,
            Grayout,
        }

        [Serializable]
        enum MatchType
        {
            Hunt,
            PvP,
            ClubMatch,
            Practice
        }

        
        [Serializable]
        private class CostUiObject
        {
            public GameObject gameObject;
            public KickOffCostType[] costTypes;
        }
        
        private readonly PageType[] NoProfilePageList = {PageType.Title};

        #region PageParams
        public class PageParams
        {
            public PageType openFrom;
            public object backArgs;
            // ライバルリー、ストーリ
            public HuntMasterObject huntMasterObject;
            public HuntTimetableMasterObject huntTimetableMasterObject;
            public HuntEnemyMasterObject huntEnemyMasterObject;
            // 練習試合
            public long targetUMasterId;
            // 練習試合
            public UserProfileUserStatus enemyStatus = null;
            // PvP
            public ColosseumDeck colosseumDeck = null;
            public ColosseumRankingUser colosseumUser;
            public ColosseumSeasonData colosseumSeasonData;

            // ライバルリー、ストーリ
            public PageParams(PageType _openFrom, object _backArgs, HuntMasterObject _huntMasterObject, HuntTimetableMasterObject _huntTimetableMasterObject, HuntEnemyMasterObject _huntEnemyMasterObject)
            {
                openFrom = _openFrom;
                backArgs = _backArgs;
                huntMasterObject = _huntMasterObject;
                huntTimetableMasterObject = _huntTimetableMasterObject;
                huntEnemyMasterObject = _huntEnemyMasterObject;
                targetUMasterId = -1;
                enemyStatus = null;
                colosseumSeasonData = null;
                colosseumDeck = null;
                colosseumUser = null;
            }

            // Pvp
            public PageParams(PageType _openFrom, object _backArgs, ColosseumDeck _colosseumDeck, ColosseumRankingUser _colosseumUser, ColosseumSeasonData _colosseumSeasonData)
            {
                openFrom = _openFrom;
                backArgs = _backArgs;
                huntMasterObject = null;
                huntTimetableMasterObject = null;
                huntEnemyMasterObject = null;
                targetUMasterId = -1;
                enemyStatus = null;
                colosseumSeasonData = _colosseumSeasonData;
                colosseumDeck = _colosseumDeck;
                colosseumUser = _colosseumUser;
            }

            // 練習試合
            public PageParams(PageType _openFrom, object _backArgs, long _targetUMasterId, UserProfileUserStatus _enemyStatus)
            {
                openFrom = _openFrom;
                backArgs = _backArgs;
                huntMasterObject = null;
                huntTimetableMasterObject = null;
                huntEnemyMasterObject = null;
                targetUMasterId = _targetUMasterId;
                enemyStatus = _enemyStatus;
                colosseumSeasonData = null;
                colosseumDeck = null;
                colosseumUser = null;
            }
        }
        #endregion

        #region SerializeFields
        [SerializeField] private GameObject[] playerObjects;
        [SerializeField] private GameObject[] enemyObjects;
        [SerializeField] private TextMeshProUGUI scenarioNameText;
        [SerializeField] private TextMeshProUGUI switchText;
        [SerializeField] private DeckPanelView enemyDeckPanelView;
        [SerializeField] private GameObject playerDeckPanel;
        [SerializeField] private ScrollBanner playerDeckScrollGrid;
        [SerializeField] private CharacterCardImage characterCardImage;
        [SerializeField] private TextMeshProUGUI rewardTitleText;
        [SerializeField] private GameObject rewardTitleWarning;
        [SerializeField] private ScrollGrid rewardList;
        [SerializeField] private Button startButton;
        // 開始ボタンがグレーアウト時に押されるボタン
        [SerializeField]
        private UIButton startGrayOutButton;
        [SerializeField] private UIBadgeBalloonCrossFade balloonCrossFade;
        [SerializeField] private StaminaView staminaUi;
        [SerializeField] private GameObject staminaCostRoot;
        [SerializeField] private TextMeshProUGUI staminaCostText;
        [SerializeField] private UIToggle skipMatchToggle;
        [SerializeField] private TrainingCharacterStatusResult statusResult;
        [SerializeField] private GameObject itemRewardRoot;
        [SerializeField] private CostUiObject[] costUiList;
        [SerializeField] private Animator startButtonAnimator;
        [SerializeField] private Button rewardBoostButton;
        [SerializeField] private GameObject rewardBoostBadge;
        [SerializeField] private TMP_Text rewardBoostText;

        // 試合開始条件ボタン
        [SerializeField]
        private GameObject battleStartConditionButton = null;
        // 弱体化条件ボタン
        [SerializeField]
        private GameObject weakConditionButton = null;
        
        
        [Header("Combination Match")]
        [SerializeField] private GameObject combinationMatchLockObject;
        [SerializeField] private GameObject activatingCombinationMatchCountRoot;
        [SerializeField] private TextMeshProUGUI activatingCombinationMatchCountText;
        
        [Header("Club Match (GvG)")]
        [SerializeField] private TextMeshProUGUI enemyClubNameText;
        [SerializeField] private Sprite[] rankingImageList;
        [SerializeField] private GameObject enemyRankingRoot;
        [SerializeField] private TextMeshProUGUI enemyRankingText;
        [SerializeField] private Image enemyRankingImage;
        [SerializeField] private GameObject enemyScoreObject;
        [SerializeField] private TextMeshProUGUI enemyScoreText;
        [SerializeField] private GameObject fatigueViewRoot;
        [SerializeField] private ConditionView conditionView;
        [SerializeField] private GameObject consecutivePenaltyObject;
        [SerializeField] private TextMeshProUGUI consecutivePenaltyText;

        
        #endregion

        private PageParams _pageParams;
        private DeckListData _deckListData;
        private HuntEnemyMasterObject mHuntEnemy => _pageParams.huntEnemyMasterObject;
        private HuntMasterObject mHunt => _pageParams.huntMasterObject;
        private HuntTimetableMasterObject mHuntTimetable => _pageParams.huntTimetableMasterObject;
        private readonly Color EnemyColor = new(202f/255f, 55f/255f, 94f/255f);
        private readonly Color PlayerColor = new (38f/255f, 64f/255f, 218f/255f);
        private bool isShowPlayerDeck;
        private ColosseumDeckChara[] charaList;
        private BattleV2Chara[] charaVariableList;
        private CostPointEscalationMasterObject mCostPointEscalation;

        // 試合開始条件を満たしているか
        private bool isMatchBattleStartCondition = true;
        
        private MatchType CurrentMatchType
        {
            get 
            {
                if (_pageParams.colosseumDeck != null && _pageParams.openFrom == PageType.ClubMatch) return MatchType.ClubMatch;
                else if (_pageParams.colosseumDeck != null && _pageParams.openFrom == PageType.Colosseum) return MatchType.PvP;
                else if (_pageParams.enemyStatus != null) return MatchType.Practice;
                else if (_pageParams.huntMasterObject != null) return MatchType.Hunt;
                return MatchType.Hunt;
            }
        }

        #region OverrideMethods
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _pageParams = (PageParams) args;
            Init();
            return base.OnPreOpen(args, token);
        }

        private void Awake()
        {
            playerDeckScrollGrid.onChangedPage += OnChangePage;
        }

        protected override async UniTask<bool> OnPreLeave(CancellationToken token)
        {
            var selectedBannerParameters = (DeckPanelScrollGridItem.Parameters) playerDeckScrollGrid.GetNullableBannerData();
            var selectedPartyNumber = selectedBannerParameters?.viewParams.deckId ?? 0;
            var deckReady  = selectedBannerParameters?.viewParams.iconParams
                .All(aData => aData.nullableCharacterData != null && aData.nullableCharacterData.MCharaId != 0) ?? false;
            if (deckReady) await TryCallDeckSelectAPI(selectedPartyNumber);
            // 試合スキップ設定を保存
            LocalSaveManager.saveData.skipMatchData = skipMatchToggle.isOn;
            // セーブデータ保存
            LocalSaveManager.Instance.SaveData();
            // PrizeJsonArgsの値リセット
            ResetPrizeJson();
            return await base.OnPreLeave(token);
        }
        #endregion
        
        #region PrivateMethods
        private async void Init()
        {
            
            if (CurrentMatchType == MatchType.PvP || CurrentMatchType == MatchType.ClubMatch)
            {
                charaList = _pageParams.colosseumDeck.charaList;
                charaVariableList = _pageParams.colosseumDeck.charaVariableList;
            }
            else if (CurrentMatchType == MatchType.Practice)
            {
                await GetPvpMimicGetPreparationDetailAPI(_pageParams.targetUMasterId);
            }
            
            InitDisplay();
            SetPlayerDeckDisplay();
            SwitchDisplay(isShowPlayerDeck);
            
            if (staminaUi != null)
            {
                SetStaminaDisplay();
            }
            if (statusResult != null)
            {
                SetStatusResult(isShowPlayerDeck);
            }
        }
        
        private void InitDisplay()
        {
            isShowPlayerDeck = true;
            enemyDeckPanelView.Init();
            
            if (scenarioNameText != null)
            {
                scenarioNameText.text = CurrentMatchType == MatchType.PvP || CurrentMatchType == MatchType.ClubMatch ? _pageParams.colosseumUser.name : mHuntEnemy.name;
            }

            skipMatchToggle.isOn = LocalSaveManager.saveData.skipMatchData;
            SetEnemyDeckDisplay();
        }

        private void SetClubMatchDisplay()
        {
            // クラブ名
            if (enemyClubNameText != null)
            {
                enemyClubNameText.gameObject.SetActive(CurrentMatchType == MatchType.ClubMatch);
                enemyClubNameText.text = CurrentMatchType == MatchType.ClubMatch ? _pageParams.colosseumUser.groupName : string.Empty;
            }
            // ランキング
            if (enemyRankingImage != null && enemyRankingText != null && CurrentMatchType == MatchType.ClubMatch)
            {
                var ranking = _pageParams.colosseumUser.ranking;
                var isTopUser = ranking <= TOP_RANKING_BORDER;
                enemyRankingImage.gameObject.SetActive(isTopUser);
                enemyRankingText.gameObject.SetActive(!isTopUser);
                if (isTopUser)
                {
                    enemyRankingImage.sprite = rankingImageList[ranking - 1];
                }
                else
                {
                    enemyRankingText.text = ranking.ToString();   
                }
            }
            else if (enemyRankingImage != null && enemyRankingText != null)
            {
                enemyRankingRoot.gameObject.SetActive(false);
            }
            // 疲労度
            if (fatigueViewRoot != null && conditionView != null)
            {
                fatigueViewRoot.SetActive(CurrentMatchType == MatchType.ClubMatch && isShowPlayerDeck);
                if (CurrentMatchType == MatchType.ClubMatch)
                {
                    // 疲労度を更新
                    var selectingIndex = GetDeckSelectingIndex();
                    _deckListData.DeckDataList[selectingIndex].UpdateFatigueValue();
                    conditionView.SetCondition(_deckListData.DeckDataList[selectingIndex].FixedClubConditionData);
                }
            }
            // 連勝ペナルティ
            if (consecutivePenaltyObject != null && consecutivePenaltyText != null)
            {
                // 一旦固定という方針で
                var penaltyValue = (int)(ColosseumManager.GetPenaltyCoefficient(_pageParams.colosseumUser?.defenseCount ?? 0) * 100);
                consecutivePenaltyObject.SetActive(CurrentMatchType == MatchType.ClubMatch && penaltyValue > 0 && !isShowPlayerDeck);
                consecutivePenaltyText.text = string.Format(StringValueAssetLoader.Instance["clubmatch.penalty"], penaltyValue);
            }
            // 予定スコア
            if (enemyScoreObject != null && enemyScoreText != null)
            {
                enemyScoreObject.SetActive(!isShowPlayerDeck && CurrentMatchType == MatchType.ClubMatch);
                if (CurrentMatchType == MatchType.ClubMatch)
                {
                    enemyScoreText.text = ColosseumManager.GetExpectedScoreData(_pageParams.colosseumUser, _pageParams.colosseumSeasonData.ScoreBattleTurn).totalScore.ToString();
                }
            }
        }

        private void SetRewardDisplay()
        {
            if (CurrentMatchType != MatchType.Hunt)
            {
                itemRewardRoot.SetActive(false);
                return;
            }
            
            itemRewardRoot.SetActive(true);
            var mHuntEnemyPrizeList = MasterManager.Instance.huntEnemyPrizeMaster.FindGeneralRewardList(mHuntEnemy.mHuntId, mHuntEnemy.difficulty, mHuntEnemy.rarity);
            var prizeArgs = new List<PrizeJsonGridItem.Data>();
            // 選手ブースト
            var huntSpecificCharaMasterObject = RivalryManager.GetRewardBoost(_pageParams.huntTimetableMasterObject.id);
            var mPointIdList = huntSpecificCharaMasterObject != null ? (List<object>)MiniJSON.Json.Deserialize(huntSpecificCharaMasterObject.mPointIdList) : new List<object>();
            var effectBoostValue = RivalryManager.GetRewardBoostValue(_pageParams.huntTimetableMasterObject.id, (DeckPanelScrollGridItem.Parameters) playerDeckScrollGrid.GetNullableBannerData());
            // サブスク効果
            var activePassEffectList = UserDataManager.Instance.GetActivePointGetBonus();
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
                        totalBoostValue += effectBoostValue;
                    }
                    // サブスク効果
                    var isActivePassReward = false;
                    foreach (var activePass in activePassEffectList)
                    {
                        if (activePass.mPointId == prize.args.mPointId)
                        {
                            isActivePassReward = true;
                            totalBoostValue += (activePass.rate / 100);
                        }
                    }
                    // 効果適用
                    if (prize.args.valueOriginal > 0 && prize.args.value != prize.args.valueOriginal)
                    {
                        prize.args.value = prize.args.valueOriginal;
                    }
                    prize.args.value = prize.args.valueOriginal + (long)Math.Round((float)totalBoostValue * prize.args.valueOriginal / 100, 0, MidpointRounding.AwayFromZero);
                    prizeArgs.Add(new PrizeJsonGridItem.Data(prize, isActiveBoostEffectReward || isActivePassReward));
                }
            }
            rewardList.SetItems(prizeArgs);
            rewardTitleText.text = !mHunt.isClosedOnceWin ? string.Format(StringValueAssetLoader.Instance["rivalry.confirm.reward_title"]) : string.Format(StringValueAssetLoader.Instance["rivalry.confirm.reward_title_tower"]);
            rewardTitleWarning.SetActive(mHunt.hasChoicePrize);
        }
        
        private void ResetPrizeJson()
        {
            if (CurrentMatchType != MatchType.Hunt)
            {
                return;
            }
            
            var mHuntEnemyPrizeList = MasterManager.Instance.huntEnemyPrizeMaster.FindGeneralRewardList(mHuntEnemy.mHuntId, mHuntEnemy.difficulty, mHuntEnemy.rarity);
            foreach (var mHuntEnemyPrize in mHuntEnemyPrizeList)
            {
                foreach (var prize in mHuntEnemyPrize?.prizeJson)
                {
                    if (prize.args.valueOriginal > 0)
                    {
                        prize.args.value = prize.args.valueOriginal;
                    }
                }
            }
        }

        private void SetStaminaDisplay()
        {
            staminaCostRoot.SetActive(CurrentMatchType != MatchType.ClubMatch);
            if (CurrentMatchType == MatchType.ClubMatch)
            {
                staminaUi.gameObject.SetActive(false);
            }
            else if (CurrentMatchType == MatchType.PvP)
            {
                staminaUi.gameObject.SetActive(true);
                staminaUi.UpdateAsync(StaminaUtility.StaminaType.Colosseum, _pageParams.colosseumSeasonData.MColosseumEvent.mStaminaId).Forget();
                staminaUi.OnUpdateStamina = UpdateKickOffButtonView;
                UpdateKickOffButtonView();
            }
            else
            {
                staminaUi.gameObject.SetActive(true);
                staminaUi.UpdateAsync(StaminaUtility.StaminaType.RivalryBattle).Forget();
                staminaUi.OnUpdateStamina = null;
                mCostPointEscalation = null;
                SetStartButtonView(KickOffCostType.Stamina);
                staminaCostText.text = string.Format(StringValueAssetLoader.Instance["common.stamina_usage"], mHunt.useStaminaValue);
            }
        }

        private void SetStatusResult(bool isShowPlayerDeck)
        {
            var nullableSelectedPlayerDeckViewParam = (DeckPanelScrollGridItem.Parameters) playerDeckScrollGrid.GetNullableBannerData();
            var characterVariableDetailData = 
                isShowPlayerDeck && nullableSelectedPlayerDeckViewParam?.viewParams.iconParams.Count > 0 ? nullableSelectedPlayerDeckViewParam.viewParams.iconParams[0].nullableCharacterData:
                !isShowPlayerDeck && enemyDeckPanelView.viewParams.iconParams.Count > 0 ? enemyDeckPanelView.viewParams.iconParams[0].nullableCharacterData : null;
            if (characterVariableDetailData == null)
            {
                statusResult.SetEmpty();
            }
            else
            {
                statusResult.SetStatus(characterVariableDetailData.MCharaId, characterVariableDetailData.Status, characterVariableDetailData.AbilityList);
            }
        }

        private async void SetPlayerDeckDisplay()
        {
            var deckType = CurrentMatchType == MatchType.ClubMatch ? DeckType.Club : DeckType.Battle;
            characterCardImage.gameObject.SetActive(false);
            _deckListData = await DeckUtility.GetDeckList(deckType);
            var huntSpecificCharaMasterObject = RivalryManager.GetRewardBoost(_pageParams.huntTimetableMasterObject?.id ?? 0);
            var mCharaIdList = huntSpecificCharaMasterObject != null ? (List<object>)MiniJSON.Json.Deserialize(huntSpecificCharaMasterObject.mCharaIdList) : new List<object>();
            var deckDataList = _deckListData.DeckDataList.ToList();

            HuntDeckRegulationType regulationType = HuntDeckRegulationType.None;
            IEnumerable<HuntDeckRegulationConditionMasterObject> regulationList = null;
            // ライバルリー編成条件
            if (CurrentMatchType == MatchType.Hunt)
            {
                // 条件タイプ取得
                regulationType = RivalryManager.GetRegulationType(mHuntEnemy.mHuntDeckRegulationId);
                // 条件マスタリスト取得
                regulationList = MasterManager.Instance.huntDeckRegulationConditionMaster.values.Where(x => x.mHuntDeckRegulationId == mHuntEnemy.mHuntDeckRegulationId);
            }

            var bannerDataList = deckDataList.Select(aDeck =>
            {
                List<CharacterVariableDetailData> detailOrderList = aDeck.Deck.memberIdList
                    .Where(x => x.l[1] != DeckUtility.EmptyDeckSlotId).Select(x =>
                        new CharacterVariableDetailData(UserDataManager.Instance.charaVariable.Find(x.l[1]))).ToList(); 
                int detailOrderIndex = 0;
                if(deckType == DeckType.Club)   aDeck.UpdateFatigueValue();
                return new DeckPanelScrollGridItem.Parameters
                {
                    viewParams = new DeckPanelView.ViewParams
                    (
                        deckId: aDeck.Deck.partyNumber,
                        deckName: aDeck.Deck.name,
                        strategy: (BattleConst.DeckStrategy)aDeck.Deck.optionValue,
                        iconParams: aDeck.Deck.memberIdList
                            .Where(aMember => aMember.l?.Length >= 3) // データ不正の場合処理されない
                            .Select((aMember) =>
                            {
                                CharacterVariableDetailData detailData = aMember.l[1] > 0 ? new CharacterVariableDetailData(UserDataManager.Instance.charaVariable.Find(aMember.l[1])) : null;

                                return new DeckPanelCharaIconView.ViewParams
                                {
                                    type = aMember.l[0],
                                    nullableCharacterData = detailData,
                                    position = (RoleNumber)aMember.l[2],
                                    swipeableParams = new SwipeableParams<CharacterVariableDetailData>(detailOrderList, detailOrderIndex++),
                                    boostValue = huntSpecificCharaMasterObject != null && mCharaIdList.Any(chara => (long)chara == (detailData?.MCharaId ?? -1))
                                        ? huntSpecificCharaMasterObject.rate / 100
                                        : 0,
                                };
                            }).ToList(),
                        isPlayerDeck: true,
                        onClickDeckEditButton: OnClickDeckEditButton,
                        rankBgColor: PlayerColor,
                        onStrategyChanged: TrySaveDeck,
                        conditionData: (deckType == DeckType.Club) ? aDeck.FixedClubConditionData : null,
                        // ライバルリーで条件が設定されている時のみ行う
                        isMatchCondition: CurrentMatchType != MatchType.Hunt || regulationType == HuntDeckRegulationType.None || RivalryManager.CheckHuntCondition(detailOrderList, regulationList)
                    )
                };
            }).ToList();
            playerDeckScrollGrid.SetBannerDatas(bannerDataList);
            playerDeckScrollGrid.SetIndex(_deckListData.SelectingIndex);
        }

        private string GetDeckName()
        {
            if (CurrentMatchType == MatchType.PvP || CurrentMatchType == MatchType.ClubMatch)
            {
                return StringValueAssetLoader.Instance["pvp.user.name"];
            }
            else if (CurrentMatchType == MatchType.Practice)
            {
                return _pageParams.enemyStatus.deck.name;
            }
            else
            {
                return mHuntEnemy.name;
            }
        }
        
        private void SetEnemyDeckDisplay()
        {
            var enemyIconParams = new List<DeckPanelCharaIconView.ViewParams>();
            var deckName = GetDeckName();
            
            List<CharacterVariableDetailData> detailOrderList = new();
            int detailOrderIndex = 0;

            // 弱体化時の戦力
            long weakenTotalPower = 0;
            
            if (CurrentMatchType == MatchType.Practice || CurrentMatchType == MatchType.PvP || CurrentMatchType == MatchType.ClubMatch)
            {
                enemyIconParams = charaList
                    .Select(aMember =>
                    {
                        var detailData = new CharacterVariableDetailData(charaVariableList.FirstOrDefault(chara => chara.mCharaId == aMember.mCharaId));
                        detailOrderList.Add(detailData);
                        return new DeckPanelCharaIconView.ViewParams
                        {
                            type = 1,
                            position = (RoleNumber)aMember.roleNumber,
                            nullableCharacterData = new CharacterVariableDetailData
                            (
                                aMember.mCharaId,
                                StatusUtility.ToCharacterStatus(charaVariableList.FirstOrDefault(chara => chara.mCharaId == aMember.mCharaId)),
                                detailData.AbilityList,
                                detailData.CharacterLevel,
                                detailData.TrainingScenarioId,
                                detailData.OpenedCollections
                            ),
                            swipeableParams = new SwipeableParams<CharacterVariableDetailData>(detailOrderList, detailOrderIndex++)
                        };
                    }).ToList();
            }
            // Rivalry
            else
            {
                for (var i = 0; i < Mathf.Max(mHuntEnemy.mCharaNpcIdList.Length, mHuntEnemy.roleNumberList.Length); i++)
                {
                    var detailData = mHuntEnemy.mCharaNpcIdList.Length > i ? new CharacterVariableDetailData(MasterManager.Instance.charaNpcMaster.FindData(mHuntEnemy.mCharaNpcIdList[i])) : null;
                    
                    enemyIconParams.Add(new DeckPanelCharaIconView.ViewParams
                    {
                        nullableCharacterData = detailData,
                        position = (RoleNumber)(mHuntEnemy.roleNumberList.Length > i ? mHuntEnemy.roleNumberList[i] : 0),
                        type = 1,
                        swipeableParams = (detailData != null) ? new SwipeableParams<CharacterVariableDetailData>(detailOrderList, detailOrderIndex++) : null,
                    });
                    if(detailData != null)
                        detailOrderList.Add(detailData);
                }

                var regulationMaster = MasterManager.Instance.huntDeckRegulationMaster.FindData(mHuntEnemy.mHuntDeckRegulationId);
                // 弱体化時の戦力取得
                if (regulationMaster != null && regulationMaster.HuntConditionType == HuntDeckRegulationType.Weaken)
                {
                    weakenTotalPower = regulationMaster.condtionCompleteBonusValue;
                }
            }
         
            enemyDeckPanelView.SetDisplay(new DeckPanelView.ViewParams (
                isPlayerDeck: false,
                deckName: deckName,
                strategy: BattleConst.DeckStrategy.None,
                rankBgColor: EnemyColor,
                iconParams: enemyIconParams,
                deckId: 0,
                onClickDeckEditButton: null,
                onStrategyChanged: null,
                penaltyValue: CurrentMatchType == MatchType.ClubMatch ? (long)(ColosseumManager.GetPenaltyCoefficient(_pageParams.colosseumUser?.defenseCount ?? 0) * 100) : 0,
                weakenTotalCombatPower: weakenTotalPower
            ));
        }


        private void SwitchDisplay(bool isShowPlayerDeck)
        {
            this.isShowPlayerDeck = isShowPlayerDeck;
            foreach (var obj in playerObjects)
            {
                obj.SetActive(isShowPlayerDeck);
            }
            foreach (var obj in enemyObjects)
            {
                obj.SetActive(!isShowPlayerDeck);
            }
            enemyDeckPanelView.gameObject.SetActive(!isShowPlayerDeck);
            playerDeckPanel.SetActive(isShowPlayerDeck);
            if (statusResult != null)
            {
                SetStatusResult(isShowPlayerDeck);
            }
            switchText.text = isShowPlayerDeck ? StringValueAssetLoader.Instance["common.enemy_team"] : StringValueAssetLoader.Instance["common.self_team"];
        }

        private void UpdateDisplay()
        {
            var nullableSelectedPlayerDeckViewParam = (DeckPanelScrollGridItem.Parameters) playerDeckScrollGrid.GetNullableBannerData();
            startButton.interactable = GetStartButtonInteractable();
            
            // 戦力表示タイプ
            DeckPanelView.CombatPowerViewType combatViewType = DeckPanelView.CombatPowerViewType.Normal;
            
            // ライバルリーでの弱体化時表示のみ
            if (CurrentMatchType == MatchType.Hunt)
            {
                // 弱体化条件を満たしている
                if (RivalryManager.GetRegulationType(mHuntEnemy.mHuntDeckRegulationId) == HuntDeckRegulationType.Weaken && nullableSelectedPlayerDeckViewParam != null && nullableSelectedPlayerDeckViewParam.viewParams.isMatchCondition)
                {
                    combatViewType = DeckPanelView.CombatPowerViewType.Weaken;
                    // 弱体化時の戦力をセット(インゲームの表示上書き用に保持)
                    RivalryManager.Instance.FixedEnemyCombatPower = enemyDeckPanelView.viewParams.WeakenTotalCombatPower;
                }
                // 初期化
                else
                {
                    RivalryManager.Instance.FixedEnemyCombatPower = BigValue.Zero;
                }
            }
            
            // 表示を切り替える
            enemyDeckPanelView.ChangeDisplay(combatViewType);
            
            var charaId = 
                isShowPlayerDeck && nullableSelectedPlayerDeckViewParam?.viewParams.iconParams.Count > 0 ? nullableSelectedPlayerDeckViewParam.viewParams.iconParams[0].nullableCharacterData?.MCharaId ?? 0 :
                !isShowPlayerDeck && enemyDeckPanelView.viewParams.iconParams.Count > 0 ? enemyDeckPanelView.viewParams.iconParams[0].nullableCharacterData.MCharaId :
                0;

            if (charaId == 0) characterCardImage.gameObject.SetActive(false);
            else {
                characterCardImage.gameObject.SetActive(true);
                characterCardImage.SetTexture(charaId);
            }
            
            enemyDeckPanelView.SetEnemyNoticeGameObject(
                isActive: nullableSelectedPlayerDeckViewParam != null && nullableSelectedPlayerDeckViewParam.viewParams.NewTotalCombatPower < enemyDeckPanelView.viewParams.NewTotalCombatPower);

            // ステータス
            if (statusResult != null)
            {
                SetStatusResult(isShowPlayerDeck);
            }
            
            // マッチスキル
            if (isShowPlayerDeck)
            {
                var isCombinationUnlocked = CombinationManager.IsUnLockCombination();
                
                SetActivatingCombinationMatchCount(isCombinationUnlocked, nullableSelectedPlayerDeckViewParam?.viewParams.iconParams.Where(x => x.nullableCharacterData != null)
                    .Select(x => (long)x.nullableCharacterData.MCharaId).ToList());
            }
            else
            {
                
                SetActivatingCombinationMatchCount(true, enemyDeckPanelView.viewParams.iconParams.Where(x => x.nullableCharacterData != null)
                    .Select(x => (long)x.nullableCharacterData.MCharaId).ToList());
            }

            // スタミナ消費なしの吹き出し
            if (balloonCrossFade != null)
            {
                var staminaId = CurrentMatchType == MatchType.PvP || CurrentMatchType == MatchType.ClubMatch ? _pageParams.colosseumSeasonData.MColosseumEvent.mStaminaId : (long)StaminaUtility.StaminaType.RivalryBattle;
                var staminaCost = CurrentMatchType == MatchType.PvP || CurrentMatchType == MatchType.ClubMatch ? _pageParams.colosseumSeasonData.MColosseumEvent.useStaminaValue : mHunt.useStaminaValue;
                var freeStamina = StaminaUtility.GetFreeStaminaRemainingUse(staminaId, staminaCost);

                // 吹き出しバッジ表示
                // 1. スタミナ消費なし
                // 2. 回数制限
                var staminaFreeCondition = freeStamina > 0;
                var staminaFreeString = string.Format(StringValueAssetLoader.Instance["stamina.free"], freeStamina);
                var limitCondition = false;
                var limitString = string.Empty;
                if (CurrentMatchType == MatchType.Hunt)
                {
                    var usedLimit = RivalryManager.Instance.huntResultList.FirstOrDefault(data => data.mHuntTimetableId == _pageParams.huntTimetableMasterObject.id)?.dailyPlayCount ?? 0;
                    var remainingLimit = _pageParams.huntTimetableMasterObject.dailyPlayCount - usedLimit;
                    limitCondition = remainingLimit > 0;
                    limitString = string.Format(StringValueAssetLoader.Instance[_pageParams.huntTimetableMasterObject.playCountType == (long)HuntPlayCountType.Win ? "rivalry.match_limit.win" : "rivalry.match_limit.challenge"], remainingLimit);
                }
                balloonCrossFade.SetView(staminaFreeCondition, limitCondition, staminaFreeString, limitString);
            
            }
            
            // いったん条件表示はすべてオフに
            if (battleStartConditionButton != null) battleStartConditionButton.SetActive(false);
            if (weakConditionButton != null) weakConditionButton.SetActive(false);
            
            if (CurrentMatchType == MatchType.Hunt)
            {
                // 報酬ブースト
                var rewardBoostValue = RivalryManager.GetRewardBoostValue(_pageParams.huntTimetableMasterObject.id, nullableSelectedPlayerDeckViewParam);
                if (rewardBoostValue > 0)
                {
                    if (rewardBoostBadge != null) rewardBoostBadge.SetActive(true);
                    if (rewardBoostText != null) rewardBoostText.text = string.Format(StringValueAssetLoader.Instance["rivalry.rewardboost"], rewardBoostValue);
                }
                else
                {
                    if (rewardBoostBadge != null) rewardBoostBadge.SetActive(false);
                }
                if (rewardBoostButton != null) rewardBoostButton.gameObject.SetActive(RivalryManager.GetRewardBoost(_pageParams.huntTimetableMasterObject.id) != null);
                
                // 編成条件表示
                HuntDeckRegulationType regulationType = RivalryManager.GetRegulationType(mHuntEnemy.mHuntDeckRegulationId);

                // 条件タイプごとに出し分け
                switch (regulationType)
                {
                    // 条件ない場合は表示はなし
                    case HuntDeckRegulationType.None:
                    {
                        break;
                    }
                    case HuntDeckRegulationType.Weaken:
                    {
                        if (weakConditionButton != null) weakConditionButton.SetActive(true);
                        break;
                    }
                    case HuntDeckRegulationType.BattleStart:
                    {
                        if (battleStartConditionButton != null) battleStartConditionButton.SetActive(true);
                        break;
                    }
                }
            }
            else
            {
                if (rewardBoostButton != null) rewardBoostButton.gameObject.SetActive(false);
            }
            
            // 報酬一覧
            if (rewardTitleText != null && rewardTitleWarning != null && rewardList != null)
            {
                SetRewardDisplay();
            }
            
            SetClubMatchDisplay();
            UpdateKickOffButtonView();
        }
        
        private void SetActivatingCombinationMatchCount(bool isCombinationUnlocked, List<long> idList)
        {
            combinationMatchLockObject.SetActive(!isCombinationUnlocked);
            int activatingCount = CombinationManager.ActivatingCombinationMatchCount(idList);
            activatingCombinationMatchCountRoot.SetActive(isCombinationUnlocked && activatingCount > 0);
            activatingCombinationMatchCountText.text = string.Format(StringValueAssetLoader.Instance["common.combination.current_active"], activatingCount);
        }

        private void TrySaveDeck(BattleConst.DeckStrategy strategy)
        {
            var selectedBannerParameters = (DeckPanelScrollGridItem.Parameters) playerDeckScrollGrid.GetNullableBannerData();
            var selectedPartyNumber = selectedBannerParameters?.viewParams.deckId ?? 0;
            foreach(DeckData deck in _deckListData.DeckDataList)
            {
                if (deck.PartyNumber != selectedPartyNumber) continue;
                deck.Deck.optionValue = (int)strategy;
            }
            _deckListData.SaveAsync(selectedPartyNumber).Forget();
        }

        private async Task TryCallDeckSelectAPI(long selectedPartyNumber)
        {
            if (_deckListData.DeckDataList[_deckListData.SelectingIndex].PartyNumber != selectedPartyNumber) await _deckListData.SelectDeckAsync(partyNumber: selectedPartyNumber);
        }

        private int GetDeckSelectingIndex()
        {
            var selectedBannerParameters = (DeckPanelScrollGridItem.Parameters) playerDeckScrollGrid.GetNullableBannerData();
            var selectedPartyNumber = selectedBannerParameters?.viewParams.deckId ?? 0;
            var selectedIndex = _deckListData.DeckDataList.ToList().FindIndex(data => data.PartyNumber == selectedPartyNumber);
            if (selectedIndex < 0) selectedIndex = 0;
            return selectedIndex;
        }

        private void UpdateKickOffButtonView()
        {
            if (CurrentMatchType != MatchType.PvP && CurrentMatchType != MatchType.ClubMatch)
            {
                return;
            }

            var mColosseumEvent = _pageParams.colosseumSeasonData.MColosseumEvent;
            var staminaId = mColosseumEvent.mStaminaId;
            var staminaValue = mColosseumEvent.useStaminaValue;
            var enoughStamina = CheckEnoughStamina(staminaId, staminaValue);
            var interactable = GetStartButtonInteractable();
            var normalAnim = interactable ? "Normal" : "Disabled";

            if (enoughStamina)
            {
                mCostPointEscalation = null;
                startButtonAnimator.SetTrigger(normalAnim);
                SetStartButtonView(KickOffCostType.Stamina);
                staminaCostText.text = string.Format(StringValueAssetLoader.Instance["common.stamina_usage"], staminaValue);
            }
            else
            {
                var costGroupID = mColosseumEvent.mCostPointEscalationGroupId;
                var battleCountExtra = UserDataManager.Instance.GetColosseumDailyStatus(_pageParams.colosseumSeasonData.SeasonId)?.battleCountExtra ?? 0;
                var costMaster = MasterManager.Instance.costPointEscalationMaster.GetCostPoint(costGroupID, battleCountExtra);
                if (costMaster != null)
                {
                    mCostPointEscalation = costMaster;
                    startButtonAnimator.SetTrigger(normalAnim);
                    SetStartButtonView(KickOffCostType.Point);
                    staminaCostText.text = string.Format(StringValueAssetLoader.Instance["common.stamina_usage"], mCostPointEscalation.value);
                }   
                else
                {
                    mCostPointEscalation = null;
                    startButtonAnimator.SetTrigger("Grayedout");
                    SetStartButtonView(KickOffCostType.Grayout);
                }
            }
        }
        
        /// <summary> 試合開始可能か </summary>
        private bool GetStartButtonInteractable()
        {
            var nullableSelectedPlayerDeckViewParam = (DeckPanelScrollGridItem.Parameters) playerDeckScrollGrid.GetNullableBannerData();
            var ret = false;

            // 試合開始条件を満たしているか
            isMatchBattleStartCondition = true;
            
            if (nullableSelectedPlayerDeckViewParam != null)
            {
                // ライバルリーの試合開始条件が設定されている時のみ判定を行う
                if (CurrentMatchType == MatchType.Hunt)
                {
                    if (RivalryManager.GetRegulationType(mHuntEnemy.mHuntDeckRegulationId) == HuntDeckRegulationType.BattleStart)
                    {
                        isMatchBattleStartCondition = nullableSelectedPlayerDeckViewParam.viewParams.isMatchCondition;
                    }
                }
                
                // 空データがない＆試合条件を満たしているか
                ret = nullableSelectedPlayerDeckViewParam.viewParams.iconParams.TrueForAll(aData => aData.nullableCharacterData != null) && isMatchBattleStartCondition;
            }

            // 試合開始ボタンがグレーアウト時のボタン制御(空データの場合にボタンが反応しないように)
            startGrayOutButton.interactable = isMatchBattleStartCondition == false;
            
            
            if (_pageParams.colosseumSeasonData != null)
            {
                var mColosseumEvent = _pageParams.colosseumSeasonData.MColosseumEvent;
                var staminaId = mColosseumEvent.mStaminaId;
                var staminaValue = mColosseumEvent.useStaminaValue;            
                var enoughStamina = CheckEnoughStamina(staminaId, staminaValue);
                
                var costGroupID = mColosseumEvent.mCostPointEscalationGroupId;
                var battleCountExtra = UserDataManager.Instance.GetColosseumDailyStatus(_pageParams.colosseumSeasonData.SeasonId)?.battleCountExtra ?? 0;
                var costMaster = MasterManager.Instance.costPointEscalationMaster.GetCostPoint(costGroupID, battleCountExtra);

                // スタミナ不足かつpoint指定の挑戦も不可能なら非活性
                if (!enoughStamina && costMaster == null) ret = false;
            }
            
            return ret;
        }

        private void SetStartButtonView(KickOffCostType type)
        {
            foreach (var obj in costUiList)
            {
                var isOn = obj.costTypes.Contains(type);
                obj.gameObject.SetActive(isOn);
            }
        }

        private bool CheckEnoughStamina(long staminaId, long staminaValue)
        {
            var currentStamina = StaminaUtility.GetStamina(staminaId);
            var currentAdditionStamina = StaminaUtility.GetStaminaAddition(staminaId);
            return currentStamina >= staminaValue　|| currentAdditionStamina >= staminaValue;
        }

        #endregion
        
        #region API
        private async UniTask GetPvpMimicGetPreparationDetailAPI(long opponentUMasterId)
        {
            PvpMimicGetPreparationDetailAPIRequest request = new PvpMimicGetPreparationDetailAPIRequest();
            PvpMimicGetPreparationDetailAPIPost post = new PvpMimicGetPreparationDetailAPIPost();
            post.opponentUMasterId = opponentUMasterId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            var response = request.GetResponseData();
            charaList = response.deck.charaList;
            charaVariableList = response.deck.charaVariableList;
        }
        private async UniTask<PvpMimicGetBattleDataAPIResponse> GetPvpMimicGetBattleDataAPI(long opponentUMasterId)
        {
            PvpMimicGetBattleDataAPIRequest request = new PvpMimicGetBattleDataAPIRequest();
            PvpMimicGetBattleDataAPIPost post = new PvpMimicGetBattleDataAPIPost();
            post.opponentUMasterId = opponentUMasterId;
            request.SetPostData(post);
            await APIManager.Instance.Connect(request);
            return request.GetResponseData();
        }
        #endregion
        
        #region EventListeners
        public void OnClickKickoff()
        {
            // スクロール中だったらreturn
            if (playerDeckScrollGrid.ScrollGrid.IsPlayingAnimation) return;

            if (CurrentMatchType == MatchType.PvP || CurrentMatchType == MatchType.ClubMatch)
            {
                if (mCostPointEscalation != null)
                {
                    KickOffColosseumByPoint();
                }
                else
                {
                    KickOffColosseum();
                }
            }
            else if (CurrentMatchType == MatchType.Hunt)
            {
                KickOffHunt();
            }
            else
            {
                KickOffPractice();
            }
        }

        private async UniTask<bool> KickOff(StaminaUtility.StaminaType staminaType, long staminaId, long staminaValue)
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

            // パス期間チェック
            if (UserDataManager.Instance.IsExpiredPass(staminaId))
            {
                ConfirmModalWindow.Open(new ConfirmModalData(
                    StringValueAssetLoader.Instance["common.confirm"], 
                    StringValueAssetLoader.Instance["pass.expired"], 
                    string.Empty, 
                    new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], window => 
                    {
                        Init();
                        window.Close();
                    })));
                return false;
            }

            var selectedBannerParameters = (DeckPanelScrollGridItem.Parameters) playerDeckScrollGrid.GetNullableBannerData();
            var selectedPartyNumber = selectedBannerParameters?.viewParams.deckId ?? 0;
            var teamCaptainMCharaId = selectedBannerParameters?.viewParams.iconParams[0].nullableCharacterData?.MCharaId;
            var teamCaptainMChara = MasterManager.Instance.charaMaster.FindData((long)teamCaptainMCharaId);
            AppManager.Instance.UIManager.System.TouchGuard.Show();
            await TryCallDeckSelectAPI(selectedPartyNumber);
            await VoiceManager.Instance.PlayInVoiceForLocationTypeAsync(teamCaptainMChara, LocationType.IN_YELL);
            AppManager.Instance.UIManager.System.TouchGuard.Hide();
            return true;
        }

        private async void KickOffHunt()
        {
            var isSuccess = await KickOff(StaminaUtility.StaminaType.RivalryBattle, (long)StaminaUtility.StaminaType.RivalryBattle, mHunt.useStaminaValue);
            if (!isSuccess) return;
            RivalryManager.Instance.OnRivalryBattleStart(mHuntEnemy.id, mHuntTimetable.id);
        }
        
        private async void KickOffColosseum()
        {
            if (!KickOffTirednessCheck()) return;
            if (!KickOffClubMatchOverCheck()) return;
            var mColosseumEvent = _pageParams.colosseumSeasonData.MColosseumEvent;
            var isSuccess = await KickOff(StaminaUtility.StaminaType.Colosseum, mColosseumEvent.mStaminaId, mColosseumEvent.useStaminaValue);
            if (!isSuccess) return;
            var costType = mCostPointEscalation == null ? 1L : 2L;
            
            var selectedBannerParameters = (DeckPanelScrollGridItem.Parameters) playerDeckScrollGrid.GetNullableBannerData();
            var selectedPartyNumber = selectedBannerParameters?.viewParams.deckId ?? 0;

            var costValue = mCostPointEscalation == null ? mColosseumEvent.useStaminaValue : mCostPointEscalation.value;
            ColosseumManager.OnClickColosseumInGameButton(
                _pageParams.openFrom,
                _pageParams.colosseumUser.userType, 
                _pageParams.colosseumUser.uMasterId,
                _pageParams.colosseumUser.ranking,
                costType,
                costValue,
                _pageParams.colosseumSeasonData.SeasonId,
                selectedPartyNumber);
        }

        private void KickOffColosseumByPoint()
        {
            if (mCostPointEscalation == null) return;
            var cost = mCostPointEscalation.value;
            var hasCost = UserDataManager.Instance.point.Find(mCostPointEscalation.mPointId);
            var value = hasCost?.value ?? 0;
            if (cost > value)
            {
                var data = new CommonExecuteConfirmPointShortageModal.Data();
                data.pointId = mCostPointEscalation.mPointId;
                data.value = mCostPointEscalation.value;
                AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.CommonExecuteConfirmPointShortage, data);
                return;
            }
            var pointMaster = MasterManager.Instance.pointMaster.FindData(mCostPointEscalation.mPointId);
            var param = new CommonExecuteConfirmModal.Data(
                mCostPointEscalation.mPointId,
                mCostPointEscalation.value,
                StringValueAssetLoader.Instance["common.confirm"],
                string.Format(StringValueAssetLoader.Instance["pvp.kickoff.confirm"], pointMaster.name,
                    mCostPointEscalation.value, pointMaster.unitName),
                String.Empty, KickOffColosseum);
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.CommonExecuteConfirm, param);
        }
        
        private bool KickOffTirednessCheck()
        {
            if (CurrentMatchType != MatchType.ClubMatch) return true;

            
            var selectingIndex = GetDeckSelectingIndex();
            _deckListData.DeckDataList[selectingIndex].UpdateFatigueValue();
            var canMatch = _deckListData.DeckDataList[selectingIndex].FixedClubConditionData.condition is not ClubDeckCondition.Awful;
            if (!canMatch)
            {
                ClubDeckPage.ShowFatigueRestrictionPopup();
            }
            return canMatch;
        }

        private bool KickOffClubMatchOverCheck()
        {
            if (CurrentMatchType != MatchType.ClubMatch) return true;

            var canMatch = _pageParams.colosseumSeasonData.UserSeasonStatus.endAt.TryConvertToDateTime().IsFuture(AppTime.Now);
            if (!canMatch)
            {
                ShowClubMatchOverPopup();
            }
            return canMatch;
        }

        private async void KickOffPractice()
        {
            var selectedBannerParameters = (DeckPanelScrollGridItem.Parameters) playerDeckScrollGrid.GetNullableBannerData();
            var selectedPartyNumber = selectedBannerParameters?.viewParams.deckId ?? 0;
            var teamCaptainMCharaId = selectedBannerParameters?.viewParams.iconParams[0].nullableCharacterData?.MCharaId;
            var teamCaptainMChara = MasterManager.Instance.charaMaster.FindData((long)teamCaptainMCharaId);
            AppManager.Instance.UIManager.System.TouchGuard.Show();
            await TryCallDeckSelectAPI(selectedPartyNumber);
            await VoiceManager.Instance.PlayInVoiceForLocationTypeAsync(teamCaptainMChara, LocationType.IN_YELL);
            // PageManagerからスタックを削除
            AppManager.Instance.UIManager.PageManager.RemovePageStack(PageType.TeamConfirmTrainingMatch);
            AppManager.Instance.UIManager.System.TouchGuard.Hide();
            PvpMimicGetBattleDataAPIResponse response = await GetPvpMimicGetBattleDataAPI(_pageParams.targetUMasterId);
            OpenInGame(response.clientData);
        }

        private void OpenInGame(BattleV2ClientData clientData)
        {
            AppManager.Instance.UIManager.PageManager.OpenPage(PageType.NewInGame, false,
                new InGame.NewInGameOpenArgs(_pageParams.openFrom, clientData, 
                    new TeamConfirmPage.PageParams(
                        PageType.Home, null, 
                        _pageParams.targetUMasterId,
                        _pageParams.enemyStatus
                    )
                )
            );
        }

        public void OnClickSwitchDeckButton()
        {
            SwitchDisplay(!isShowPlayerDeck);
            UpdateDisplay();
        }

        private void OnChangePage(int index)
        {
            UpdateDisplay();
        }

        public async void OnClickBack()
        {
            // ToDo: 元画面がTeamConfirmPageだったらPageManagerが同じページ扱っちゃうので仮処理。。。
            PageType targetPage = _pageParams.openFrom != PageType.TeamConfirm && _pageParams.openFrom != PageType.TeamConfirmTrainingMatch ? _pageParams.openFrom : PageType.Rivalry;
            //ページがイベントかどうか
            bool isEvent = RivalryManager.GetMatchType(_pageParams.huntTimetableMasterObject, _pageParams.huntMasterObject) == RivalryMatchType.Event;
            if (targetPage == PageType.Rivalry && isEvent)
            {
                RivalryPage.Data pageData = new();
                long usedLimit = RivalryManager.Instance.huntResultList.FirstOrDefault(data => data.mHuntTimetableId == mHuntTimetable.id)?.dailyPlayCount ?? 0;
                bool isLimit = mHuntTimetable.dailyPlayCount > 0 && usedLimit >= mHuntTimetable.dailyPlayCount;
                List<HuntEnemyMasterObject> huntEnemyMasterObjectList = new();
                foreach (HuntEnemyMasterObject huntEnemyMasterObject in MasterManager.Instance.huntEnemyMaster.values)
                {
                    if (huntEnemyMasterObject.mHuntId == mHunt.id)
                    {
                        huntEnemyMasterObjectList.Add(huntEnemyMasterObject);
                    }
                }
                pageData.pageType = RivalryPageType.RivalryEvent;
                pageData.args = new RivalryEventPage.PageParams{huntMasterObject = mHunt, huntTimetableMasterObject = mHuntTimetable, HuntEnemyMasterObjectList = huntEnemyMasterObjectList, autoTransitToEventTop = isLimit};
                _pageParams.backArgs = pageData;
            }
            await AppManager.Instance.UIManager.PageManager.OpenPageAsync(targetPage, false, _pageParams.backArgs);
            
            // モーダル表示しないページ
            bool openModal = true;
            foreach (var NoProfilePage in NoProfilePageList)
            {
                if (AppManager.Instance.UIManager.PageManager.CurrentPageType == NoProfilePage)
                {
                    openModal = false;
                    break;
                }
            }
            if(AppManager.Instance.UIManager.PageManager.CurrentPageObject is HomePage homePage)
            {
                if(homePage.CurrentPageType == HomePageType.LoginBonus)
                {
                    openModal = false;
                }
                
                if (SystemUnlockDataManager.Instance.IsUnlockPrepared)
                {
                    openModal = false;
                }
            }
            if (CurrentMatchType == MatchType.Practice && openModal)
            {
                // PageManagerからスタックを削除
                AppManager.Instance.UIManager.PageManager.RemovePageStack(PageType.TeamConfirmTrainingMatch);
                
                // todo: トレーニングマッチ導線実装次第動作確認
                TrainerCardModalWindow.WindowParams param = new TrainerCardModalWindow.WindowParams(_pageParams.targetUMasterId);
                await AppManager.Instance.UIManager.ModalManager.OpenModalAsync(ModalType.TrainerCard, param);
            }
            else
            {
                AppManager.Instance.UIManager.PageManager.RemovePageStack(PageType.TeamConfirm);
            }
        }

        public void OnClickExitConditions()
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.ExitConditions, null);
        }

        private async void OnClickDeckEditButton(long partyNumber)
        {
            var targetPage = CurrentMatchType == MatchType.ClubMatch ? PageType.ClubDeck : PageType.Deck;
            await AppManager.Instance.UIManager.PageManager.OpenPageAsync(targetPage, true, new DeckPageParameters{InitialPartyNumber = partyNumber, OpenFrom = PageType.TeamConfirm});
        }
        
        public void OnClickCombinationMatchButton()
        {
            if (isShowPlayerDeck && !CombinationManager.IsUnLockCombination())
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
            
            var iconParams = (isShowPlayerDeck? ((DeckPanelScrollGridItem.Parameters)playerDeckScrollGrid.GetNullableBannerData())?.viewParams.iconParams: enemyDeckPanelView.viewParams.iconParams);
            if(iconParams is null) return;
            CombinationMatchModal.Open(new CombinationMatchModal.WindowParams()
            {
                IdList = iconParams
                    .Where(x => x.nullableCharacterData != null)
                    .Select(x => (long)x.nullableCharacterData.MCharaId).ToList(),
                IsPlayerDeck = isShowPlayerDeck
            });
        }

        public void OnClickBoostEffect()
        {
            BoostEffectModal.Open(
                new BoostEffectModal.Data{
                    huntSpecificCharaMasterObject = RivalryManager.GetRewardBoost(_pageParams.huntTimetableMasterObject.id), 
                    huntTimetableMasterObject = _pageParams.huntTimetableMasterObject, 
                    deckParameters = (DeckPanelScrollGridItem.Parameters) playerDeckScrollGrid.GetNullableBannerData(),
                    showCurrentEffect = true,
                    showCharaIconActivation = true,
                }
            );
        }

        public void OnClickClubMatchDeckList()
        {
            for(int i=0; i<_deckListData.DeckDataList.Length; i++)
            {
                _deckListData.DeckDataList[i].UpdateFatigueValue();
            }

            ClubTeamSummaryWindow.Open(new ClubTeamSummaryWindow.WindowParams
            { 
                DeckList = _deckListData.DeckDataList,
                SelectingIndex = GetDeckSelectingIndex(),
                OnClosed = OnChangedDeck,
            });
        }

        /// <summary> 弱体化条件ボタン </summary>
        public void OnClickWeakRegulationButton()
        {
            OnClickRegulationButton(HuntDeckRegulationType.Weaken);
        }
        
        /// <summary> 試合開始条件ボタン </summary>
        public void OnClickBattleStartRegulationButton()
        {
            OnClickRegulationButton(HuntDeckRegulationType.BattleStart);
        }

        /// <summary> 試合開始ボタンがグレーアウト時の処理 </summary>
        public void OnClickBattleStartGrayOutButton()
        {
            // 試合開始条件を満たしていない場合条件を表示する
            if (isMatchBattleStartCondition == false)
            {
                OnClickBattleStartRegulationButton();
                return;
            }
        }

        /// <summary> 条件表示モーダルを開く </summary>
        private void OnClickRegulationButton(HuntDeckRegulationType regulationType)
        {
            string modalTitle = String.Empty;
            
            switch (regulationType)
            {
                // 条件ない場合は導線ないけどリターンしておく
                case HuntDeckRegulationType.None:
                {
                    return;
                }
                case HuntDeckRegulationType.Weaken:
                {
                    modalTitle = StringValueAssetLoader.Instance["rivalry.regulation_type.weaken"];
                    break;
                }
                case HuntDeckRegulationType.BattleStart:
                {
                    modalTitle = StringValueAssetLoader.Instance["rivalry.regulation_type.battle_start"];
                    break;
                }
            }

            // 説明文
            string description = mHuntEnemy.subName;

            ConfirmModalData modalData = new ConfirmModalData(modalTitle, description, string.Empty,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"],
                    window =>
                    {
                        window.Close();
                    }));
            
            ConfirmModalWindow.Open(modalData);
        }

        private void OnChangedDeck(long index)
        {
            playerDeckScrollGrid.SetIndex((int)index, true);
            UpdateDisplay();
        }
        
        public static void ShowClubMatchOverPopup()
        {
            ConfirmModalWindow.Open(new ConfirmModalData(
                StringValueAssetLoader.Instance["club.season.end_title"],
                StringValueAssetLoader.Instance["club.season.end_content"], string.Empty,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"],
                    (window => 
                    {
                        window.Close();
                        AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Home, false, null); 
                    }))));
        }


        #endregion
    }
}
