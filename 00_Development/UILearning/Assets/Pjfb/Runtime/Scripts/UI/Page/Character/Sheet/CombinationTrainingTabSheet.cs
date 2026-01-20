using System.Collections.Generic;
using CruFramework.Page;
using UnityEngine;

namespace Pjfb.Character
{
    public class CombinationTrainingTabSheet : Sheet
    {
        [SerializeField] private CombinationTrainingScrollDynamic scrollDynamic;

        public void Initialize(IReadOnlyList<CharacterDetailData> supportCharaList)
        {
            scrollDynamic.Initialize(supportCharaList);
        }
    }
}
