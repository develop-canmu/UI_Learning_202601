using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;

namespace Pjfb.Combination
{
    public class CombinationCollectionActivatableNotificationModal : ModalWindow
    {
        public class Data
        {
            public List<CombinationManager.CombinationCollection> CombinationCollectionList;
            public List<CombinationManager.CollectionProgressData> CollectionProgressDataList;

            public Data(List<CombinationManager.CombinationCollection> combinationCollectionList, List<CombinationManager.CollectionProgressData> collectionProgressDataList)
            {
                CombinationCollectionList = combinationCollectionList;
                CollectionProgressDataList = collectionProgressDataList;
            }
        }
        
        [SerializeField] private CombinationCollectionScrollDynamic combinationCollectionScrollDynamic;

        private Data modalData;
        
        public static void Open(Data data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.CombinationCollectionActivatableNotification, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalData = (Data)args;
            Init();
            return base.OnPreOpen(args, token);
        }

        private void Init()
        {
            combinationCollectionScrollDynamic.InitializeCanActivateByChara(modalData.CombinationCollectionList, modalData.CollectionProgressDataList);
        }

        public void OnClickMoveCombinationCollectionPage()
        {
            Close();
            CharacterPage m = (CharacterPage)AppManager.Instance.UIManager.PageManager.CurrentPageObject;
            m.OpenPage(CharacterPageType.CombinationCollection, true, null);
        }
    }
}