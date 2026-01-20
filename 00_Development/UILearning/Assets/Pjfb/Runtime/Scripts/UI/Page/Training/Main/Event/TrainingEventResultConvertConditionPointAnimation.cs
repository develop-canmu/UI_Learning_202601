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
    public class TrainingEventResultConvertConditionPointAnimation : TrainingEventResultConvertBaseAnimation
    {
        // AdvLog
        private static readonly string LogConverConditiontKey = "training.log.point_convert.condition";
        
        // 変換されたコンディションポイントテキスト
        [SerializeField] private TMP_Text convertBoostPointText;
        
        //// <summary> コンディションポイント変換のアニメーション再生 </summary>
        public void PlayConvertConditionPointAnimation(Param param, long convertPointValue, Action onComplete)
        {
            PlayConvertConditionPointAnimationAsync(param, convertPointValue, onComplete).Forget();
        }

        private async UniTask PlayConvertConditionPointAnimationAsync(Param param, long convertPointValue, Action onComplete)
        {
            this.param = param;
            this.onComplete = onComplete;
            cancellTokenSource = new CancellationTokenSource();
            isComplete = false;
            this.convertPointValue = convertPointValue;
            // スキップ処理に登録
            onSkip += SkipCompleteConvert;
            // スキップボタンをオンに
            skipButton.gameObject.SetActive(true);
            // オート設定時はタップ表示を出さない
            tapRoot.gameObject.SetActive(param.isAuto == false);
            gameObject.SetActive(true);
            // タッチガードをオン
            touchGuard.SetActive(true);

            // 変換されたポイント数を桁区切りで表示
            boostPointText = convertPointValue.GetStringNumberWithComma();
            convertBoostPointText.text = boostPointText;
            
            // ブーストポイント表示UIの初期化
            InitBoostPointView(param, convertPointValue);
            
            // 変換のアニメーションを再生しIdleStateになるまで待つ
            animator.SetTrigger(ConvertKey);
            await AnimatorUtility.WaitStateChangeAsync(animator, IdleKey, cancellTokenSource.Token);
            
            // すでに完了しているなら処理を終わる
            if (isComplete)return;

            CompleteConvert();
        }
        
        //// <summary> 変換演出完了時の処理 </summary>
        protected override void CompleteConvert()
        {
            // Advにログを追加
            param.AdvManager.AddMessageLog(StringValueAssetLoader.Instance[LogPointConvertKey], string.Format(StringValueAssetLoader.Instance[LogConverConditiontKey], boostPointText), 0);
            
            base.CompleteConvert();
        }
    }
}