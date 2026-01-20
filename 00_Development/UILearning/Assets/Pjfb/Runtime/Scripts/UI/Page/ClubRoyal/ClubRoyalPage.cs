using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb.ClubRoyal
{
    public enum ClubRoyalPageType
    {
        // Topページ
        ClubRoyalTop,
    }
    
    public class ClubRoyalPage : PageManager<ClubRoyalPageType>
    {
        protected override string GetAddress(ClubRoyalPageType page)
        {
            return $"Prefabs/UI/Page/ClubRoyal/{page}Page.prefab";
        }

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            await OpenPageAsync(ClubRoyalPageType.ClubRoyalTop, true, args, token);
        }
    }
}