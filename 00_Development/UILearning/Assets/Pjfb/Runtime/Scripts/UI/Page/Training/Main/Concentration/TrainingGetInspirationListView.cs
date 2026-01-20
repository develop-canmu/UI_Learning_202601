using System.Linq;
using CruFramework.UI;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.Training
{
    /// <summary>
    /// インスピレーション一覧表示用View
    /// </summary>
    public class TrainingGetInspirationListView : MonoBehaviour
    {
        [SerializeField]
        private ScrollGrid inspirationScrollGrid = null;
        
        /// <summary>
        /// インスピレーションリストを初期化して表示
        /// </summary>
        public void Initialize(TrainingPending pending)
        {
            // インスピレーションリスト
            TrainingInspire[] orderdList = pending.inspireList.
                // グレード順
                OrderByDescending(v=>
                {
                    TrainingCardInspireMasterObject mCard = MasterManager.Instance.trainingCardInspireMaster.FindData(v.id);
                    return mCard.grade;
                }).
                // 優先度順
                ThenByDescending(v=>
                {
                    TrainingCardInspireMasterObject mCard = MasterManager.Instance.trainingCardInspireMaster.FindData(v.id);
                    return mCard.priority;
                }).
                // Id順
                ThenBy(v=>
                {
                    return v.id;
                }).
                ToArray();
            
            
            // スクロール用引数
            TrainingGetInspirationScrollItem.Argument[] itemArgs = new TrainingGetInspirationScrollItem.Argument[orderdList.Length];
            for(int i=0;i<itemArgs.Length;i++)
            {
                itemArgs[i] = new TrainingGetInspirationScrollItem.Argument(orderdList[i].id, false);
            }            
            // スクロールにセット
            inspirationScrollGrid.SetItems(itemArgs);
        }
    }
}

