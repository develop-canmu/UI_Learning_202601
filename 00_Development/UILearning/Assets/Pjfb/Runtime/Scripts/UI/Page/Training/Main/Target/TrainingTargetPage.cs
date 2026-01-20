using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.App.Request;
using Pjfb.Training;

namespace Pjfb.Training
{
    public class TrainingTargetPage : TrainingPageBase
    {
        /// <summary>
        /// UGUI
        /// </summary>
        private void OnNextButton()
        {
            // 目標ページ表示のオプションを外す
            TrainingMainArguments.Options options = MainArguments.OptionFlags ^ TrainingMainArguments.Options.OpenTargetPage;
            OpenPage(TrainingMainPageType.Top, new TrainingMainArguments(MainArguments, MainArguments.ArgumentsKeeps, options));
        }
    
        protected override UniTask OnPreOpen(object args, CancellationToken token)
        {
            // ヘッダーを非表示
            MainPageManager.Header.Hide();
            // キャラを非表示
            MainPageManager.Character.gameObject.SetActive(false);

            return base.OnPreOpen(args, token);
        }

        protected override void OnEnablePage(object args)
        {
            MainPageManager.TargetPageView.Open(MainArguments.CurrentTarget, IsTargetEffectSkip, OnNextButton);
            base.OnEnablePage(args);
        }

    }
}
