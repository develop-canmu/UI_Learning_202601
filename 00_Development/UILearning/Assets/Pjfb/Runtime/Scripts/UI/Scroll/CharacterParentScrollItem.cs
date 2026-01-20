using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using TMPro;
using UnityEngine;

namespace Pjfb.Encyclopedia
{
    public class CharacterParentScrollItem : ScrollGridItem
    {
        [SerializeField] private CharacterParentIcon characterParentIcon;
        [SerializeField] [StringValue] private string PossessionString;
        [SerializeField] private TextMeshProUGUI characterParentNameText;
        [SerializeField] private TextMeshProUGUI characterParentEnglishNameText;
        [SerializeField] private TextMeshProUGUI possessionCountText;
        [SerializeField] private GameObject cover;
        [SerializeField] private GameObject badge;
        private CharaParentData data;
        
        public void OnSelected()
        {
            TriggerEvent(data);
        }
        
        protected override void OnSetView(object value)
        {
            data = (CharaParentData)value;
            characterParentIcon.SetIconId(data.CharaParentBase.parentMCharaId);
            characterParentNameText.text = data.MCharaParent?.name ?? string.Empty;
            characterParentEnglishNameText.text =
                data.MCharaParent?.MCharaLibraryProfileDictionary.GetValueOrDefault(CharacterProfileType.EnglishName, string.Empty);
            possessionCountText.text = string.Format(StringValueAssetLoader.Instance[PossessionString], data.PossessionCount, data.MaxCount);
            badge.SetActive(data.CharaParentBase.hasTrustPrize);
            cover.SetActive(data.PossessionCount == 0);
        }
        
        
    }
}