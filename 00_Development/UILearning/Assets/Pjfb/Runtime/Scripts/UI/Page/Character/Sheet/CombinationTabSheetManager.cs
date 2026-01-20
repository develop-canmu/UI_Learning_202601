using System.Collections.Generic;
using CruFramework.Page;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.Character
{
    public enum CombinationTabSheetType
    {
        CombinationTrainingScrollDynamic,
        CombinationCollectionScrollDynamic,
    }

    public class CombinationTabSheetManager : SheetManager<CombinationTabSheetType>
    {
        [SerializeField] private CombinationTrainingTabSheet combinationTrainingTabSheet;
        [SerializeField] private CombinationCollectionTabSheet combinationCollectionTabSheet;

        public void Initialize(IReadOnlyList<CombinationOpenedMinimum> openedCollections, IReadOnlyList<CharacterDetailData> supportCharaList)
        {
            combinationTrainingTabSheet.Initialize(supportCharaList);
            combinationCollectionTabSheet.Initialize(openedCollections);
        }
    }
}
