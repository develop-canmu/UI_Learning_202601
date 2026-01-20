using System;
using System.Collections.Generic;
using CruFramework.UI;
using Pjfb.Combination;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb
{
    public class CombinationCollectionScrollData
    {
        public readonly long MCombinationId;
        public readonly bool ShowBadge;
        public readonly bool ShowDetailButton;
        public readonly bool ShowCharaLevel;
        public readonly List<CharacterDetailData> CharacterDetailDataList;
        public IReadOnlyDictionary<long, UserDataChara> MCharaIdPossessionDictionary;
        public SkillDataListCollection SkillDataLists;
        public readonly Action<long> OnProgressCombinationCollectionAction;

        public CombinationCollectionScrollData(long mCombinationId, bool showBadge,
            bool showDetailButton, bool showCharaLevel, List<CharacterDetailData> characterDetailDataList,
            IReadOnlyDictionary<long, UserDataChara> mCharaIdPossessionDictionary,
            SkillDataListCollection skillDataLists,
            Action<long> onProgressCombinationCollection = null)
        {
            MCombinationId = mCombinationId;
            ShowBadge = showBadge;
            ShowDetailButton = showDetailButton;
            ShowCharaLevel = showCharaLevel;
            CharacterDetailDataList = characterDetailDataList;
            MCharaIdPossessionDictionary = mCharaIdPossessionDictionary;
            SkillDataLists = skillDataLists;
            OnProgressCombinationCollectionAction = onProgressCombinationCollection;
        }
    }
    
    public class CombinationCollectionScrollDynamicItem : ScrollDynamicItem
    {
        [SerializeField] private CombinationCollectionView combinationCollectionView;
        
        private CombinationCollectionScrollData scrollData;


        protected override void OnSetView(object value)
        {
            scrollData = (CombinationCollectionScrollData)value;

            combinationCollectionView.Initialize(scrollData);
            Canvas.ForceUpdateCanvases();
            RecalculateSize();
        }
        
    }
}
