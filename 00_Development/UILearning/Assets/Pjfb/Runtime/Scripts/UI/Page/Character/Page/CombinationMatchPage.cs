using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Combination;
using Pjfb.Master;
using Pjfb.UserData;
using UniRx;

namespace Pjfb.Character
{
    public class CombinationMatchPage : Page
    {
        [SerializeField] private CombinationMatchScrollDynamic scrollDynamic;
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            scrollDynamic.InitializeAll();
            CombinationManager.SaveConfirmedCombinationMatchId();
            AppManager.Instance.UIManager.Footer.CharacterButton.SetNotificationBadge(BadgeUtility.IsCharacterBadge);
            return base.OnPreOpen(args, token);
        }
    }
}