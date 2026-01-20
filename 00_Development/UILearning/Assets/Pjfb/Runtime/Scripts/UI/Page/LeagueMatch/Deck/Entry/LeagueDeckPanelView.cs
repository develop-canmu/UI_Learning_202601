using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pjfb.Deck;
using Pjfb.Extensions;
using Pjfb.Gacha;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Pjfb.LeagueMatch;
using Pjfb.UserData;
using Unity.VisualScripting;

namespace Pjfb.UI
{
    public class LeagueDeckPanelView : MonoBehaviour
    {
        #region ViewParams
        public class ViewParams
        {
            public long DeckId;
            public bool IsPlayerDeck;
            public string DeckName;
            public long PenaltyValue = 0;
            public Color RankBgColor;
            public BattleConst.DeckStrategy Strategy;
            public Action<long> OnClickDeckEditButton;
            public List<DeckPanelCharaIconView.ViewParams> IconParams;
            public Action<BattleConst.DeckStrategy> OnStrategyChanged;
            
            public BigValue NewTotalCombatPower { get; private set; }
            public BigValue OriginTotalCombatPower { get; private set; }

            public ViewParams(
                long deckId,
                bool isPlayerDeck,
                string deckName,
                BattleConst.DeckStrategy strategy,
                Color rankBgColor,
                Action<long> onClickDeckEditButton,
                List<DeckPanelCharaIconView.ViewParams> iconParams,
                Action<BattleConst.DeckStrategy> onStrategyChanged,
                long penaltyValue
            )
            {
                this.DeckId = deckId;
                this.IsPlayerDeck = isPlayerDeck;
                this.DeckName = deckName;
                this.Strategy = strategy;
                this.RankBgColor = rankBgColor;
                this.OnClickDeckEditButton = onClickDeckEditButton;
                this.IconParams = iconParams;
                this.OnStrategyChanged = onStrategyChanged;
                this.PenaltyValue = penaltyValue;

                OriginTotalCombatPower = BigValue.Zero;
                NewTotalCombatPower = BigValue.Zero;

                BigValue combatPowerAmplifier = BigValue.RateValue;
                foreach (var viewParam in iconParams)
                {
                    BigValue charaCombatPower = viewParam.nullableCharacterData?.CombatPower ?? BigValue.Zero;
                    if (charaCombatPower == 0) continue;
                    OriginTotalCombatPower += charaCombatPower;
                    NewTotalCombatPower += BigValue.MulRate(charaCombatPower, combatPowerAmplifier);
                }

                if (penaltyValue > 0)
                {
                    NewTotalCombatPower = NewTotalCombatPower * (100 - penaltyValue) / 100;
                }
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
        [Header("LeagueMatch")]
        [SerializeField] private GameObject textRegistered;
        [SerializeField] private GameObject coverRegistered;

        #endregion

        #region Properties
        public ViewParams viewParams { get; private set; }
        #endregion
        
        #region PublicMethods
        public void Init()
        {
            charaIcons.ForEach(anIcon => anIcon.Init());
            editDeckButton.SetActive(false);
            deckNameText.text = string.Empty;
            rankDescriptionText.text = string.Empty;
            SetEnemyNoticeGameObject(isActive: false);
            textRegistered.SetActive(false);
            coverRegistered.SetActive(false);
        }

        public void SetDisplay(ViewParams viewParams)
        {
            this.viewParams = viewParams;
            rankBg.color = viewParams.RankBgColor;
            editDeckButton.SetActive(viewParams.IsPlayerDeck);
            deckNameText.text = viewParams.DeckName;
            rankDescriptionText.text = viewParams.NewTotalCombatPower.ToDisplayString(omissionTextSetter.GetOmissionData());
            rankDescriptionText.color = ColorValueAssetLoader.Instance[viewParams.PenaltyValue > 0 ? "highlight.orange" : "white"];
            rankPenaltyArrow.SetActive(viewParams.PenaltyValue > 0);
            var rankData = MasterManager.Instance.charaRankMaster.FindDataByTypeAndPower(CharaRankMasterStatusType.PartyTotal, viewParams.OriginTotalCombatPower);
            if (rankData == null) rankImage.gameObject.SetActive(false);
            else {
                rankImage.gameObject.SetActive(true);
                rankImage.SetTexture(rankData.rankNumber);
            }
            
            for (var i = 0; i < Mathf.Min(charaIcons.Count, viewParams.IconParams.Count); i++)
            {
                charaIcons[i].SetDisplay(viewParams.IconParams[i]);    
            }

            // 登録されている試合番号
            long entryRoundNumber = LeagueMatchUtility.GetEntryRoundNumber(viewParams.DeckId);
            // デッキがスロットにエントリー済みか
            bool isSlotEntry = entryRoundNumber != 0;
           
            strategyButton.interactable = !isSlotEntry && viewParams.IconParams.TrueForAll(aData => aData.nullableCharacterData != null);
            SetStrategy(viewParams.Strategy);

            powerUpObject.SetActive(viewParams.NewTotalCombatPower > viewParams.OriginTotalCombatPower);
            powerDownObject.SetActive(viewParams.NewTotalCombatPower < viewParams.OriginTotalCombatPower);

            if (viewParams.NewTotalCombatPower == viewParams.OriginTotalCombatPower)
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
            
            // 別スロットに登録済みの表示(どこかのスロットに登録されており、今選択している試合番号ではない)
            bool registeredAnotherSlot = isSlotEntry && LeagueMatchUtility.SelectedRoundNumber != entryRoundNumber;
            coverRegistered.SetActive(registeredAnotherSlot);
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
            viewParams?.OnClickDeckEditButton?.Invoke(viewParams.DeckId);
        }
        
        public void OnClickSelectStrategyButton()
        {
            StrategyChoiceModalWindow.Open(new StrategyChoiceModalWindow.WindowParams
            {
                strategy = viewParams.Strategy,
                onClosed = null,
                onStrategyChanged = (strategy) =>
                {
                    viewParams.Strategy = strategy;
                    SetStrategy(strategy);
                    viewParams.OnStrategyChanged(strategy);
                }
            });
        }
        #endregion
    }
}