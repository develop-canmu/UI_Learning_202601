using System;
using System.Collections.Generic;
using System.Linq;
using Pjfb.Character;
using Pjfb.ClubDeck;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using UnityEngine;

namespace Pjfb.Deck
{
    public class DeckView : MonoBehaviour
    {
        [SerializeField] protected RankPowerUI rankPowerUI;
        [SerializeField] protected List<DeckCharacterView> deckCharacterCells;
        [SerializeField] protected DeckNameView deckNameView;
        [SerializeField] protected TextMeshProUGUI strategyText;
        [SerializeField] private GameObject coverObject;
        [SerializeField] private UIButton resetButton;
        [SerializeField] private UIButton deckRecommendationButton;
        [SerializeField] private UIButton strategyChoiceButton;
        [SerializeField] private TextMeshProUGUI coverText = null;

        [Header("Club Deck")]
      
        
        public Action<int, int> OnClickChara;
        public Action<int> OnClickEditPosition = null;
        public Func<long, HashSet<long>> GetRecommendationExcludedIdList;
        public Action OnClickReset;
        public Action OnApplyDeckRecommendations;
        public Action OnStrategyChanged;
        
        
        protected DeckData deckData;
        private List<CharacterScrollData> characterData;

        /// <summary>編成ロックされているか</summary>
        protected bool IsLocked
        {
            get { return deckData.IsLocked || deckData.IsLockedPeriod; }
        }
        
        protected virtual string GetCoverText(DeckData data)
        {
            return string.Empty;
        }

        public virtual void InitializeUI(DeckData data)
        {
            deckData = data;
            if (deckNameView != null) deckNameView.SetDeckData(deckData);

            rankPowerUI.InitializePartyRankUI(deckData.CombatPower);
            List<CharacterVariableDetailData> detailOrderList = new List<CharacterVariableDetailData>();
            int scrollIndex = 0;
            for (int i = 0; i < deckCharacterCells.Count; i++)
            {
                int slotIndex = i;
                
                long memberId = deckData.GetMemberId(i); 
                if (memberId != DeckUtility.EmptyDeckSlotId)
                {
                    deckCharacterCells[i].SetDetailOrderList(new SwipeableParams<CharacterVariableDetailData>(detailOrderList, scrollIndex++));
                    detailOrderList.Add(new CharacterVariableDetailData(UserDataManager.Instance.charaVariable.Find(memberId)));
                }
                
                deckCharacterCells[i].InitializeUI(i, memberId, deckData.GetMemberPosition(i));
                deckCharacterCells[i].SetActions((id) => OnClickEditPosition?.Invoke(id) , (id) => OnClickChara?.Invoke(id, slotIndex));
            }

            SetStrategy(deckData.Deck);
            
            if (IsLocked)
            {
                SetCoverText();
                SetCoverObjectActive(true);
                SetResetButtonInteractable(false);
                SetDeckRecommendationButtonInteractable(false);
                SetStrategyChoiceButtonInteractable(false);
            }
            else
            {
                SetCoverObjectActive(false);
                SetResetButtonInteractable(true);
                SetDeckRecommendationButtonInteractable(true);
                SetStrategyChoiceButtonInteractable(true);
            }
        }

        public void InitializeUI(DeckSlotCharacter[] recommendationsCharacters)
        {
            List<CharacterVariableDetailData> detailOrderList = new List<CharacterVariableDetailData>();
            int scrollIndex = 0;
            BigValue combatPower = BigValue.Zero;
            for (int i = 0; i < deckCharacterCells.Count; i++)
            {
                deckCharacterCells[i].InitializeUI(i,
                    recommendationsCharacters[i].Chara.id,
                    recommendationsCharacters[i].Position);
                combatPower += recommendationsCharacters[i].Chara.combatPower;
                detailOrderList.Add(new CharacterVariableDetailData(UserDataManager.Instance.charaVariable.Find(recommendationsCharacters[i].Chara.id)));
                deckCharacterCells[i].SetDetailOrderList(new SwipeableParams<CharacterVariableDetailData>(detailOrderList, scrollIndex++));
            }
            rankPowerUI.InitializePartyRankUI(combatPower);
        }

