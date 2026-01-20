using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Pjfb.Combination;

namespace Pjfb
{
    public class CombinationCollectionPracticeSkillView : CombinationPracticeSkillView
    {
        [SerializeField] private GameObject buttonRoot;
        [SerializeField] private UIButton activeButton;
        
        private long mCombinationId;
        private long mProgressId;
        private Action<long> onProgressCombinationCollectionAction;

        
        public void InitializeUI(List<PracticeSkillInfo> skillDataList, bool isLock, string lockString, bool showHighlight)
        {
            InitializeSkill(skillDataList, isLock, lockString, showHighlight);
        }

        public void InitializeUniqueUi(long combinationId, long progressId, bool showActiveButton, bool isActiveButtonInteractable, Action<long> onProgressCombinationCollection)
        {
            mCombinationId = combinationId;
            mProgressId = progressId;
            buttonRoot.SetActive(showActiveButton);
            activeButton.interactable = isActiveButtonInteractable;
            onProgressCombinationCollectionAction = onProgressCombinationCollection;
        }
        
        public void OnClickProgressButton()
        {
            CombinationManager
                .CombinationCollectionProgress(mCombinationId, mProgressId, onProgressCombinationCollectionAction)
                .Forget();
        }
    }
}