using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;


namespace Pjfb
{
    [CreateAssetMenu]
    public class TutorialCommandSetting : ScriptableObject
    {
        // 各コマンドの抽象クラス
        public abstract class TutorialCommandData
        {
            protected enum FocusObjectType 
            {
                Character = 0,
                Default = 9999
            }
            public abstract UniTask ActionCommand(CancellationToken token); 
            
            private const int MaxSearchCount = 60 * 15;
            private const int DelayFrame = 1;
            
            // 指定IDのTutorialFocusTargetを探す
            protected async UniTask<TutorialCommandTarget> FindTarget(string findId,CancellationToken token)
            {
                int cnt = 0;
                while (cnt < MaxSearchCount)
                {
                    TutorialCommandTarget[] targets = FindObjectsByType<TutorialCommandTarget>(FindObjectsSortMode.None);
                    foreach (TutorialCommandTarget target in targets)
                    {
                        if (target.FocusId != findId) continue;
                        return target;
                    }
                    cnt++;
                    await UniTask.DelayFrame(DelayFrame, cancellationToken:token);
                }
                return null;
            }
        }
        
        [Serializable]
        public class ActionSetting
        {
            [SerializeReference] private TutorialCommandData command;

            public TutorialCommandData Command
            {
                get {return command;}
                set {command = value;}
            }
        }
        
        // アクションセッティングリスト
        [SerializeField] private ActionSetting[] actionSettingList;
        public ActionSetting[] ActionSettingList => actionSettingList;
    }
}