#if (CRUFRAMEWORK_DEBUG && !PJFB_REL) || UNITY_EDITOR
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Pjfb.DebugPage
{
    public enum DebugPageType
    {
        CutScene,
        Training3D,
    }
    
    public class DebugPage : PageManager<DebugPageType>
    {
        public class Arguments
        {
            public DebugPageType PageType { get; }
            public Arguments(DebugPageType pageType)
            {
                this.PageType = pageType;
            }
        }
        
        protected override string GetAddress(DebugPageType page)
        {
            switch(page)
            {
                case DebugPageType.CutScene : 
                case DebugPageType.Training3D :
                    // デバッグパスに配置している場合
                    return $"Prefabs/UI/Page/Debug/Debug{page}Page.prefab";
                default: throw new Exception("PageTypeが定義されてません。");
            }
        }

        protected override void OnEnablePage(object args)
        {
            // ヘッダーとフッタを非表示に
            AppManager.Instance.UIManager.Footer.Hide();
            AppManager.Instance.UIManager.Header.Hide();

            base.OnEnablePage(args);
        }

 

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            Arguments arg = (Arguments)args;
            return OpenPageAsync(arg.PageType, true, null, token);
        }

        protected override void OnClosed()
        {
            // ヘッダーとフッタを表示に
            AppManager.Instance.UIManager.Footer.Show();
            AppManager.Instance.UIManager.Header.Show();
            base.OnClosed();
        }
        
        public void Back()
        {
            AppManager.Instance.BackToTitle();
        }
        
    }
}
#endif