using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework.UI;
using Pjfb.Character;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
    
    public class SpecialSupportCharacterScrollItem : ScrollGridItem
    {
        [SerializeField]
        private SpecialSupportCardIcon characterIcon = null;
        
        [SerializeField] private GameObject selectingRoot;
        [SerializeField] private GameObject badge = null;
        CharacterScrollData characterData = null;
        
        [SerializeField]
        private Sprite selectedSprite = null;
        [SerializeField]
        private Sprite cantSelectSprite = null;
        [SerializeField]
        private Sprite specialAttackSprite = null;
        [SerializeField]
        private ImageCrossfade imageCrossfade = null;
        
        [SerializeField]
        private Image filter = null;
        [SerializeField]
        private TMPro.TMP_Text messageText = null;
        
        /// <summary>UGUI</summary>
        public void OnSelected()
        {
            TriggerEvent(characterData);
        }
        
        protected override void OnSetView(object value)
        {
            characterData = (CharacterScrollData)value;
            // アイコンに登録
            characterIcon.SetIcon(characterData.CharacterId, characterData.CharacterLv, characterData.LiberationLv);
            selectingRoot.SetActive(characterData.IsSelecting);
            // 強化/能力解放可能バッジ表示
            badge.SetActive(characterData.HasOption(CharacterScrollDataOptions.Badge));

            List<Sprite> labelList = new List<Sprite>();
            // 選択中表示
            if(characterData.HasOption(CharacterScrollDataOptions.Selected))
            {
                labelList.Add(selectedSprite);
            }
            
            // 選択不可
            if(characterData.HasOption(CharacterScrollDataOptions.Selected) == false && characterData.HasOption(CharacterScrollDataOptions.Disable))
            {
                labelList.Add(cantSelectSprite);
            }

            // 特攻
            if(characterData.HasOption(CharacterScrollDataOptions.ScenarioSpecialAttack))
            {
                labelList.Add(specialAttackSprite);
            }
            
            // メッセージ
            if(string.IsNullOrEmpty(characterData.Message))
            {
                filter.gameObject.SetActive(false);
            }
            else
            {
                filter.gameObject.SetActive(true);
                messageText.text = characterData.Message;
                filter.color = characterData.FilterColor;
            }

            imageCrossfade.SetSpriteList(labelList);

            characterIcon.SwipeableParams = characterData.SwipeableParams;
        }
        
        protected override void OnSelectedItem()
        {
            selectingRoot.gameObject.SetActive(true);
        }
        
        protected override void OnDeselectItem()
        {
            selectingRoot.gameObject.SetActive(false);
        }
    }
}