using System.Linq;
using System.Threading;
using CruFramework.Page;
using Cysharp.Threading.Tasks;
using Pjfb.Master;
using Pjfb.UserData;
using UnityEngine;

namespace Pjfb.Character
{
    public class SpecialSupportCardListPage : BaseCharaGrowthLiberationListPage
    {
        protected override CharacterPageType selectedPageType => CharacterPageType.SpecialSupportCardLevelUp;
    }
}
