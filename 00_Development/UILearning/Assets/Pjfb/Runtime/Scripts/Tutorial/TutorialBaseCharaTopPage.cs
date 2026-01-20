using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Character;

namespace Pjfb
{
    public class TutorialBaseCharaTopPage : BaseCharaTopPage
    {
        protected override void OnOpened(object args)
        {
            AppManager.Instance.TutorialManager.ExecuteTutorialAction().Forget();
            base.OnOpened(args);
        }
    }
}