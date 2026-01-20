using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Networking.App.Request;
using Pjfb.Training;
using UnityEngine.UI;

namespace Pjfb.Training
{
    public class TrainingTargetView : MonoBehaviour
    {
    
        [SerializeField]
        private TMPro.TMP_Text descriptionText = null;
        
        [SerializeField]
        private Image clearCover = null;        
        [SerializeField]
        private Image clearIcon = null;
        [SerializeField]
        private Image nextIcon = null;
        [SerializeField]
        private Image notAchieved = null;

        /// <summary>目標の設定</summary>
        public void SetTarget(TrainingGoal target)
        {
            // 説明
            descriptionText.text = target.goalDescription;
            // 達成済み
            bool isCompleted = target.state == (int)TrainingUtility.TargetState.Completed;
            bool isNext = target.state == (int)TrainingUtility.TargetState.CurrentTarget;
            clearCover.gameObject.SetActive(isCompleted);
            clearIcon.gameObject.SetActive(isCompleted);
            // 未達成
            notAchieved.gameObject.SetActive( isNext == false && isCompleted == false );
            // 現在の目標
            nextIcon.gameObject.SetActive(isNext);
        }
    }
}