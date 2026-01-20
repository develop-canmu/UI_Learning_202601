using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using UnityEngine;

namespace Pjfb.Combination
{
    public class CombinationCollectionPracticeSkillListModal : ModalWindow
    {
        public class Data
        {
            public IReadOnlyCollection<PracticeSkillInfo> CollectionPracticeSkillDataList { get; }

            public Data(IReadOnlyCollection<PracticeSkillInfo> collectionPracticeSkillDataList)
            {
                CollectionPracticeSkillDataList = collectionPracticeSkillDataList;
            }
        }
        
        [SerializeField] private ScrollGrid scrollGrid;
        [SerializeField] private GameObject noActivatingCombinationText;
        
        public static void Open(Data data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.CombinationCollectionPracticeSkillList, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            var data = (Data)args;
            
            Init(data.CollectionPracticeSkillDataList);
            return base.OnPreOpen(args, token);
        }

        private void Init(IReadOnlyCollection<PracticeSkillInfo> collectionPracticeSkillDataList)
        {
            scrollGrid.SetItems(CreatePracticeSkillDataList(collectionPracticeSkillDataList));
        }
        
        private List<PracticeSkillViewMiniGridItem.Info> CreatePracticeSkillDataList(IReadOnlyCollection<PracticeSkillInfo> collectionPracticeSkillDataList)
        {
            noActivatingCombinationText.SetActive(collectionPracticeSkillDataList.Count == 0);
            return collectionPracticeSkillDataList.Select(collectionPracticeSkill => new PracticeSkillViewMiniGridItem.Info(collectionPracticeSkill, false, false)).ToList();
        }
    }
}