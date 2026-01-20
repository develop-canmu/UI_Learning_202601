using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Extensions;
using TMPro;

namespace Pjfb
{
    public class TrainingEventResultConvertExtraTurnPointAnimation : TrainingEventResultConvertBaseAnimation
    {
        private static readonly string OpenKey = "OpenExtraTurn";
        
        // AdvLog
        private static readonly string LogConverExtraTurntKey = "training.log.point_convert.extra_turn";
        
        // ターン延長の表示テキスト
        [SerializeField] private TMP_Text extraTurnValueText;
        // 変換されたブーストポイントテキスト
        [SerializeField] private TMP_Text convertBoostPointText;

        // 延長ターン数
        private long extraTurnValue = 0;
        
        //// <summary> ターン延長ポイント変換アニメーション再生 </summary>
        public void PlayConvertExtraTurnPointAnimation(Param param, long extraTurnValue, long convertPointValue, Action onComplete)
        {
            PlayConvertExtraTurnPointAnimationAsync(param, extraTurnValue, convertPointValue, onComplete).Forget();
        }

        private async UniTask PlayConvertExtraTurnPointAnimationAsync(Param param, long extraTurnValue, long convertPointValue, Action onComplete)
        {
            this.param = param;
            this.onComplete = onComplete;
            cancellTokenSource = new CancellationTokenSource();
            isComplete = false;
            this.extraTurnValue = extraTurnValue;
            this.convertPointValue = convertPointValue;
            // スキップ処理に登録
            onSkip += SkipCompleteConvert;
            // スキップボタンをオンに
            skipButton.gameObject.SetActive(true);
            // オート設定時はタップ表示を出さない
            tapRoot.gameObject.SetActive(param.isAuto == false);
            // オブジェクトをアクティブに
            gameObject.SetActive(true);
            // タッチガードをオン
            touchGuard.SetActive(true);
            
            // ターン延長数の設定
            extraTurnValueText.text = string.Format(StringValueAssetLoader.Instance["training.event_result.convert.extra_turn.turn_count"], extraTurnValue);
            // 桁区切り
            boostPointText = convertPointValue.GetStringNumberWithComma();
            convertBoostPointText.text = boostPointText;
            // ブーストポイント表示UIの初期化
            InitBoostPointView(param, convertPointValue);
            
            // アニメーションを再生
            animator.SetTrigger(OpenKey);
            
            // IdleStateに変わるまで待つ
            await AnimatorUtility.WaitStateChangeAsync(animator, IdleKey, cancellTokenSource.Token);

            // すでに完了しているなら処理を終わる
            if (isComplete)return;
            
            CompleteConvert();
        }

        //// <summary> 変換演出完了時の処理 </summary>
        protected override void CompleteConvert()
        {
            // Advにログを追加
            param.AdvManager.AddMessageLog(StringValueAssetLoader.Instance[LogPointConvertKey], string.Format(StringValueAssetLoader.Instance[LogConverExtraTurntKey], extraTurnValue, boostPointText), 0);
            
            base.CompleteConvert();
        }
    }
}