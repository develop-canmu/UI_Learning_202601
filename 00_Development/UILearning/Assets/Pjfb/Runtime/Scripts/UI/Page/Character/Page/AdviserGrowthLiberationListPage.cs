using System.Linq;
using System.Threading;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.UserData;

namespace Pjfb.Character
{
    /// <summary> アドバイザー強化・解放ページ </summary>
    public class AdviserGrowthLiberationListPage : BaseCharaGrowthLiberationListPage
    {
        protected override CharacterPageType selectedPageType => CharacterPageType.AdviserGrowthLiberation;
    }
}