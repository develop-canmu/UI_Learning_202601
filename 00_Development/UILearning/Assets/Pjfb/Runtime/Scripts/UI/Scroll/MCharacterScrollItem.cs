using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework.UI;
using TMPro;
using UnityEngine;

namespace Pjfb
{
    public class MCharacterScrollData
    {
        /// <summary>キャラId</summary>
        public long MCharaId { get; }
        public bool IsSelected;
        public bool HasCharacter { get; } = false;
        
        public MCharacterScrollData(long mCharaId, bool hasCharacter)
        {
            MCharaId = mCharaId;
            HasCharacter = hasCharacter;
        }
    }
    public class MCharacterScrollItem : ScrollGridItem
    {
        [Flags]
        private enum ActiveParam
        {
            None = 0,
            Rarity = 1 << 0,
            Lv = 1 << 1,
        }
        
        [SerializeField] private CharacterIcon characterIcon = null;
        [SerializeField] private GameObject selectedRoot;
        [SerializeField] private GameObject cover;
        [SerializeField] private ActiveParam activeParam = ActiveParam.None;
        MCharacterScrollData data = null;
        
        /// <summary>UGUI</summary>
        public void OnSelected()
        {
            TriggerEvent(data);
        }
        
        protected override void OnSetView(object value)
        {
            data = (MCharacterScrollData)value;
            
            characterIcon.SetIcon( data.MCharaId );
            characterIcon.SetActiveRarity((activeParam & ActiveParam.Rarity) == ActiveParam.Rarity);
            characterIcon.SetActiveLv((activeParam & ActiveParam.Lv) == ActiveParam.Lv);

            if(selectedRoot != null)
            {
                selectedRoot.SetActive(data.IsSelected);
            }
            
            if(cover != null)
            {
                cover.SetActive(!data.HasCharacter);
            }
        }
    }
}