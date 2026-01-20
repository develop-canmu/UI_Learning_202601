using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.App.Request;
using System.Linq;
using Pjfb.Master;

namespace Pjfb.Training
{
    public class AutoTrainingInspirationListModal : ModalWindow
    {
        [SerializeField]
        private ScrollGrid getListScroll = null;
        [SerializeField]
        private ScrollGrid consumListScroll = null;


        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            
            TrainingAutoResultStatus result = (TrainingAutoResultStatus)args;
            
            List<TrainingGetInspirationScrollItem.Argument> getList = new List<TrainingGetInspirationScrollItem.Argument>();
            
            // ソート
            
            // 獲得インスピ
            foreach(ResultIdCount r in SortList(result.inspireList))
            {
                getList.Add(new TrainingGetInspirationScrollItem.Argument(r.id, false, r.count));
            }
            // スクロールに登録
            getListScroll.SetItems(getList);
            
            List<TrainingGetInspirationScrollItem.Argument> consumList = new List<TrainingGetInspirationScrollItem.Argument>();
            // 消費インスピ
            foreach(ResultIdCount r in SortList(result.inspireExecuteList))
            {
                consumList.Add(new TrainingGetInspirationScrollItem.Argument(r.id, false, r.count));
            }
            // スクロールに登録
            consumListScroll.SetItems(consumList);
            
            
            
            return base.OnPreOpen(args, token);
        }
        
        private IEnumerable<ResultIdCount> SortList(ResultIdCount[] list)
        {
            return list.
                OrderByDescending(v=> MasterManager.Instance.trainingCardInspireMaster.FindData(v.id).grade).
                ThenByDescending(v=>v.count).
                ThenByDescending(v=>v.id);
        }
    }
}