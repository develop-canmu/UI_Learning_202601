using System;
using System.Collections;
using System.Collections.Generic;
using CruFramework.UI;
using Pjfb.Character;
using Pjfb.Extensions;
using Pjfb.Master;
using Pjfb.UserData;
using TMPro;
using UnityEngine;

namespace Pjfb
{
    public class BoostCharaScrollData
    {
        public BoostCharaScrollData(long mCharaId, bool hasChara, long rate, SwipeableParams<CharacterDetailData> swipeableParams, bool isInDeck = true, bool showActivation = false)
        {
            MCharaId = mCharaId;
            HasChara = hasChara;
            IsInDeck = isInDeck;
            Rate = rate;
            SwipeableParams = swipeableParams;
            ShowActivation = showActivation;
        }
        public long MCharaId;
        public bool HasChara;
        public bool IsInDeck;
        public long Rate;
        public bool ShowActivation;
        public SwipeableParams<CharacterDetailData> SwipeableParams;
    }
    public class BoostCharacterIcon : ScrollGridItem
    {
        [SerializeField] private CharacterIcon characterIcon = null;
        [SerializeField] private GameObject cover;
        [SerializeField] private TextMeshProUGUI coverText;
        [SerializeField] private TextMeshProUGUI rateText;
        [SerializeField] private GameObject effectActivationBadge;
        
        
        public BoostCharaScrollData BoostCharacterData { get; private set; }
        
        public void InitializeUI(BoostCharaScrollData data)
        {
            BoostCharacterData = data;
            CharaMasterObject mChara = MasterManager.Instance.charaMaster.FindData(BoostCharacterData.MCharaId);
            characterIcon.SetIcon(BoostCharacterData.MCharaId);
            characterIcon.CanLiberation = false;
            characterIcon.SetActiveLv(false);
            characterIcon.SwipeableParams = BoostCharacterData.SwipeableParams;
            characterIcon.SetActiveRarity(false);
            cover.SetActive(!BoostCharacterData.HasChara || !BoostCharacterData.IsInDeck);
            coverText.text = !BoostCharacterData.HasChara ? string.Format(StringValueAssetLoader.Instance["rivalry.boosteffect.notowned"]) : string.Format(StringValueAssetLoader.Instance["rivalry.boosteffect.notindeck"]);
            if (effectActivationBadge != null) effectActivationBadge.SetActive(BoostCharacterData.ShowActivation && BoostCharacterData.Rate > 0 && BoostCharacterData.HasChara && BoostCharacterData.IsInDeck);
            rateText.text = string.Format(StringValueAssetLoader.Instance["common.percent_value"], (BoostCharacterData.Rate/100f).ToString("0.##"));
        }

        protected override void OnSetView(object value)
        {
            InitializeUI((BoostCharaScrollData) value);
        }
    }
}