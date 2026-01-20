using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Networking.App.Request;

namespace Pjfb.Training
{
    public class AutoTrainingSummaryTrainingLvView : MonoBehaviour
    {
        [SerializeField]
        private AutoTrainingSummaryTrainingLvItemView itemView = null;
        
        // 生成したView
        private List<AutoTrainingSummaryTrainingLvItemView> createdViews = new List<AutoTrainingSummaryTrainingLvItemView>();
        
        public void SetData(TrainingMainArguments mainArguments)
        {
            
            // 生成済みのViewを削除
            foreach(AutoTrainingSummaryTrainingLvItemView view in createdViews)
            {
                GameObject.Destroy(view.gameObject);
            }
            createdViews.Clear();
            
            // 各カード
            foreach(TrainingPracticeProgress card in mainArguments.Pending.practiceProgressList)
            {
                AutoTrainingSummaryTrainingLvItemView view = GameObject.Instantiate<AutoTrainingSummaryTrainingLvItemView>(itemView, transform);
                // 名前
                view.SetName( StringValueAssetLoader.Instance[$"practice_name{(int)card.practiceType}"] + StringValueAssetLoader.Instance["auto_training.summary.card_lv"] );
                // Lv
                view.SetValue( card.level );
                // アクティブ
                view.gameObject.SetActive(true);
                
                createdViews.Add(view);
            }
        }
    }
}