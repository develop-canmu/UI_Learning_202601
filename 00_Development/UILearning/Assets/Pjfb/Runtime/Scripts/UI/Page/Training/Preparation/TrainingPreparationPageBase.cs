using System.Collections;
using System.Collections.Generic;
using Pjfb.Networking.App.Request;
using UnityEngine;

namespace Pjfb.Training
{
    
    public abstract class TrainingPreparationPageBase : Page
    {
        /// <summary>ページマネージャー</summary>
        protected TrainingPreparation TrainingPreparationManager{get{return (TrainingPreparation)Manager;}}
        
        private TrainingPreparationArgs arguments = null;
        /// <summary>準備画面の引数</summary>
        protected TrainingPreparationArgs Arguments{get{return (TrainingPreparationArgs)OpenArguments;}}
        
        /// <summary>自動トレーニングデータ</summary>
        public TrainingAutoUserStatus AutoTrainingUserStatus{get{return ((TrainingPreparation)Manager).AutoTrainingStatus?.userStatus; }}
        /// <summary>自動トレーニングデータ</summary>
        public TrainingAutoPendingStatus[] AutoTrainingPendingStatus{get{return ((TrainingPreparation)Manager).AutoTrainingStatus?.pendingStatusList; }}
        
        /// <summary>選択中のスロット</summary>
        public TrainingAutoPendingStatus CurrentSlotAutoTrainingPendingStatus
        {
            get
            {
                foreach(TrainingAutoPendingStatus status in AutoTrainingPendingStatus)
                {
                    if(status.slotNumber == arguments.AutoTrainingSlot)return status;
                }
                return null;
            }
        } 
        
        public virtual void OnBackPage()
        {
        }
        
        protected override void OnEnablePage(object args)
        { 
            arguments = (TrainingPreparationArgs)args;
            base.OnEnablePage(args);
        }
        
        
        public void OpenHelpModal()
        {
            TrainingUtility.OpenHelpModal();
        }
    }
}