using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Pjfb.Adv;
using CruFramework;
using Logger = CruFramework.Logger;

namespace Pjfb
{
    
    public class TutorialAdvTopPage : Page
    {
        
        [SerializeField]
        private AppAdvManager advManager = null;

        private TutorialAdvPage.SceneParam nextParam;

        #region OverrideMethods
        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            nextParam = args as TutorialAdvPage.SceneParam;
            await base.OnPreOpen(args, token);
        }
        
        protected override async UniTask OnMessage(object value)
        {
            if(value is PageManager.MessageType type)
            {
                switch (type)
                {
                    case PageManager.MessageType.BeginFade:
                        AppManager.Instance.UIManager.Footer.Hide();
                        AppManager.Instance.UIManager.Header.Hide();
                        // TODO PageのBeginFadeでデフォルトのBGM指定が行われてしまうためシナリオ読み込みを移動
                        await advManager.LoadAdvFile(ResourcePathManager.GetPath(AppAdvManager.ResourcePathKey, nextParam.scenarioId));
                        break;
                }
            }
            await base.OnMessage(value);
        }

        #endregion
        
        private void OnAdvEnd()
        {
            EndAsync().Forget();
        }
        
        
        private async UniTask EndAsync()
        {
            if (nextParam.nextPageType != PageType.TutorialDefault)
            {
                await AppManager.Instance.TutorialManager.OnClosedTutorialScenario(nextParam.nextPageType, nextParam.stack, nextParam.args);
            }
            else
            {
                var id = AppManager.Instance.TutorialManager.GetNextScenarioId();
                if (id > 0)
                {
                    await advManager.LoadAdvFile(ResourcePathManager.GetPath(AppAdvManager.ResourcePathKey, id));
                }
            }
        }
        
        private void Awake()
        {
            advManager.OnEnded += OnAdvEnd;
        }

        protected override void OnDestroy()
        {
            advManager.OnEnded -= OnAdvEnd;
            base.OnDestroy();
        }
        
    }
}