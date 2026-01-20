using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Combination;

namespace Pjfb.Character
{
    public class CombinationTopPage : Page
    {
        [SerializeField] private GameObject combinationMatchBadge;
        [SerializeField] private GameObject combinationTrainingBadge;
        [SerializeField] private GameObject combinationCollectionBadge;
        
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            // 開放状況の更新
            await CombinationManager.GetCombinationCollectionListOpenedAPI();
            UpdateBadges();
            await base.OnPreOpen(args, token);
        }
        
        protected override void OnOpened(object args)
        {
            AppManager.Instance.TutorialManager.OpenSkillConnectTutorialAsync().Forget();
            base.OnOpened(args);
        }

        private void UpdateBadges()
        {
            combinationMatchBadge.SetActive(CombinationManager.HasNewCombinationMatchBadge);
            combinationTrainingBadge.SetActive(CombinationManager.HasNewCombinationTrainingBadge);
            combinationCollectionBadge.SetActive(CombinationManager.CanActiveCombinationCollectionBadge);
        }
        
        public void OnClickCombinationMatchButton()
        {
            CharacterPage m = (CharacterPage)Manager;
            m.OpenPage(CharacterPageType.CombinationMatch, true, null);
        }
        
        public void OnClickCombinationTrainingButton()
        {
            CharacterPage m = (CharacterPage)Manager;
            m.OpenPage(CharacterPageType.CombinationTraining, true, null);
        }
        
        public void OnClickCombinationCollectionButton()
        {
            CharacterPage m = (CharacterPage)Manager;
            m.OpenPage(CharacterPageType.CombinationCollection, true, null);
        }
    }
}