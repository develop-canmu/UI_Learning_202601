using System.Threading;
using CruFramework.Page;
using Cysharp.Threading.Tasks;

namespace Pjfb.LeagueMatchTournament
{
    public enum LeagueMatchTournamentPageType
    {
        // 大会リストページ
        List,
        // 大会エントリーページ
        Entry,
    }
    
    public class LeagueMatchTournamentPage : PageManager<LeagueMatchTournamentPageType>
    {
        protected override string GetAddress(LeagueMatchTournamentPageType type)
        {
            return $"Prefabs/UI/Page/LeagueMatchTournament/LeagueMatchTournament{type}Page.prefab";
        }

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            // 別ページから戻ってきたなら前に開いていたページを再度開く
            if (TransitionType == PageTransitionType.Back && CurrentPageObject != null)
            {
                await OpenPageAsync(CurrentPageType, true, CurrentPageObject.OpenArguments, token);
                return;
            }
            
            await OpenPageAsync(LeagueMatchTournamentPageType.List, true, args, token);
        }
    }
}