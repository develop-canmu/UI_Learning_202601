using System.Threading;
using Cysharp.Threading.Tasks;
using Pjfb.Rivalry;

namespace Pjfb.MatchResult
{
    public enum MatchResultPageType
    {
        MatchResultWin,
        MatchResultLose,
        MatchResultWinColosseum,
        MatchResultWinClubMatch,
    }
    
    public class MatchResultPage : PageManager<MatchResultPageType>
    {
        public class Data
        {
            public MatchResultPageType pageType;
            public bool showHeaderAndFooter = true;
            public object args;
        }
        
        protected Data data;

        protected override string GetAddress(MatchResultPageType page)
        {
            return $"Prefabs/UI/Page/MatchResult/{page}Page.prefab";
        }

        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            ClearPageStack();
            data = (Data) args;
            RivalryManager.ResetCache();
            if (data == null) data = new Data();
            return OpenPageAsync(data.pageType, true, data.args, token);
        }
        
        protected override void OnEnablePage(object args)
        {
            if (!data.showHeaderAndFooter)
            {
                AppManager.Instance.UIManager.Header.Hide();
                AppManager.Instance.UIManager.Footer.Hide();
            }
            base.OnEnablePage(args);
        }
    }
}


