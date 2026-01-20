using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pjfb.ClubMatch;
using Pjfb.Deck;
using Pjfb.Extensions;
using Pjfb.Master;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.UI
{
    public class DeckPanelView : MonoBehaviour
    {

        /// <summary> 戦力表示タイプ </summary>
        public enum CombatPowerViewType
        {
            // 通常表示
            Normal,
            // 弱体化時の表示
            Weaken
        }
        
        private const float UpdateConditionInterval = 1.0f;
    
        #region ViewParams
        public class ViewParams
        {
            public long deckId;
            public bool isPlayerDeck;
            public string deckName;
            public long penaltyValue = 0;
            public Color rankBgColor;
            public BattleConst.DeckStrategy strategy;
            public Action<long> onClickDeckEditButton;
            public List<DeckPanelCharaIconView.ViewParams> iconParams;
            public Action<BattleConst.DeckStrategy> onStrategyChanged;
            public ClubConditionData conditionData;
            // 条件を満たしているか
            public bool isMatchCondition = true;


            private BigValue newTotalCombatPower = default;

            public BigValue NewTotalCombatPower
            {
                get
                {
                    // 表示タイプ毎に返す戦力データを変える
                    switch (CombatPowerViewType)
                    {
                        // 通常表示
                        case CombatPowerViewType.Normal:
                        {
                            return newTotalCombatPower;
                        }
                        // 弱体化時
                        case CombatPowerViewType.Weaken:
                        {
                            return weakenTotalCombatPower;
                        }
                        // それ以外は通常の戦力を返す
                        default:
                        {
                            return newTotalCombatPower;
                        }
                    }
                }
            }

            private BigValue originTotalCombatPower = default;
            public BigValue OriginTotalCombatPower => originTotalCombatPower;

            private BigValue weakenTotalCombatPower = default;
            // 弱体化時の戦力
            public BigValue WeakenTotalCombatPower => weakenTotalCombatPower;

            // 戦力表示タイプ
            private CombatPowerViewType combatPowerViewType = CombatPowerViewType.Normal;
            public CombatPowerViewType CombatPowerViewType => combatPowerViewType;
            
            public ViewParams(long deckId, bool isPlayerDeck, string deckName, BattleConst.DeckStrategy strategy, Color rankBgColor, Action<long> onClickDeckEditButton, List<DeckPanelCharaIconView.ViewParams> iconParams, Action<BattleConst.DeckStrategy> onStrategyChanged, long penaltyValue = 0, ClubConditionData conditionData = null, long weakenTotalCombatPower = 0, bool isMatchCondition = true)
            {
                this.deckId = deckId;
                this.isPlayerDeck = isPlayerDeck;
                this.deckName = deckName;
                this.strategy = strategy;
                this.rankBgColor = rankBgColor;
                this.onClickDeckEditButton = onClickDeckEditButton;
                this.iconParams = iconParams;
                this.onStrategyChanged = onStrategyChanged;
                this.penaltyValue = penaltyValue;
                this.conditionData = conditionData;
                this.isMatchCondition = isMatchCondition;

                originTotalCombatPower = BigValue.Zero;
                newTotalCombatPower = BigValue.Zero;
                this.weakenTotalCombatPower = new BigValue(weakenTotalCombatPower);
                combatPowerViewType = CombatPowerViewType.Normal;
                
                BigValue combatPowerAmplifier = conditionData?.combatPowerAmplifier ?? BigValue.RateValue;
                foreach (var viewParam in iconParams)
                {
                    BigValue charaCombatPower = viewParam.nullableCharacterData?.CombatPower ?? BigValue.Zero;
                    if(charaCombatPower == 0)   continue;
                    originTotalCombatPower += charaCombatPower;
                }
                newTotalCombatPower += BigValue.MulRate(OriginTotalCombatPower, combatPowerAmplifier);

                if (penaltyValue > 0)
                {
                    newTotalCombatPower = NewTotalCombatPower * (100 - penaltyValue) / 100;
                }
                    
            }

            /// <summary> 戦力表示パラメータセット </summary>
            public void SetCombatPowerViewType(CombatPowerViewType type)
            {
                combatPowerViewType = type;
            }
        }
        #endregion

        #region SerializeFields
        [SerializeField] private List<DeckPanelCharaIconView> charaIcons;
        [SerializeField] private TextMeshProUGUI deckNameText;
        [SerializeField] private TextMeshProUGUI rankDescriptionText;
        [SerializeField] private OmissionTextSetter omissionTextSetter;
        [SerializeField] private Image rankBg;
        [SerializeField] private GameObject rankPenaltyArrow;
        [SerializeField] private GameObject editDeckButton;
        [SerializeField] private GameObject enemyNoticeGameObject;
        [SerializeField] private DeckRankImage rankImage;
        [SerializeField] private TextMeshProUGUI strategyText;
        [SerializeField] private UIButton strategyButton;
        
        
        [SerializeField] private GameObject powerUpObject;
        [SerializeField] private GameObject powerDownObject;
        
        [Header("Club")]
        [SerializeField] private GameObject cannotBattleBadge;
        #endregion

        #region Properties
        public ViewParams viewParams { get; private set; }
        #endregion
        
        #region Feidls
        
        private DeckData deck = null;
        private float updateConditionInterval = 0;

        #endregion
        
        #region PublicMethods
        public void Init()
        {
            charaIcons.ForEach(anIcon => anIcon.Init());
            editDeckButton.SetActive(false);
            deckNameText.text = string.Empty;
            rankDescriptionText.text = string.Empty;
            SetEnemyNoticeGameObject(isActive: false);
        }

        public void SetDisplay(ViewParams viewParams)
        {
            this.viewParams = viewParams;
            rankBg.color = viewParams.rankBgColor;
            editDeckButton.SetActive(viewParams.isPlayerDeck);
            deckNameText.text = viewParams.deckName;
            rankDescriptionText.text = viewParams.NewTotalCombatPower.ToDisplayString(omissionTextSetter.GetOmissionData());
            rankDescriptionText.color = ColorValueAssetLoader.Instance[viewParams.penaltyValue > 0 ? "highlight.orange" : "white"];
            rankPenaltyArrow.SetActive(viewParams.penaltyValue > 0);
            var rankData = MasterManager.Instance.charaRankMaster.FindDataByTypeAndPower(CharaRankMasterStatusType.PartyTotal, viewParams.OriginTotalCombatPower);
            if (rankData == null) rankImage.gameObject.SetActive(false);
            else {
                rankImage.gameObject.SetActive(true);
                rankImage.SetTexture(rankData.rankNumber);
            }
            
            for (var i = 0; i < Mathf.Min(charaIcons.Count, viewParams.iconParams.Count); i++)
            {
                charaIcons[i].SetDisplay(viewParams.iconParams[i]);    
            }

            strategyButton.interactable = viewParams.iconParams.TrueForAll(aData => aData.nullableCharacterData != null);
            SetStrategy(viewParams.strategy);

            powerUpObject.SetActive(viewParams.NewTotalCombatPower > viewParams.OriginTotalCombatPower);
            powerDownObject.SetActive(viewParams.NewTotalCombatPower < viewParams.OriginTotalCombatPower);
            
            
            if (viewParams.conditionData is not null)
            {
                rankDescriptionText.color = viewParams.conditionData.combatPowerTextColor;
                UpdateConditionCannotBattleBadge(viewParams.conditionData.condition);
            }
            else
            {
                // 戦力が増減しないか表示タイプが弱体化の時は通常色で戦力表示する(弱体化時は背景と色が似ているので)
                if (viewParams.NewTotalCombatPower == viewParams.OriginTotalCombatPower || viewParams.CombatPowerViewType == CombatPowerViewType.Weaken)
                {
                    rankDescriptionText.color = (ColorValueAssetLoader.Instance["white"]);
                }
                else if (viewParams.NewTotalCombatPower < viewParams.OriginTotalCombatPower)
                {
                    rankDescriptionText.color = (ColorValueAssetLoader.Instance["f661b4"]);
                }
                else
                {
                    rankDescriptionText.color = (ColorValueAssetLoader.Instance["8e9ffa"]);
                }
            }
            
            CacheDeckData().Forget();
        }

        /// <summary> 表示切り替え </summary>
        public void ChangeDisplay(CombatPowerViewType viewType)
        {
            // 現在の表示と同じならリターン
            if (viewParams.CombatPowerViewType == viewType)
            {
                return;
            }
            // 表示タイプセット
            viewParams.SetCombatPowerViewType(viewType);
            // 表示更新
            SetDisplay(viewParams);
        }
        
        private async UniTask CacheDeckData()
        {
            deck = null;
            // 一定間隔でコンディションの回復をチェック
            DeckListData deckList = await DeckUtility.GetClubBattleDeck();
            deck = deckList.GetDeck(viewParams.deckId);
        }
        
        private void UpdateConditionCannotBattleBadge(ClubDeckCondition condition)
        {
            cannotBattleBadge.SetActive(condition is ClubDeckCondition.Awful);
        }
        
        private void Update()
        {
                
            if(deck != null)
            {
                updateConditionInterval += Time.deltaTime;
                if(updateConditionInterval >= UpdateConditionInterval)
                {
                    updateConditionInterval = 0;
                    // コンディションの更新
                    deck.UpdateFatigueValue();
                    if(deck.FixedClubConditionData != null)
                    {
                        UpdateConditionCannotBattleBadge(deck.FixedClubConditionData.condition);
                    }
                }
            }
        }

        /// <summary>
        /// 「格上の相手です」バッジアイコン
        /// </summary>
        public void SetEnemyNoticeGameObject(bool isActive)
        {
            enemyNoticeGameObject.SetActive(isActive);
        }
        #endregion

        #region PrivateMethods
        private void SetStrategy(BattleConst.DeckStrategy strategy)
        {
            if (strategyText == null) return;
            strategyText.text = strategy switch
            {
                BattleConst.DeckStrategy.Aggressive => StringValueAssetLoader.Instance["deck.strategy.aggressive"],
                BattleConst.DeckStrategy.Dribble => StringValueAssetLoader.Instance["deck.strategy.dribble"],
                BattleConst.DeckStrategy.Pass => StringValueAssetLoader.Instance["deck.strategy.pass"],
                _ => "-"
            };
        }
        #endregion

        #region PublicMethods
        public void OnClickDeckEditButton()
        {
            viewParams?.onClickDeckEditButton?.Invoke(viewParams.deckId);
        }
        
        public void OnClickSelectStrategyButton()
        {
            StrategyChoiceModalWindow.Open(new StrategyChoiceModalWindow.WindowParams
            {
                strategy = viewParams.strategy,
                onClosed = null,
                onStrategyChanged = (strategy) =>
                {
                    viewParams.strategy = strategy;
                    SetStrategy(strategy);
                    viewParams.onStrategyChanged(strategy);
                }
            });
        }
        #endregion
    }
}