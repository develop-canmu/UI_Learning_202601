using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb.InGame
{
    public class NewInGameCharacterIcon : MonoBehaviour
    {
        [SerializeField] private CharacterVariableIcon characterIcon;
        [SerializeField] private InGameStaminaParts staminaParts;
        [SerializeField] private GameObject isAceDisplay;
        [SerializeField] private GameObject iconFrameRoot;
        [SerializeField] private GameObject[] iconFrames;
        public CanvasGroup CanvasGroup;
        [HideInInspector] public bool IsInitialPositioning = true;
        
        public void SetCharacterIcon(BattleCharacterModel characterModel)
        {
            characterIcon.SetIconTextureWithEffectAsync(characterModel.MCharaId).Forget();
            foreach (var frame in iconFrames)
            {
                frame.SetActive(false);
            }
            isAceDisplay.SetActive(characterModel.IsAceCharacter);
            iconFrames[(int)characterModel.Side].SetActive(true);
        }

        public void SetFrameActive(bool isActive)
        {
            iconFrameRoot.SetActive(isActive);
        }

        public void SetStaminaGauge(float rate)
        {
            staminaParts.SetUI(rate);
        }

        public void SetActiveStaminaGauge(bool flg)
        {
            staminaParts.gameObject.SetActive(flg);
        }
    }
}