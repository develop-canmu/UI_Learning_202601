using System;
using Pjfb.UserData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Character
{
    public class CharacterRankPowerChangesView : MonoBehaviour
    {
        public enum HighlightState
        {
            Current,
            After
        }
        
        [SerializeField] private RankPowerUI currentStatus;
        [SerializeField] private RankPowerUI afterStatus;

        [SerializeField] [ColorValue] private string imageNormalColorValueKey;
        [SerializeField] [ColorValue] private string imageHighlightColorValueKey;
        
        [SerializeField] [ColorValue] private string textNormalColorValueKey;
        [SerializeField] [ColorValue] private string textHighlightColorValueKey;
        
        
        [SerializeField] private Image currentStatusImage;
        [SerializeField] private TextMeshProUGUI currentStatusText;
        
        
        [SerializeField] private Image afterStatusImage;
        [SerializeField] private TextMeshProUGUI afterStatusText;
        
        [SerializeField] private TextMeshProUGUI statusDifferenceText;
        [SerializeField] private OmissionTextSetter statusDifferenceOmissionTextSetter;
        [SerializeField] [ColorValue] private string statusDifferencePlusColorValueKey;
        [SerializeField] [ColorValue] private string statusDifferenceMinusColorValueKey;
        [SerializeField] [ColorValue] private string statusDifferenceZeroColorValueKey;
        
        
        private void Awake()
        {
            SetCurrentStatusHighlight();
        }

        public void InitializeUI(UserDataCharaVariable currentChara, UserDataCharaVariable afterChara)
        {
            CharacterVariableDetailData currentCharaDetail = currentChara == null ? null : new CharacterVariableDetailData(currentChara);
            CharacterVariableDetailData afterCharaDetail = afterChara == null ? null : new CharacterVariableDetailData(afterChara);
            currentStatus.InitializeCharacterVariableRankUI(currentCharaDetail);
            afterStatus.InitializeCharacterVariableRankUI(afterCharaDetail);
            if(afterCharaDetail == null)
            {
                statusDifferenceText.text = StringValueAssetLoader.Instance["deck.difference_plus"] + "0";
                statusDifferenceText.color = ColorValueAssetLoader.Instance[statusDifferenceZeroColorValueKey];
            }
            else if(currentCharaDetail == null)
            {
                statusDifferenceText.text = StringValueAssetLoader.Instance["deck.difference_plus"] + afterCharaDetail.CombatPower.ToDisplayString(statusDifferenceOmissionTextSetter.GetOmissionData());
                statusDifferenceText.color = ColorValueAssetLoader.Instance[statusDifferencePlusColorValueKey];
            }
            else
            {
                BigValue difference = afterCharaDetail.CombatPower - currentCharaDetail.CombatPower;
                statusDifferenceText.text = difference.ToDisplayString(statusDifferenceOmissionTextSetter.GetOmissionData());
                if (difference > 0)
                {
                    statusDifferenceText.text = StringValueAssetLoader.Instance["deck.difference_plus"] + statusDifferenceText.text;
                    statusDifferenceText.color = ColorValueAssetLoader.Instance[statusDifferencePlusColorValueKey];
                }
                else if (difference == 0)
                {
                    statusDifferenceText.text = StringValueAssetLoader.Instance["deck.difference_plus"] + statusDifferenceText.text;
                    statusDifferenceText.color = ColorValueAssetLoader.Instance[statusDifferenceZeroColorValueKey];
                }
                else
                {
                    statusDifferenceText.color = ColorValueAssetLoader.Instance[statusDifferenceMinusColorValueKey];
                }
                
            }
        }

        public void SetHighlight(HighlightState state)
        {
            switch (state)
            {
                case HighlightState.Current:
                    SetCurrentStatusHighlight();
                    break;
                case HighlightState.After:
                    SetAfterStatusHighlight();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        
        private void SetCurrentStatusHighlight()
        {
            currentStatusImage.color = ColorValueAssetLoader.Instance[imageHighlightColorValueKey];
            currentStatusText.color =  ColorValueAssetLoader.Instance[textHighlightColorValueKey];
            afterStatusImage.color = ColorValueAssetLoader.Instance[imageNormalColorValueKey];
            afterStatusText.color =  ColorValueAssetLoader.Instance[textNormalColorValueKey];
            
            currentStatus.EnableHighlight();
            afterStatus.DisableHighlight();
        }

        private void SetAfterStatusHighlight()
        {
            currentStatusImage.color = ColorValueAssetLoader.Instance[imageNormalColorValueKey];
            currentStatusText.color =  ColorValueAssetLoader.Instance[textNormalColorValueKey];
            afterStatusImage.color = ColorValueAssetLoader.Instance[imageHighlightColorValueKey];
            afterStatusText.color =  ColorValueAssetLoader.Instance[textHighlightColorValueKey];
            
            
            currentStatus.DisableHighlight();
            afterStatus.EnableHighlight();
            
        }
    }
}

