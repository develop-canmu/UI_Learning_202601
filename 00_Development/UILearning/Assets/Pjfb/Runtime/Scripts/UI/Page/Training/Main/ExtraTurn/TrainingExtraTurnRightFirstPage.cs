using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pjfb.Training
{
    /// <summary>
    /// トレーニング開始直後、練習前の権利付与
    /// </summary>
    public class TrainingExtraTurnRightFirstPage : TrainingPageBase
    {
        [SerializeField] private ExtraTurnRightView extraTurnRightView;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            Adv.Footer.EnableAutoButton(false);

            await base.OnPreOpen(args, token);
        }

        protected override void OnEnablePage(object args)
        {
            base.OnEnablePage(args);

            if (MainArguments.IsFirstActionTurnCurrent() && MainArguments.HasExtraTurnCurrentTarget())
            {
                PlayEffectAsync().Forget();
                return;
            }
            
            OpenNextPage();
        }

        private async UniTask PlayEffectAsync()
        {
            Header.SetActiveAllPage();
            Header.UpdateCondition(MainArguments.Pending);

            MainPageManager.Character.gameObject.SetActive(true);

            await extraTurnRightView.PlayEffectAsync(new ExtraTurnRightView.PlayArguments(Header, Adv, MainArguments, IsFastMode), () => { });

            OpenNextPage();
        }

        private void OpenNextPage()
        {
            Adv.Footer.EnableAutoButton(true);

            MainArguments.ArgumentsKeeps.IsShownGetExtraTurnRightFirstEffect = true;
            MainArguments.ArgumentsKeeps.LatestShowGetExtraTurnRightFirstEffectGoalIndex = MainArguments.Pending.nextGoalIndex;
            OpenPage(TrainingMainPageType.Top, MainArguments);
        }

        /// <summary>
        /// UGUI
        /// </summary>
        public void OnNextButton()
        {
            if (extraTurnRightView.IsPlayingEffect)
            {
                extraTurnRightView.SkipEffectForget(new ExtraTurnRightView.PlayArguments(Header, Adv, MainArguments, IsFastMode));
            }
        }
    }
}
