using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework.UI;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.UserData;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Pjfb
{
    
    public class CharacterScrollItem : ScrollGridItem
    {
        [SerializeField] protected CharacterIcon characterIcon = null;
        [SerializeField] protected GameObject selectingRoot;
        
        [SerializeField] private GameObject friendSettingRoot = null;
        [SerializeField] protected GameObject nonPossessionRoot = null;
        [SerializeField] private GameObject badge = null;
        
        [SerializeField] Sprite selectedSprite = null;
        [SerializeField] Sprite cantSelectSprite = null;
        [SerializeField] Sprite followSprite = null;
        [SerializeField] Sprite mutualFollowSprite = null;
        [SerializeField] Sprite specialAttackSprite = null;
        [SerializeField] Sprite currentSelectSprite = null;
        
        [SerializeField] private ImageCrossfade imageCrossfade = null;
        
        [SerializeField]
        private Image textRoot = null;
        [SerializeField]
        private TMPro.TMP_Text messageText = null;
        
        public CharacterScrollData characterData = null;

        /// <summary>UGUI</summary>
        public void OnSelected()
        {
            TriggerEvent(characterData);
        }
        protected override void OnSetView(object value)
        {
            characterData = (CharacterScrollData)value;
            CharaMasterObject mChara = MasterManager.Instance.charaMaster.FindData(characterData.CharacterId);
            characterIcon.SetIcon(characterData.CharacterId, characterData.CharacterLv, characterData.LiberationLv);
            characterIcon.CanLiberation = characterData.HasOption(CharacterScrollDataOptions.CanLiberation);
            characterIcon.CanGrowth = characterData.HasOption(CharacterScrollDataOptions.CanGrowth);
            // 未所持の場合はレベルを表示しない。
            characterIcon.SetActiveLv(!characterData.HasOption(CharacterScrollDataOptions.NonPossession) && !characterData.HasOption(CharacterScrollDataOptions.StealReward));
            friendSettingRoot.SetActive(characterData.HasOption(CharacterScrollDataOptions.FriendSetting));
            // 未所持表示
            nonPossessionRoot.SetActive(characterData.HasOption(CharacterScrollDataOptions.NonPossession));
            // フェードさせるスプライト
            List<Sprite> fadeLabelSpriteList = new List<Sprite>();
            // 選択中表示
            if(characterData.HasOption((CharacterScrollDataOptions.Selected)))
            {
                fadeLabelSpriteList.Add(selectedSprite);
            }
            // 選択不可
            if(characterData.HasOption(CharacterScrollDataOptions.Selected) == false && characterData.HasOption(CharacterScrollDataOptions.Disable))
            {
                fadeLabelSpriteList.Add(cantSelectSprite);
            }
            // フォロー表示
            if(characterData.HasOption(CharacterScrollDataOptions.Follow))
            {
                fadeLabelSpriteList.Add(followSprite);
            }
            // 相互フォロー表示
            if(characterData.HasOption(CharacterScrollDataOptions.MutualFoolow))
            {
                fadeLabelSpriteList.Add(mutualFollowSprite);
            }
            // 特攻
            if(characterData.HasOption(CharacterScrollDataOptions.ScenarioSpecialAttack))
            {
                fadeLabelSpriteList.Add(specialAttackSprite);
            }

            // 現在選択中の枠に編成中
            if (characterData.HasOption(CharacterScrollDataOptions.CurrentSelect))
            {
                fadeLabelSpriteList.Add(currentSelectSprite);
            }
            
            imageCrossfade.SetSpriteList(fadeLabelSpriteList);
            
            // 強化/能力解放または解放可能バッジ表示
            badge.SetActive(characterData.HasOption(CharacterScrollDataOptions.Badge));
            characterIcon.SwipeableParams = characterData.SwipeableParams;
            characterIcon.GetTrainingScenarioId = characterData.GetTrainingScenarioId;
            // メッセージ
            if(string.IsNullOrEmpty(characterData.Message))
            {
                textRoot.gameObject.SetActive(false);
            }
            else
            {
                textRoot.gameObject.SetActive(true);
                messageText.text = characterData.Message;
                textRoot.color = characterData.FilterColor;
            }

            // 選択エフェクト
            if (characterData.IsSelecting)
            {
                SelectItem();
            }

            characterIcon.BaseCharaType = characterData.BaseCharacterType;
        }

        protected override void OnSelectedItem()
        {
            selectingRoot.SetActive(true);
        }
        
        protected override void OnDeselectItem()
        {
            selectingRoot.SetActive(false);
        }
    }
}