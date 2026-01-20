using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.Training;

namespace Pjfb.Training
{
    public class TrainingGetBoostListModal : ModalWindow
    {
        public class Data
        {
            public List<TrainingGetBoostListScrollItem.TrainingGetBoostListScrollData> SortedScrollData { get; }

            public Data(List<TrainingGetBoostListScrollItem.TrainingGetBoostListScrollData> sortedScrollData)
            {
                SortedScrollData = sortedScrollData;
            }
        }
        
        [SerializeField]
        private ScrollGrid scrollGrid = null;

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            Data arg = (Data)args;
            
            scrollGrid.SetItems(arg.SortedScrollData);
            
            return base.OnPreOpen(args, token);
        }
    }
}