using System;
using System.Linq;
using System.Threading;
using CruFramework;
using Cysharp.Threading.Tasks;
using Pjfb.Gacha;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Pjfb.Training;

namespace Pjfb
{
    public enum TutorialAdvPageType
    {
        AdvTop,
    }
    
    public class TutorialAdvPage : PageManager<TutorialAdvPageType>
    {
        
        public class SceneParam
        {
            public int scenarioId;
            public PageType nextPageType;
            public object args;
            public bool stack;
        }
        
        protected override string GetAddress(TutorialAdvPageType page)
        {
            return $"Prefabs/UI/Page/TutorialAdv/{page}Page.prefab";
        }
        
        /// <summary>事前準備</summary>
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            var isParam = args is SceneParam;
            if (!isParam)
            {
                var param = new SceneParam();
                param.nextPageType = PageType.TutorialDefault;
                param.scenarioId = AppManager.Instance.TutorialManager.GetNextScenarioId();
                args = param;
            }

            AppManager.Instance.TutorialManager.AddDebugCommand(PageType.TutorialAdv);
            return OpenPageAsync(TutorialAdvPageType.AdvTop, true, args, token);
        }

        protected override void OnClosed()
        {
            AppManager.Instance.TutorialManager.RemoveDebugCommand(PageType.TutorialAdv);
            base.OnClosed();
        }
     
    }
}