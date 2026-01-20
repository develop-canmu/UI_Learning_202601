using System.Collections.Generic;
using CruFramework.UI;
using Pjfb.Combination;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb
{ 
    public class CombinationTrainingScrollData
    {
        public CombinationTrainingScrollData(long mCharaTrainingComboBuffId, List<CharacterDetailData> characterDetailDataList, Dictionary<long, UserDataChara> mCharaIdPossessionDictionary, 
            List<CombinationManager.TrainingMultipleSkillData> activatedSkillDataList, List<CombinationManager.TrainingMultipleSkillData> lockSkillDataList, 
            CombinationViewBase<CombinationPracticeSkillView, CombinationManager.TrainingMultipleSkillData>.NotActiveLabelType notActiveLabelType, bool showDetailButton)
        {
            MCharaTrainingComboBuffId = mCharaTrainingComboBuffId;
            CharacterDetailDataList = characterDetailDataList;
            MCharaIdPossessionDictionary = mCharaIdPossessionDictionary;
            ActivatedSkillDataList = activatedSkillDataList;
            LockSkillDataList = lockSkillDataList;
            NotActiveLabelType = notActiveLabelType;
            ShowDetailButton = showDetailButton;
        }

        public readonly long MCharaTrainingComboBuffId;
        public readonly List<CharacterDetailData> CharacterDetailDataList;
        public readonly Dictionary<long, UserDataChara> MCharaIdPossessionDictionary;
        public readonly List<CombinationManager.TrainingMultipleSkillData> ActivatedSkillDataList;
        public readonly List<CombinationManager.TrainingMultipleSkillData> LockSkillDataList;
        public readonly
            CombinationViewBase<CombinationPracticeSkillView, CombinationManager.TrainingMultipleSkillData>.NotActiveLabelType NotActiveLabelType;

        public readonly bool ShowDetailButton;
    }
    
    public class CombinationTrainingScrollDynamicItem : ScrollDynamicItem
    {
        [SerializeField] private CombinationTrainingView combinationTrainingView;
        
        private CombinationTrainingScrollData scrollData;
        
        protected override void OnSetView(object value)
        {
            scrollData = (CombinationTrainingScrollData)value;

            combinationTrainingView.Initialize(scrollData.ShowDetailButton, scrollData.MCharaTrainingComboBuffId, scrollData.CharacterDetailDataList,
                scrollData.MCharaIdPossessionDictionary, scrollData.ActivatedSkillDataList,
                scrollData.LockSkillDataList, scrollData.NotActiveLabelType);
            Canvas.ForceUpdateCanvases();
            RecalculateSize();
        }
    }
}