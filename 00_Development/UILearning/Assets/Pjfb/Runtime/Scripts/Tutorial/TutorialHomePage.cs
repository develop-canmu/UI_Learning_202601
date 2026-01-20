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
    public enum TutorialHomePageType
    {
        HomeTop,
    }
    
    public class TutorialHomePage : PageManager<TutorialHomePageType>
    {
        protected override string GetAddress(TutorialHomePageType page)
        {
            return $"Prefabs/UI/Page/TutorialHome/{page}Page.prefab";
        }
        
        /// <summary>事前準備</summary>
        protected async override UniTask OnPreOpen(object args, CancellationToken token)
        {
            AppManager.Instance.TutorialManager.AddDebugCommand(PageType.TutorialHome);
            await OpenPageAsync(TutorialHomePageType.HomeTop, true, args, token);
        }

        protected override void OnClosed()
        {
            AppManager.Instance.TutorialManager.RemoveDebugCommand(PageType.TutorialHome);
            base.OnClosed();
        }
     
    }
}