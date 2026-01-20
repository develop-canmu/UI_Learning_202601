using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pjfb.Training
{
    public class TrainingExtraTurnLotteryPage : TrainingPageBase
    {
        private const string LogAddExtraTurnFirstKey = "training.log.turn_add_first";
        private const string LogAddExtraTurnFirstSpeakerKey = "training.log.turn_add_first_speaker";
        private const string LogAddContinueExtraTurnKey = "training.log.turn_add_continue";
        private const string LogAddContinueExtraSpeakerTurnKey = "training.log.turn_add_continue_speaker";

        [SerializeField] private LotteryExtraTurnView lotteryExtraTurnView;

        protected override async UniTask OnPreOpen(object args, CancellationToken token)
        {
            Adv.Footer.EnableAutoButton(false);
            AppManager.Instance.UIManager.System.TouchGuard.Show();

            await base.OnPreOpen(args, token);
        }

        protected override void OnEnablePage(object args)
        {
            base.OnEnablePage(args);

            if (MainArguments.IsStartFirstExtraTurnThisTurn())
            {
                PlayLotteryExtraTurnEffectAsync(MainArguments.CurrentTarget.firstAddedTurn, MainArguments.Pending.turnAddEffectType, IsFastMode).Forget();
                return;
            }

            if (MainArguments.IsStartContinueExtraTurnThisTurn())
            {
                PlayContinueExtraTurnEffectAsync().Forget();
                return;
            }

            OpenNextPage();
        }

        private async UniTask PlayLotteryExtraTurnEffectAsync(long extraTurn, long effectType, bool isFastMode)
        {
            Header.SetActiveAllPage();
            Header.UpdateCondition(MainArguments.Pending);
            
            // ターン数が0になる演出
            Header.PlayHasExtraTurnRightEffect(0, MainArguments.CurrentTargetTurn, effectType);
            await UniTask.Delay(TimeSpan.FromSeconds(TrainingTurnView.GaugeAnimationDuration));

            // ヘッダーUI側の演出
            MainPageManager.Character.gameObject.SetActive(true);
            await Header.PlayStartLotteryExtraTurnEffectAsync();

            Adv.AddMessageLog(
                StringValueAssetLoader.Instance[LogAddExtraTurnFirstSpeakerKey],
                string.Format(StringValueAssetLoader.Instance[LogAddExtraTurnFirstKey], extraTurn),
                0
            );

            // 画面全体をおおう演出
            AppManager.Instance.UIManager.System.TouchGuard.Hide();
            await lotteryExtraTurnView.PlayEffectAsync(extraTurn, isFastMode);

            // ヘッダーUI側の演出
            await Header.PlayEnterExtraTurnEffectAsync(extraTurn, effectType);

            MainArguments.ArgumentsKeeps.IsShownFirstExtraTurnEffect = true;
            MainArguments.ArgumentsKeeps.LatestShowFirstExtraTurnEffectGoalIndex = MainArguments.Pending.nextGoalIndex;
            OpenNextPage();
        }

        private async UniTask PlayContinueExtraTurnEffectAsync()
        {
            Header.SetActiveAllPage();
            Header.UpdateCondition(MainArguments.Pending);

            // 延長ターン数が0になる演出
            Header.PlayFirstExtraTurnEffect(0, MainArguments.CurrentTarget.firstAddedTurn, MainArguments.CurrentTarget.addedTurn, MainArguments.Pending.turnAddEffectType);
            await UniTask.Delay(TimeSpan.FromSeconds(TrainingTurnView.GaugeAnimationDuration));

            // ヘッダーUI側の演出
            MainPageManager.Character.gameObject.SetActive(true);
            await Header.PlayContinueExtraTurnEffectAsync();

            Adv.AddMessageLog(StringValueAssetLoader.Instance[LogAddContinueExtraSpeakerTurnKey], StringValueAssetLoader.Instance[LogAddContinueExtraTurnKey], 0);

            MainArguments.ArgumentsKeeps.IsShownContinueExtraTurnEffect = true;
            MainArguments.ArgumentsKeeps.LatestShowContinueExtraTurnEffectGoalIndex = MainArguments.Pending.nextGoalIndex;
            OpenNextPage();
        }

        private void OpenNextPage()
        {
            Adv.Footer.EnableAutoButton(true);
            AppManager.Instance.UIManager.System.TouchGuard.Hide();

            OpenPage(TrainingMainPageType.Top, MainArguments);
        }

        /// <summary>
        /// UGUI
        /// </summary>
        public void OnNextButton()
        {
            lotteryExtraTurnView.SkipEffectForget(MainArguments.CurrentTarget.firstAddedTurn);
        }
    }
}
