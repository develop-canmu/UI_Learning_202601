using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using TMPro;

namespace Pjfb
{
    public class DeckNameView : MonoBehaviour
    {
        private const int DeckNameCharacterMaxCount = 8;
        
        [SerializeField] private TMP_InputField inputField;

        private DeckData deckData;

        private void Awake()
        {
            if (inputField is not null)
            {
                inputField.enabled = false;
                inputField.onValidateInput = (currentStr, index, inputChar) => StringUtility.OnValidateInput(currentStr, index, inputChar, DeckNameCharacterMaxCount,inputField.fontAsset);   
            }
        }

        public void SetDeckData(DeckData deckData)
        {
            this.deckData = deckData;
            inputField.text = deckData.Deck.name;
        }

        public void SetDeckName(string deckName)
        {
            inputField.text = deckName;
        }
        
        public void OnClickChangeNameButton()
        {
            inputField.enabled = true;
            inputField.Select();
        }

        public async void OnEndEditDeckName()
        {
            inputField.enabled = false;
            string newDeckName = StringUtility.GetLimitNumCharacter(inputField.text, DeckNameCharacterMaxCount, true);
            if (string.IsNullOrEmpty(newDeckName))
            {
                inputField.text = deckData.Deck.name;
            }
            else
            {
                try
                {
                    await deckData.SaveDeckNameAsync(newDeckName);
                    inputField.text = newDeckName;
                }
                catch
                {
                    inputField.text = deckData.Deck.name;
                }
            }
        }
        
        
    }
}