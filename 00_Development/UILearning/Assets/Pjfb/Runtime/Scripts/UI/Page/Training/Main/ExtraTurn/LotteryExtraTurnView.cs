using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Pjfb.Training
{
    public class LotteryExtraTurnView : MonoBehaviour
    {
        private const int SlotEffectLevel2Min = 3;
        private const int SlotEffectLevel3Min = 5;

        private enum EffectPhase
        {
            Closed,
            Open,
            Slot1Roll,
            Slot1,
            Slot2Roll,
            Slot2,
            Slot3Roll,
            Slot3,
            Close
        }

        private EffectPhase currentPhase;

        private const string OpenTrigger = "Open";
        private const string CloseTrigger = "Close";
        private const string StartRollingTrigger = "StartRolling";
        private const string RollDigitsTrigger = "RollDigits";
        private const string ExtraTurnLevel1Trigger = "ExtraTurnLevel001";
        private const string ExtraTurnLevel2Trigger = "ExtraTurnLevel002";
        private const string ExtraTurnLevel3Trigger = "ExtraTurnLevel003";

        [SerializeField] private Animator animator;
        [SerializeField] private List<TextMeshProUGUI> digitsTexts;

        public async UniTask PlayEffectAsync(long extraTurn, bool isFastMode)
        {
            if (isFastMode)
            {
                currentPhase = EffectPhase.Open;
                SkipEffectForget(extraTurn);
            }
            else
            {
                PlayOpenEffectAsync(extraTurn).Forget();
            }

            await UniTask.WaitUntil(() => currentPhase == EffectPhase.Closed);
        }

        public void SkipEffectForget(long extraTurn)
        {
            switch (currentPhase)
            {
                case EffectPhase.Closed:
                case EffectPhase.Close:
                    return;
                case EffectPhase.Open:
                case EffectPhase.Slot1Roll:
                case EffectPhase.Slot1:
                case EffectPhase.Slot2Roll:
                case EffectPhase.Slot2:
                case EffectPhase.Slot3Roll:
                case EffectPhase.Slot3:
                    var finalSlotPhase = GetFinalSlotPhase(extraTurn);
                    if (finalSlotPhase != currentPhase)
                    {
                        currentPhase = finalSlotPhase;
                        PlayPhaseEffectAsync(currentPhase, extraTurn).Forget();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private UniTask PlayNextPhaseEffectAsync(long extraTurn)
        {
            if (currentPhase == EffectPhase.Close)
            {
                return UniTask.CompletedTask;
            }

            return PlayPhaseEffectAsync(currentPhase + 1, extraTurn);
        }

        private async UniTask PlayPhaseEffectAsync(EffectPhase phase, long extraTurn)
        {
            switch (phase)
            {
                case EffectPhase.Closed:
                    break;
                case EffectPhase.Open:
                    await PlayOpenEffectAsync(extraTurn);
                    break;
                case EffectPhase.Slot1Roll:
                    await PlaySlotRollEffectAsync(EffectPhase.Slot1Roll, extraTurn);
                    break;
                case EffectPhase.Slot1:
                    await PlaySlotEffectAsync(EffectPhase.Slot1, extraTurn, 0, SlotEffectLevel2Min - 1, ExtraTurnLevel1Trigger);
                    break;
                case EffectPhase.Slot2Roll:
                    await PlaySlotRollEffectAsync(EffectPhase.Slot2Roll, extraTurn);
                    break;
                case EffectPhase.Slot2:
                    await PlaySlotEffectAsync(EffectPhase.Slot2, extraTurn, SlotEffectLevel2Min, SlotEffectLevel3Min - 1, ExtraTurnLevel2Trigger);
                    break;
                case EffectPhase.Slot3Roll:
                    await PlaySlotRollEffectAsync(EffectPhase.Slot3Roll, extraTurn);
                    break;
                case EffectPhase.Slot3:
                    await PlaySlotEffectAsync(EffectPhase.Slot3, extraTurn, SlotEffectLevel3Min, (int)extraTurn, ExtraTurnLevel3Trigger);
                    break;
                case EffectPhase.Close:
                    await PlayCloseEffectAsync();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async UniTask PlayOpenEffectAsync(long extraTurn)
        {
            const EffectPhase handlePhase = EffectPhase.Open;
            currentPhase = handlePhase;

            SetDigitsTexts(0);
            await AnimatorUtility.WaitStateAsync(animator, OpenTrigger);

            if (handlePhase == currentPhase)
            {
                PlayNextPhaseEffectAsync(extraTurn).Forget();
            }
        }

        private async UniTask PlaySlotRollEffectAsync(EffectPhase handlePhase, long extraTurn)
        {
            currentPhase = handlePhase;

            await AnimatorUtility.WaitStateAsync(animator, StartRollingTrigger);
            if (handlePhase != currentPhase)
            {
                return;
            }

            await AnimatorUtility.WaitStateAsync(animator, RollDigitsTrigger);

            if (handlePhase == currentPhase)
            {
                PlayNextPhaseEffectAsync(extraTurn).Forget();
            }
        }

        private async UniTask PlaySlotEffectAsync(EffectPhase handlePhase, long extraTurn, int minDigits, int maxDigits, string effectStateName)
        {
            currentPhase = handlePhase;

            var finalSlotPhase = GetFinalSlotPhase(extraTurn);
            if (finalSlotPhase < handlePhase)
            {
                PlayCloseEffectAsync().Forget();
                return;
            }

            var thisPhaseDigits = Mathf.Clamp((int)extraTurn, minDigits, maxDigits);
            SetDigitsTexts(thisPhaseDigits);
            await AnimatorUtility.WaitStateAsync(animator, effectStateName);

            if (handlePhase != currentPhase)
            {
                return;
            }

            if (handlePhase == finalSlotPhase)
            {
                PlayCloseEffectAsync().Forget();
                return;
            }

            PlayNextPhaseEffectAsync(extraTurn).Forget();
        }

        private async UniTask PlayCloseEffectAsync()
        {
            currentPhase = EffectPhase.Close;
            await AnimatorUtility.WaitStateAsync(animator, CloseTrigger);
            currentPhase = EffectPhase.Closed;
        }

        private void SetDigitsTexts(long turn)
        {
            foreach (var item in digitsTexts)
            {
                item.text = turn.ToString();
            }
        }

        private static EffectPhase GetFinalSlotPhase(long extraTurn)
        {
            if (extraTurn < SlotEffectLevel2Min) return EffectPhase.Slot1;
            if (extraTurn < SlotEffectLevel3Min) return EffectPhase.Slot2;

            return EffectPhase.Slot3;
        }
    }
}
