using System;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Page;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;

namespace Pjfb.Deck
{
    public class DeckRecommendationsModalWindow : ModalWindow
    {
        [SerializeField] private DeckView currentDeck;
        [SerializeField] private DeckView afterDeck;
        [SerializeField] private TextMeshProUGUI combatPowerDifferenceText;
        [SerializeField] private OmissionTextSetter combatPowerDifferenceOmissionTextSetter;
        [SerializeField] [ColorValue] private string statusDifferencePlusColorValueKey;
        [SerializeField] [ColorValue] private string statusDifferenceMinusColorValueKey;
        [SerializeField] [ColorValue] private string statusDifferenceZeroColorValueKey;

        private DeckSlotCharacter[] recommendedCharaList;
        
        #region Params

        public class WindowParams
        {
            public DeckData deckData;
            public List<RecommendationsTargetCharacter>  recommendationsTargetCharacterList;
            public Action onApply;
            public Action onClosed;
        }

        #endregion
        private WindowParams _windowParams;
        
        
        public static void Open(WindowParams data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.DeckRecommendationsConfirm, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            _windowParams = (WindowParams) args;
            Init();
            return base.OnPreOpen(args, token);
        }

        #region PrivateMethods
        private void Init()
        {
            currentDeck.InitializeUI(_windowParams.deckData);
            
            var recommendationsCharacters = DeckRecommendationsUtility.GetRecommendedDeck(_windowParams.recommendationsTargetCharacterList);
            recommendedCharaList = recommendationsCharacters;
            afterDeck.InitializeUI(recommendationsCharacters);
            // 編成後の総戦力を計算
            BigValue afterCombatPower;
            foreach (DeckSlotCharacter deckSlotCharacter in recommendationsCharacters)
            {
                afterCombatPower += deckSlotCharacter.Chara.combatPower;
            }
            BigValue differenceCombatPower = afterCombatPower - _windowParams.deckData.CombatPower;
            combatPowerDifferenceText.text = differenceCombatPower.ToDisplayString(combatPowerDifferenceOmissionTextSetter.GetOmissionData());
            if (differenceCombatPower > 0)
            {
                combatPowerDifferenceText.text = StringValueAssetLoader.Instance["deck.difference_plus"] + combatPowerDifferenceText.text;
                combatPowerDifferenceText.color = ColorValueAssetLoader.Instance[statusDifferencePlusColorValueKey];
            }
            else if (differenceCombatPower == 0)
            {
                combatPowerDifferenceText.text = StringValueAssetLoader.Instance["deck.difference_plus"] + combatPowerDifferenceText.text;
                combatPowerDifferenceText.color = ColorValueAssetLoader.Instance[statusDifferenceZeroColorValueKey];
            }
            else
            {
                combatPowerDifferenceText.color = ColorValueAssetLoader.Instance[statusDifferenceMinusColorValueKey];
            }
        }
        #endregion

        #region EventListeners

        public void OnClickClose()
        {
            Close(onCompleted: _windowParams.onClosed);
        }

        public void OnClickApply()
        {
            bool hasChanged = false;
            for (int i = 0;i<_windowParams.deckData.MemberCount;i++)
            {
                if (recommendedCharaList[i].Chara.id != _windowParams.deckData.GetMemberId(i) ||
                    recommendedCharaList[i].Position != _windowParams.deckData.GetMemberPosition(i))
                {
                    hasChanged = true;
                    break;
                }
            }

            if (hasChanged)
            {
                _windowParams.deckData.SetRecommendedChara(recommendedCharaList);
                Close(onCompleted: _windowParams.onApply);    
            }
            else
            {
                OnClickClose();
            }
            
        }

        #endregion
       
        
        
    }
}
