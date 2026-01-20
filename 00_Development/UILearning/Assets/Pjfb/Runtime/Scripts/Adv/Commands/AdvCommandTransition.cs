
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using Spine.Unity;
using CruFramework.Adv;
using Cysharp.Threading.Tasks;
using Pjfb.Training;

namespace Pjfb.Adv
{
    [System.Serializable]
    public class AdvCommandTransition : IAdvCommand
    {
        public enum TransitionType
        {
            In, Out, PassingOfTime
        }
    
        [SerializeField]
        [AdvDocument("フェード種類。")]
        private TransitionType type = TransitionType.In;
        
        [SerializeField]
        [AdvDocument("フェードが終わるまで待機。")]
        private bool isWait = true;

        void IAdvCommand.Execute(AdvManager manager)
        {
            ExecuteAsync((AppAdvManager)manager).Forget();
        }
        
        private async UniTask ExecuteAsync(AppAdvManager manager)
        {
            // スキップ時は無視
            if(manager.IsSkip)return;
            
            UniTask fadeTask = default;
            
            switch(type)
            {
                case TransitionType.Out:
                    fadeTask = manager.Transition.FadeOut();
                    break;
                case TransitionType.In:
                    fadeTask = manager.Transition.FadeIn();
                    break;
                case TransitionType.PassingOfTime:
                    fadeTask = manager.Transition.PlayPassingOfTime();
                    break;
            }
            
            if(isWait)
            {
                manager.IsStopCommand = true;
                await fadeTask;
                manager.IsStopCommand = false;
            }
        }
    }
}
