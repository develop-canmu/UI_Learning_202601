using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Training;
using TMPro;
using UnityEngine.UI;

namespace Pjfb
{
    //// <summary> コンボ数毎のアニメーション </summary>
    public class TrainingCardComboAnimation : MonoBehaviour
    {
        private static readonly string OpenKey = "Open";
        private static readonly string IdleKey = "Idle";
        private static readonly string CloseKey = "Close";

        // コンボ倍率の等倍値(万分率指定)
        private const long comboDefaultRateValue = 10000;
        
        [SerializeField] private Animator animator;
        
        // コンボカード表示
        [SerializeField] private TrainingComboCardView[] comboCardView;

        // コンボ数
        [SerializeField] private Image comboImage;

        // ステータス上昇％
        [SerializeField] private TMP_Text statusPercent;

        // コンボボーナス倍率
        [SerializeField] private TMP_Text comboBonusRate;

        // コンボボーナス表示オブジェクト
        [SerializeField] private GameObject comboBonusRoot;

        // タップ表示オブジェクト
        [SerializeField] private GameObject tapObject;
        
        // 演出が完了済みか
        private bool isComplete = false;
        public bool IsComplete => isComplete;

        // コンボ演出の実行タイプ
        private TrainingCardComboBaseAnimation.EffectMode effectMode = TrainingCardComboBaseAnimation.EffectMode.None;
        
        // 待機中か
        private bool isIdle = false;
        // 演出終了までの待機時間
        private float waitTime = 0f;
        // 完了時のイベント
        private Action onComplete = null;

        //// <summary> アニメーション再生前の初期化 </summary>
        public async UniTask SetUp(Sprite comboImage, long statusPercent, long comboBonusRate, long[] comboCardIdList, TrainingCardComboBaseAnimation.EffectMode effectMode, float waitTime, Action onComplete)
        {
            this.waitTime = waitTime;
            this.onComplete = onComplete;
            this.effectMode = effectMode;
            isComplete = false;
            isIdle = false;

            // 演出が自動遷移でないならタップ表示をオンに
            if (effectMode == TrainingCardComboBaseAnimation.EffectMode.None)
            {
                tapObject.SetActive(true);
            }
            else
            {
                tapObject.SetActive(false);
            }
            
            // コンボ数
            this.comboImage.sprite = comboImage;
            // ステータス上昇％
            this.statusPercent.text = string.Format(StringValueAssetLoader.Instance["common.percent_value"], statusPercent);
            // コンボボーナス倍率(万分率)
            this.comboBonusRate.text = ((comboDefaultRateValue + comboBonusRate) / (float)comboDefaultRateValue).ToString();
            
            // ボーナス倍率が等倍の場合は非表示
            comboBonusRoot.SetActive(comboBonusRate > 0);
            
            // コンボカードをセット(配列の最初が選択カード)
            for (int i = 0; i < comboCardView.Length; i++)
            {
                await comboCardView[i].PracticeCard.SetCardAsync(comboCardIdList[i]);
            }
        }
        
        //// <summary> アニメーション再生 </summary>
        public async UniTask PlayAnimation(CancellationToken token)
        {
            gameObject.SetActive(true);
            
            // 最終結果のみ表示
            if (effectMode == TrainingCardComboBaseAnimation.EffectMode.Skip)
            {
                await ShowFinishView(token);
            }
            else
            {
                // アニメーション再生
                animator.SetTrigger(OpenKey);
                // Idleになるまで待機
                await AnimatorUtility.WaitStateAsync(animator, IdleKey, token);
            }

            CompleteAsync(token).Forget();
        }

        //// <summary> アニメーション終了 </summary>
        public async UniTask CloseAnimationAsync(Action onCloseComplete)
        {
            await AnimatorUtility.WaitStateAsync(animator, CloseKey);
            gameObject.SetActive(false);
            onCloseComplete();
        }

        //// <summary> アニメーションスキップ時の処理 </summary>
        public async UniTask SkipAnimationAsync(CancellationToken token, Action onShowComplete)
        {
            // 待機中なら即終了
            if (isIdle)
            {
                isComplete = true;
                // 完了イベント発火
                onComplete();
            }
            // そうでないなら待機状態になるまで強制的に進める
            else
            {
                await ShowFinishView(token);
                // Idle状態にする
                isIdle = true;
                // 表示が完了した際の処理を実行
                onShowComplete();
                // Idleに変わるまで待機
                await AnimatorUtility.WaitStateAsync(animator, IdleKey, token);
                CompleteAsync(token).Forget();
            }
        }

        // 完了時の処理
        private async UniTask CompleteAsync(CancellationToken token)
        {
            // すでに処理が終わっているならリターン
            if (isComplete)
            {
                return;
            }

            // 待機状態
            isIdle = true;
            
            // 待機時間があるなら(待機時間が設定されていないなら完了イベント)
            if (waitTime > 0)
            {
                // 指定秒数待機
                await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken:token);
                isComplete = true;
                // 完了イベント発火
                onComplete();
            }
        }

        //// <summary> コンボ演出の結果強制表示 </summary>
        private async UniTask ShowFinishView(CancellationToken token)
        {
            // トリガーをすべてオフ
            animator.ResetAllTriggers();
            // コンボ演出の終了地点まで進める
            animator.Play(OpenKey, 0, 1);
            // 強制遷移するまで待機
            await AnimatorUtility.WaitStateFinishAsync(animator, OpenKey, token);
                
            // カードコンボの周辺エフェクトを強制表示
            foreach (TrainingComboCardView cardView in comboCardView)
            {
                cardView.CardComboEffect.PlayCardComboFrameEffect(true, true, true);
            }
                
            // 連結エフェクトを強制表示
            foreach (TrainingComboCardView cardView in comboCardView)
            {
                cardView.CardComboEffect.PlayCardComboConnectLineEffect(true);
            }
            
            // 連結接続用の球体オブジェクトを強制表示
            foreach (TrainingComboCardView cardView in comboCardView)
            {
                cardView.CardComboEffect.ShowComboSphereImmediate();
            }
        }

        #region AnimationEvent
        
        // AnimationEvent
        
        //// <summary> カード周辺のコンボ演出再生 </summary>
        public void PlayComboEffectFrame()
        {
            foreach (TrainingComboCardView cardView in comboCardView)
            {
                cardView.CardComboEffect.PlayCardComboFrameEffect(true, true);
            }
        }

        //// <summary> カードコンボの連結エフェクト再生 </summary>
        public void PlayComboEffectLine()
        {
            foreach (TrainingComboCardView cardView in comboCardView)
            {
                cardView.CardComboEffect.PlayCardComboConnectLineEffect();
            }
        }
        
        #endregion
    }
}