using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.Adv;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Training;

namespace Pjfb
{
    public abstract class TrainingEventResultConvertBaseAnimation : MonoBehaviour
    {
        protected static readonly string ConvertKey = "Convert";
        protected static readonly string IdleKey = "Idle";
        protected static readonly string MoveIconKey = "MoveIcon";
        protected static readonly string CloseKey = "Close";
        
        // AdvLog
        protected static readonly string LogPointConvertKey = "training.log.point_convert";
        
        public class Param
        {
            // トレーニングポイント数
            public long TrainingPointValue;
            // トレーニングシナリオId
            public long TrainingScenarioId;
            // トレーニングポイントレベル
            public long TrainingPointLevel;
            // 手札入れ替え回数
            public long TrainingHandReloadCount;
            // ADVマネージャー
            public AdvManager AdvManager;
            // MainArgs
            public TrainingMainArguments MainArguments;
            // オート設定か
            public bool isAuto = false;
        }
        
        [SerializeField] protected Animator animator;
        [SerializeField] protected GameObject touchGuard;
        // ブーストポイント表示UI
        [SerializeField] protected TrainingActionBoostPointView pointView;
        // スキップボタン
        [SerializeField] protected UIButton skipButton;
        // 次に送るTapToNextのRootオブジェクト
        [SerializeField] protected GameObject tapRoot;
        
        protected Param param;
        // キャンセルトークン
        protected CancellationTokenSource cancellTokenSource = null;
        protected Action onComplete = null;
        // スキップ時の処理
        protected Action onSkip = null;
        // 処理が完了したか
        protected bool isComplete = false;
        // 変換ポイント数
        protected long convertPointValue = 0;
        // 桁区切り後のポイントテキスト
        protected string boostPointText = "";
        
        //// <summary> ポイントの変換の結果を閉じる </summary>
        public void CloseConvertAnimation(Action onComplete)
        {
            CloseConvertAnimationAsync(onComplete).Forget();
        }

        private async UniTask CloseConvertAnimationAsync(Action onComplete)
        {
            this.onComplete = onComplete;
            // すでにキャンセルされてるなら新しくトークンを取得する
            if (cancellTokenSource == null)
            {
                cancellTokenSource = new CancellationTokenSource();
            }
            
            isComplete = false;
            // スキップ処理に登録
            onSkip += SkipCompleteClose;
            
            // スキップボタンをオンに
            skipButton.gameObject.SetActive(true);
            // タッチガードをオンに
            touchGuard.SetActive(true);
            // アイコンの移動演出を再生
            animator.SetTrigger(MoveIconKey);
            // 変換のアニメーションを再生しアニメーションが終わるまで待つ
            await AnimatorUtility.WaitStateChangeAsync(animator, CloseKey, cancellTokenSource.Token);

            // すでに完了しているなら処理を終わる
            if (isComplete)return;
            
            CompleteClose();
        }
        
        //// <summary> ブーストポイント表示UIの初期化 </summary>
        protected void InitBoostPointView(Param param, long convertPointValue)
        {
            this.param = param;
            pointView.SetView(param.MainArguments);
            // ポイントの所持数は獲得したポイント数が加算されているので一時的に獲得ポイント数を引いた数値で表示する
            pointView.SetView(param.TrainingPointValue - convertPointValue, false, false);
        }

        //// <summary> 終了時の処理 </summary>
        private void CompleteClose()
        {
            // スキップ処理から削除
            onSkip -= SkipCompleteClose;
            // オブジェクトのアクティブを切る
            gameObject.SetActive(false);
            Complete();
        }
        
        //// <summary> 変換演出完了時の処理 </summary>
        protected virtual void CompleteConvert()
        {
            // スキップ処理から削除
            onSkip -= SkipCompleteConvert;
            // ConvertステートだとExitTime分の待ち時間が発生するのでIdleステートに移動しておく
            animator.Play(IdleKey,0,0);
            Complete();
        }

        //// <summary> 完了時の共通処理 </summary>
        private void Complete()
        {
            isComplete = true;
            // キャンセルトークンのリソース解放
            Cancel();
            // タッチガードをオフに
            touchGuard.SetActive(false);
            // スキップボタンをオフに
            skipButton.gameObject.SetActive(false);
            // 完了イベント
            onComplete();
        }
        
        //// <summary> 変換演出が完了するところまでスキップする </summary>
        protected void SkipCompleteConvert()
        {
            SkipComplete(ConvertKey, CompleteConvert).Forget();
        }

        //// <summary> Closeまでスキップ </summary>
        private void SkipCompleteClose()
        {
            SkipComplete(CloseKey, CompleteClose).Forget();
        }

        //// <summary> 演出が完了するところまでスキップする </summary>
        private async UniTask SkipComplete(string skipState, Action skipComplete)
        {
            // キャンセル
            Cancel();
            // トリガーをすべてオフにする
            animator.ResetAllTriggers();
            
            // 変換アニメーションの終了地点まで進める
            animator.Play(skipState, 0, 1);
            // 演出完了地点に変わるまで待機
            await AnimatorUtility.WaitStateFinishAsync(animator, skipState);
            skipComplete();
        }
        
        //// <summary> AnimationEvent </summary>
        public void UpdatePointView()
        {
            // 現在の獲得ポイント数に更新
            pointView.SetView(param.TrainingPointValue, false, false);
        }

        //// <summary> スキップボタン </summary>
        public void OnSkipButton()
        {
            // すでに完了しているなら何もしない
            if (isComplete) return;
            
            if (onSkip != null)
            {
                onSkip();
            }
        }

        //// <summary> キャンセル時の処理 </summary>
        protected void Cancel()
        {
            if (cancellTokenSource != null)
            {
                cancellTokenSource.Cancel();
                cancellTokenSource.Dispose();
                cancellTokenSource = null;
            }
        }
    }
}