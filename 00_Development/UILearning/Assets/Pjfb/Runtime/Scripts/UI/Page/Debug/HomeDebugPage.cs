#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT && !PJFB_REL
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    
    public enum HomeDebugPageType
    {
        Result,
    }
    
    public class HomeDebugPage : PageManager<HomeDebugPageType>
    {
        
        public class Arguments
        {
            private HomeDebugPageType pageType;
            public HomeDebugPageType PageType => pageType;
            public Arguments(HomeDebugPageType pageType)
            {
                this.pageType = pageType;
            }
        }
        
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            Arguments arg = (Arguments)args;
            return OpenPageAsync(arg.PageType, true, null, token);
        }
        
        
        protected override string GetAddress(HomeDebugPageType page)
        {
            switch(page)
            {
                case HomeDebugPageType.Result :
                    return $"Prefabs/UI/Page/HomeDebug/DebugHome{page}Page.prefab";
                default: throw new Exception("PageTypeが定義されてません。");
            }
        }

    }
}
#endif