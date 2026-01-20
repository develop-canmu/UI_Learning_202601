
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Spine.Unity;
using CruFramework.Adv;
using Cysharp.Threading.Tasks;

namespace Pjfb.Adv
{
    [System.Serializable]
    public class AdvCommandOpenTrainingGoal : IAdvCommand, IAdvCommandSkipBreak
    {
        [SerializeField]
        [AdvDocument("メッセージウィンドウを非表示にする。")]
        private  bool hideMessageWindow = true;
        
        bool IAdvCommandSkipBreak.IsSkipBreak(AdvManager manager)
        {
            return true;
        }

        void IAdvCommand.Execute(AdvManager manager)
        {
            
#if CRUFRAMEWORK_DEBUG || UNITY_EDITOR
            if(manager.IsDebugMode)
            {
                manager.OpenDebugModal(this, "トレーニング目標表示演出");
                return;
            }
#endif
            
            TrainingAdvManagerExtension extension = manager.GetComponent<TrainingAdvManagerExtension>();
            if(extension == null)return;

            // メッセージウィンドウを非表示
            if(hideMessageWindow)
            {
                manager.HideAllMessageWindows();
            }

            AppAdvManager m = (AppAdvManager)manager;
            // コマンドを停止
            manager.IsStopCommand = true;
            extension.TrainingMainPage.OpenTargetView(m.AppAutoMode == AppAdvAutoMode.Skip4 , ()=>manager.IsStopCommand = false);
        }
        
    }
}
