using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Networking.App.Request;
using TMPro;

namespace Pjfb.Training
{
    public class AutoTrainingSummaryTurnView : MonoBehaviour
    {

        [SerializeField]
        private AutoTrainingSummaryTurnItemView itemView = null;
        
        [SerializeField]
        private TMP_Text headerLabelText = null;
        
        // 生成したView
        private List<AutoTrainingSummaryTurnItemView> createdViews = new List<AutoTrainingSummaryTurnItemView>();
        
        // 結果表示
        public void SetData(TrainingMainArguments mainArguments, ResultTurn[] result)
        {
            
            // 生成済みのViewを削除
            foreach(AutoTrainingSummaryTurnItemView view in createdViews)
            {
                GameObject.Destroy(view.gameObject);
            }
            createdViews.Clear();
            
            // 合計ターン
            long totalExtraTuen = 0;
            long turnCount = 0;
            
            int index = 1;
            foreach(ResultTurn r in result)
            {
                // Viewの生成
                AutoTrainingSummaryTurnItemView view = GameObject.Instantiate<AutoTrainingSummaryTurnItemView>(itemView, transform);
                // 表示
                view.SetData(index++, r);
                // アクティブ
                view.gameObject.SetActive(true);
                // リストに追加
                createdViews.Add(view);
                
                // 合計ターン
                turnCount += r.turnMax;
                totalExtraTuen += r.turnAddValue;
            }
            
            // 追加ターンの有無で表示切り替え
            if(totalExtraTuen > 0)
            {
                headerLabelText.text = StringValueAssetLoader.Instance["auto_training.summary_modal.turn.label2"];
            }
            else
            {
                headerLabelText.text = StringValueAssetLoader.Instance["auto_training.summary_modal.turn.label1"];
            }
            
            // 合計ターン表示よう
            AutoTrainingSummaryTurnItemView totalView = GameObject.Instantiate<AutoTrainingSummaryTurnItemView>(itemView, transform);
            totalView.SetData(StringValueAssetLoader.Instance["auto_training.summary.total_turn"], turnCount, totalExtraTuen);
            // アクティブ
            totalView.gameObject.SetActive(true);
            // リストに追加
            createdViews.Add(totalView);
        }
    }
}