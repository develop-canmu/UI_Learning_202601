using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;
using Pjfb.Master;
using Pjfb.UserData;

namespace Pjfb.Combination
{
    public class CombinationCollectionSkillSetDetailModal : ModalWindow
    {
        public class Data
        {
            public CombinationManager.CombinationCollection CombinationCollection;
            public Action<long> OnProgressCombinationCollection;

            public Data(CombinationManager.CombinationCollection combinationCollection, Action<long> onProgressCombinationCollection)
            {
                CombinationCollection = combinationCollection;
                OnProgressCombinationCollection = onProgressCombinationCollection;
            }
        }
        
        [SerializeField] private CombinationCollectionView combinationCollectionView;
        [SerializeField] private CombinationCollectionScrollDynamic combinationCollectionScrollDynamic;

        private Data modalData;
        private Dictionary<long, UserDataChara> mCharaIdPossessionDictionary;
        
        public static void Open(Data data)
        {
            AppManager.Instance.UIManager.ModalManager.OpenModal(ModalType.CombinationCollectionSkillSetDetail, data);
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            modalData = (Data)args;
            combinationCollectionScrollDynamic.InitializeSelect(
                modalData.CombinationCollection,
                modalData.OnProgressCombinationCollection
            );
            return base.OnPreOpen(args, token);
        }
    }
}