        public void InitializeUI(CharaVariableProfileStatus[] charaVariableList, DeckBase deckBase)
        {
            bool isEmpty = charaVariableList is null || charaVariableList.Length == 0;
            if (isEmpty)
            {
                charaVariableList = new CharaVariableProfileStatus[DeckUtility.BattleDeckSlotCount];
                for (int i = 0; i < DeckUtility.BattleDeckSlotCount; i++)
                {
                    charaVariableList[i] = new CharaVariableProfileStatus();
                }
            }
            
            rankPowerUI.InitializePartyRankUI(deckBase.rank, new BigValue(deckBase.combatPower));
            for (int i = 0; i < deckCharacterCells.Count; i++)
            {
                var chara = isEmpty ? charaVariableList[i] : charaVariableList.First(c => c.id == deckBase.memberIdList[i].l[1]);
                deckCharacterCells[i].InitializeUI(i, chara);
            }

            SetStrategy(deckBase);
        }

        private void SetStrategy(DeckBase deckBase)
        {
            if (strategyText == null) return;
            strategyText.text = (BattleConst.DeckStrategy)deckBase.optionValue switch
            {
                BattleConst.DeckStrategy.Aggressive => StringValueAssetLoader.Instance["deck.strategy.aggressive"],
                BattleConst.DeckStrategy.Dribble => StringValueAssetLoader.Instance["deck.strategy.dribble"],
                BattleConst.DeckStrategy.Pass => StringValueAssetLoader.Instance["deck.strategy.pass"],
                _ => "-"
            };
        }

        public void OnClickResetButton()
        {
            if (!deckData.CanEditDeck)
            {
                ClubDeckPage.ShowFatigueDeckEditRestrictionPopup();
                return;
            }
            deckData.SetDeckEmpty();
            OnClickReset?.Invoke();
        }

        public void OnClickDeckRecommendationsButton()
        {
            if (!deckData.CanEditDeck)
            {
                ClubDeckPage.ShowFatigueDeckEditRestrictionPopup();
                return;
            }

            var recommendationsTargetCharacterList =
                DeckRecommendationsUtility.GetRecommendTargetChara(
                    GetRecommendationExcludedIdList?.Invoke(deckData.Index) ??
                    new HashSet<long>());
            
            if (recommendationsTargetCharacterList.Count < DeckUtility.BattleDeckSlotCount)
            {
                ConfirmModalWindow.Open(new ConfirmModalData(
                    StringValueAssetLoader.Instance["character.deck.recommendation_chara_insufficient_title"],
                    StringValueAssetLoader.Instance["character.deck.recommendation_chara_insufficient_content"], string.Empty,
                    new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"],
                        (window => window.Close()))));
                return;
            }
            
            
            DeckRecommendationsModalWindow.Open(new DeckRecommendationsModalWindow.WindowParams
            {
                deckData = deckData,
                recommendationsTargetCharacterList = recommendationsTargetCharacterList,
                onApply = OnApplyDeckRecommendations,
                onClosed = null,
            });
        }
        
        public void OnClickSelectStrategyButton()
        {
            StrategyChoiceModalWindow.Open(new StrategyChoiceModalWindow.WindowParams
            {
                strategy = (BattleConst.DeckStrategy)deckData.Deck.optionValue,
                onClosed = null,
                onStrategyChanged = (strategy) =>
                {
                    deckData.Deck.optionValue = (int)strategy;
                    OnStrategyChanged();
                }
            });
        }

        private void SetCoverText()
        {
            if(coverText != null)
            {
                coverText.text = GetCoverText(deckData);
            }
        }
        
        protected void SetCoverText(string message)
        {
            if(coverText != null)
            {
                coverText.text = message;
            }
        }
        
        public void SetCoverObjectActive(bool value)
        {
            if(coverObject != null)
            {
                coverObject.SetActive(value);
            }
        }
        
        private void SetResetButtonInteractable(bool value)
        {
            if(resetButton != null)
            {
                resetButton.interactable = value;
            }
        }
        
        private void SetDeckRecommendationButtonInteractable(bool value)
        {
            if(deckRecommendationButton != null)
            {
                deckRecommendationButton.interactable = value;
            }
        }
        
        private void SetStrategyChoiceButtonInteractable(bool value)
        {
            if(strategyChoiceButton != null)
            {
                strategyChoiceButton.interactable = value;
            }
        }
        
    }
}