using System;
using CruFramework.Adv;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pjfb.Training
{
    public class ExtraTurnRightView : MonoBehaviour
    {
        public class PlayArguments
        {
            public TrainingHeader Header { get; }
            public AdvManager Adv { get; }
            public string ActionName { get; }
            public long EffectType { get; }
            public long RewardTurnAddValue { get; }
            public long AddedTurn { get; }
            public long FirstAddedTurn { get; }
            public bool IsFastMode { get; }

            public PlayArguments(TrainingHeader header, AdvManager adv, TrainingMainArguments mainArguments, bool isFastMode)
            {
                Header = header;
                Adv = adv;
                EffectType = mainArguments.Pending.turnAddEffectType;
                ActionName = mainArguments.ActionName;
                RewardTurnAddValue = mainArguments.Reward?.turnAddValue ?? 0;
                AddedTurn = mainArguments.CurrentTarget.addedTurn;
                FirstAddedTurn = mainArguments.CurrentTarget.firstAddedTurn;
                IsFastMode = isFastMode;
            }
        }

        private enum EffectPhase
        {
            Closed,
            Open,
            MoveIcon,
            Close
        }

        public bool IsPlayingEffect => currentPhase != EffectPhase.Closed;

        private const string LogTurnAddKey = "training.log.turn_add_right";

        [SerializeField] private AcquisitionOfLotteryRightsAnimation acquisitionOfLotteryRightsAnimation;

        private EffectPhase currentPhase;

        public async UniTask PlayEffectAsync(PlayArguments playArguments, Action onComplete)
        {
            currentPhase = EffectPhase.Open;
            PlayPhaseEffectAsync(playArguments, currentPhase).Forget();
            await UniTask.WaitUntil(() => currentPhase == EffectPhase.Closed);
            onComplete();
        }

        public void SkipEffectForget(PlayArguments playArguments)
        {
            switch (currentPhase)
            {
                case EffectPhase.Closed:
                case EffectPhase.Close:
                case EffectPhase.MoveIcon:
                    return;
                case EffectPhase.Open:
                    currentPhase++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            PlayPhaseEffectAsync(playArguments, currentPhase).Forget();
        }

        private UniTask PlayNextPhaseEffectAsync(PlayArguments playArguments)
        {
            currentPhase++;
            return PlayPhaseEffectAsync(playArguments, currentPhase);
        }

        private async UniTask PlayPhaseEffectAsync(PlayArguments playArguments, EffectPhase phase)
        {
            switch (phase)
            {
                case EffectPhase.Closed:
                    break;
                case EffectPhase.Open:
                    await PlayOpenEffectAsync(playArguments);
                    break;
                case EffectPhase.MoveIcon:
                    await PlayMoveIconEffectAsync(playArguments);
                    break;
                case EffectPhase.Close:
                    await PlayCloseEffectAsync();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async UniTask PlayOpenEffectAsync(PlayArguments playArguments)
        {
            await acquisitionOfLotteryRightsAnimation.PlayOpenEffectAsync(playArguments.EffectType, () => { });

            // Advのログに入れる
            playArguments.Adv.AddMessageLog(playArguments.ActionName, StringValueAssetLoader.Instance[LogTurnAddKey], 0);

#if UNITY_EDITOR
            playArguments.Adv.AddMessageLog(
                $"<color=#f00>[デバッグ]</color> {playArguments.ActionName}",
                $"本ターンで獲得した延長数: {playArguments.RewardTurnAddValue}<br>" +
                $"初回延長数（暫定）: {playArguments.FirstAddedTurn}<br>" +
                $"全体延長数（暫定）: {playArguments.AddedTurn}",
                0
            );
#endif

            if (playArguments.IsFastMode && currentPhase == EffectPhase.Open)
            {
                PlayNextPhaseEffectAsync(playArguments).Forget();
            }
        }

        private async UniTask PlayMoveIconEffectAsync(PlayArguments playArguments)
        {
            await acquisitionOfLotteryRightsAnimation.PlayMoveIconEffectAsync(() => { });
            playArguments.Header.PlayAddExtraTurnRightEffectAsync(playArguments.EffectType).Forget();

            if (currentPhase == EffectPhase.MoveIcon)
            {
                PlayNextPhaseEffectAsync(playArguments).Forget();
            }
        }

        private async UniTask PlayCloseEffectAsync()
        {
            await acquisitionOfLotteryRightsAnimation.PlayCloseEffectAsync(() => { });
            currentPhase = EffectPhase.Closed;
        }
    }
}
