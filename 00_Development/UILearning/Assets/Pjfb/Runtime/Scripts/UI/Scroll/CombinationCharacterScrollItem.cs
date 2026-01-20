using System.Collections.Generic;
using CruFramework.UI;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb
{
    public class CombinationCharacterScrollData
    {
        public CombinationCharacterScrollData(CharacterDetailData characterDetailData, IReadOnlyDictionary<long, UserDataChara> mCharaIdPossessionDictionary, bool showCharaLevel)
        {
            CharacterDetailData = characterDetailData;
            MCharaIdPossessionDictionary = mCharaIdPossessionDictionary;
            ShowCharaLevel = showCharaLevel;
        }

        public readonly CharacterDetailData CharacterDetailData;
        public readonly IReadOnlyDictionary<long, UserDataChara> MCharaIdPossessionDictionary;
        public readonly bool ShowCharaLevel;
    }

    public class CombinationCharacterScrollItem : ScrollGridItem
    {
        [SerializeField] private CombinationCharacterIcon combinationCharacterIcon;
        private CombinationCharacterScrollData scrollData;

        protected override void OnSetView(object value)
        {
            scrollData = (CombinationCharacterScrollData)value;
            combinationCharacterIcon.Initialize(scrollData.CharacterDetailData, scrollData.MCharaIdPossessionDictionary, scrollData.ShowCharaLevel);
        }
    }
}
