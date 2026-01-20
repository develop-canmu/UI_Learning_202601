using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    //// <summary> カードコンボのベースアニメーション </summary>
    public class TrainingCardComboBaseAnimation : MonoBehaviour
    {
        private static readonly string OpenKey = "Open";

        // オートスキップ段階毎の演出タイプ
        public enum EffectMode
        {
            None,
            // 自動
            Auto,
            // スキップ
            Skip,
        }
        
        // コンボ数毎に発生するアニメーションのデータ
        [Serializable]
        private class ComboData
        {
            // コンボ数
            [SerializeField] 
            private long comboValue;

            // コンボイメージ画像
            [SerializeField] 
            private Sprite comboImage;
            
            // アニメーション
            [SerializeField]
            private TrainingCardComboAnimation animation;

            public long ComboValue => comboValue;
            public Sprite ComboImage => comboImage;
            public TrainingCardComboAnimation Animation => animation;
        }

        // 発生するコンボリスト
        [SerializeField] private ComboData[] comboList;
        
        // ベースアニメーション
        [SerializeField] private Animator animator;

        // スキップボタン
        [SerializeField] private GameObject skipButton;
        
        // CloseModeがAutoの場合の演出表示時間
        [SerializeField] private float autoEffectCloseTime = 0f;
        // CloseModeがSkipの場合の演出表示時間
        [SerializeField] private float skipEffectCloseTime = 0f;

        // コンボデータ
        private ComboData comboData = null;

        // キャンセルトークン
        private CancellationTokenSource cancelTokenSource = null;
        
        // 演出終了までの時間
        private float waitTime = 0f;
        
        public void PlayCardComboAnimation(long comboValue, long statusPercent, long comboBonusRate, long[] comboCardIdList, EffectMode effectMode, Action onComplete)
        {
            PlayCardComboAnimationAsync(comboValue, statusPercent, comboBonusRate, comboCardIdList, effectMode, onComplete).Forget();
        }
        
        //// <summary> カードコンボ演出 </summary>
        public async UniTask PlayCardComboAnimationAsync(long comboValue, long statusPercent, long comboBonusRate, long[] comboCardIdList, EffectMode effectMode, Action onComplete)
        {
            gameObject.SetActive(true);
            // スキップボタンをオフに
            skipButton.SetActive(false);
            cancelTokenSource = new CancellationTokenSource();
            comboData = null;
            
            for (int i = 0; i < comboList.Length; i++)
            {
                // 一致するコンボデータを探す
                if (comboList[i].ComboValue == comboValue)
                {
                    comboData = comboList[i];
                    break;
                }
            }

            // コンボ数に一致するデータが定義されてないならエラーを出す
            if (comboData == null)
            {
                CruFramework.Logger.LogError($"Not Find Combo Value {comboValue} in ComboList");
                return;
            }
            
            switch (effectMode)
            {
                // 特に指定なしの場合,時間で閉じない
                case EffectMode.None:
                {
                    waitTime = -1f;
                    break;
                }
                // 自動
                case EffectMode.Auto:
                {
                    waitTime = autoEffectCloseTime;
                    break;
                }
                // スキップ
                case EffectMode.Skip:
                {
                    waitTime = skipEffectCloseTime;
                    break;
                }
            }

            await comboData.Animation.SetUp(comboData.ComboImage, statusPercent, comboBonusRate, comboCardIdList, effectMode, waitTime, onComplete);
            
            // ベースアニメーションを再生
            animator.SetTrigger(OpenKey);
           }
        
        public void CloseAnimation(Action onComplete)
        {
            CloseAnimationAsync(onComplete).Forget();
        }
        
        //// <summary> アニメーション再生終了 </summary>
        public async UniTask CloseAnimationAsync(Action onComplete)
        {
            // スキップ不可に
            skipButton.SetActive(false);
            // キャンセルトークンの後始末
            Cancel();
            // アニメーション終了
            await comboData.Animation.CloseAnimationAsync(() =>
            {
                // baseのアクティブを先に切る
                gameObject.SetActive(false);
                onComplete();
            });
        }

        /// <summary>
        /// AnimationEvent
        /// </summary>
        
        public void PlayComboAnimation()
        {
            // スキップ可能に
            skipButton.SetActive(true);
            // コンボアニメーションを再生
            comboData.Animation.PlayAnimation(cancelTokenSource.Token).Forget();
        }
        
        //// <summary> スキップ処理 </summary>
        public void OnSkipButton()
        {
            // すでにアニメーションが完了しているなら処理しない
            if (comboData == null || comboData.Animation.IsComplete)
            {
                return;
            }
            
            // キャンセル
            Cancel();
            cancelTokenSource = new CancellationTokenSource();
            // 表示の強制遷移が終わるまではスキップできないように
            skipButton.SetActive(false);
            // 演出をスキップ
            comboData.Animation.SkipAnimationAsync(cancelTokenSource.Token, () => skipButton.SetActive(true)).Forget();
        }

        //// <summary> キャンセル処理 </summary>
        private void Cancel()
        {
            if (cancelTokenSource != null)
            {
                cancelTokenSource.Cancel();
                cancelTokenSource.Dispose();
                cancelTokenSource = null;
            }
        }
    }
}