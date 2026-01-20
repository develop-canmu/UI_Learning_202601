using System.Collections.Generic;
using CruFramework.Page;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.Character
{
    public enum SuccessCharacterDetailTabSheetType
    {
        PracticeAbility, //練習能力
        TrainingDetail, //育成時の編成
        Combination, //スキルコネクト
    }

    public class SuccessCharacterDetailTabSheetManager : SheetManager<SuccessCharacterDetailTabSheetType>
    {
        [SerializeField] private CombinationTabSheetManager combinationTabSheetManager;
        [SerializeField] private GameObject trainingDetailTab;
        [SerializeField] private GameObject combinationTab;

        public void Initialize(IReadOnlyList<CombinationOpenedMinimum> openedCollections, IReadOnlyList<CharacterDetailData> supportCharaList, bool shouldShowTrainingInfo)
        {
            trainingDetailTab.SetActive(shouldShowTrainingInfo);
            combinationTab.SetActive(shouldShowTrainingInfo);
            
            if (shouldShowTrainingInfo)
            {
                combinationTabSheetManager.Initialize(openedCollections, supportCharaList);
            }
            else
            {
                OpenSheet(SuccessCharacterDetailTabSheetType.PracticeAbility, null);
            }

            var tabs = GetComponentsInChildren<SuccessCharacterDetailTabSheetButton>();
            foreach (var item in tabs)
            {
                item.UpdateSprite();
            }
        }
    }
}
