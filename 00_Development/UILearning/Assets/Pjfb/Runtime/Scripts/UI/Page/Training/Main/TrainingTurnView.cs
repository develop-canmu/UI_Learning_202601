using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Training
{
    public class TrainingTurnView : MonoBehaviour
    {
        private const string AddExtraTurnRightTrigger = "AddExtraTurnRight";
        private const string StartLotteryTrigger = "StartLottery";
        private const string EnterExtraTurnTrigger = "EnterExtraTurn";
        private const string ContinueExtraTurnTrigger = "ContinueExtraTurn";

        public const float GaugeAnimationDuration = 1f;

        [SerializeField] private TMP_Text turnText = null;
        [SerializeField] private Image gaugeImage = null;
        [SerializeField] private Image questionImage = null;
        [SerializeField] private Animator animator = null;
        [SerializeField] private List<Sprite> gaugeSprites;
        [SerializeField] private List<Sprite> questionSprites;
        [SerializeField] private Sprite defaultGaugeSprite;
        [SerializeField] private Sprite defaultQuestionSprite;
        [SerializeField] private TrainingExtraTurnView extraTurnView;

        private readonly TrainingGaugeAnimation gaugeAnimation = new TrainingGaugeAnimation(GaugeAnimationDuration);

        private long reservedTurn;

        private void Awake()
        {
            animator.keepAnimatorStateOnDisable = true;
        }

        /// <summary>ターン数</summary>
        public void SetTurn(long current, long max)
        {
            turnText.text = current.ToString();
            gaugeImage.sprite = defaultGaugeSprite;
            questionImage.sprite = defaultQuestionSprite;
            gaugeAnimation.SetValue(gaugeImage.fillAmount, (float)current / (max - 1));
        }

        public void SetTurnHasExtraTurnRight(long current, long max, long turnAddEffectType)
        {
            turnText.text = current.ToString();
            gaugeImage.sprite = gaugeSprites[(int)turnAddEffectType];
            questionImage.sprite = questionSprites[(int)turnAddEffectType];
            gaugeAnimation.SetValue(gaugeImage.fillAmount, (float)current / (max - 1));
        }

        private void SetTurnFirstExtraTurn(long current, long max, long turnAddEffectType)
        {
            turnText.text = current.ToString();
            gaugeImage.sprite = gaugeSprites[(int)turnAddEffectType];
            questionImage.sprite = questionSprites[(int)turnAddEffectType];
            gaugeAnimation.SetValue(gaugeImage.fillAmount, (float)current / max);
        }

        private void SetTurnContinueExtraTurnRecovery()
        {
            gaugeAnimation.SetValue(gaugeImage.fillAmount, 1);
        }

        private void Update()
        {
            gaugeImage.fillAmount = gaugeAnimation.Update();
        }

        /// <summary>
        /// ターン延長「権利」獲得演出
        /// </summary>
        public async UniTask PlayAddExtraTurnRightEffectAsync(long turnAddEffectType)
        {
            // 非アクティブ状態だとAnimatorを制御できないため待機
            var token = gameObject.GetCancellationTokenOnDestroy();
            await UniTask.WaitUntil(() => gameObject.activeInHierarchy, cancellationToken: token);
            if (token.IsCancellationRequested) return;
            
            AnimatorUtility.WaitStateAsync(animator, AddExtraTurnRightTrigger, token).Forget();
            gaugeImage.sprite = gaugeSprites[(int)turnAddEffectType];
            questionImage.sprite = questionSprites[(int)turnAddEffectType];
        }

        /// <summary>
        /// ターン延長「権利」獲得演出
        /// </summary>
        public async UniTask SkipToEndAddExtraTurnRightEffectAsync(long current, long max, long turnAddEffectType)
        {
            // 非アクティブ状態だとAnimatorを制御できないため待機
            var token = gameObject.GetCancellationTokenOnDestroy();
            await UniTask.WaitUntil(() => gameObject.activeInHierarchy, cancellationToken: token);
            if (token.IsCancellationRequested) return;
            
            animator.SkipToEnd(AddExtraTurnRightTrigger);
            await UniTask.DelayFrame(1, cancellationToken: token);
            SetTurnHasExtraTurnRight(current, max, turnAddEffectType);
        }

        /// <summary>
        /// ターン延長確定、スロット演出前の演出
        /// </summary>
        public UniTask PlayStartLotteryEffectAsync() => AnimatorUtility.WaitStateAsync(animator, StartLotteryTrigger);

        /// <summary>
        /// ターン延長確定、スロット演出後の演出
        /// </summary>
        public async UniTask PlayEnterExtraTurnEffectAsync(long extraTurn, long turnAddEffectType)
        {
            SetTurnFirstExtraTurn(extraTurn, extraTurn, turnAddEffectType);

            // Animation側のタイミングで表示させたいため一度テキストを0表示に戻す
            reservedTurn = extraTurn;
            turnText.text = "0";
            await AnimatorUtility.WaitStateAsync(animator, EnterExtraTurnTrigger);
        }

        /// <summary>
        /// ターン延長確定、スロット演出後の演出
        /// </summary>
        public async UniTask SkipToEndEnterExtraTurnEffectAsync(long current, long max, long addedTurn, long turnAddEffectType)
        {
            animator.SkipToEnd(EnterExtraTurnTrigger);
            await UniTask.DelayFrame(1);
            SetTurnFirstExtraTurn(current, max, turnAddEffectType);
            extraTurnView.UpdateTurn(addedTurn);
        }

        /// <summary>
        /// ターン延長継続演出
        /// </summary>
        public async UniTask PlayContinueExtraTurnEffectAsync()
        {
            SetTurnContinueExtraTurnRecovery();
            await AnimatorUtility.WaitStateAsync(animator, ContinueExtraTurnTrigger);
        }

        /// <summary>
        /// ターン延長継続演出
        /// </summary>
        public async UniTask SkipToEndContinueExtraTurnEffectAsync()
        {
            animator.SkipToEnd(ContinueExtraTurnTrigger);
            await UniTask.DelayFrame(1);
        }

        /// <summary>
        /// AnimationEvent
        /// </summary>
        public void UpdateTurnText()
        {
            turnText.text = reservedTurn.ToString();
        }
    }
}
