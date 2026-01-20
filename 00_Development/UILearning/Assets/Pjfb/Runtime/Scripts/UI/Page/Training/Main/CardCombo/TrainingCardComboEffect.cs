using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Training;
using UnityEngine.UI;

namespace Pjfb
{
    // カードコンボに必要なクラスをまとめたクラス
    [Serializable]
    public class TrainingComboCardView
    {
        // 練習カード
        [SerializeField] private PracticeCardView practiceCard;
        public PracticeCardView PracticeCard => practiceCard;
        
        // カードコンボエフェクト
        [SerializeField] private TrainingCardComboEffect cardComboEffect;
        public TrainingCardComboEffect CardComboEffect => cardComboEffect;
    }
    
    public class TrainingCardComboEffect : MonoBehaviour
    {
        private static readonly string OpenKey = "Open";
        private static readonly string IdleKey = "Idle";
        private static readonly string CloseKey = "Close";
        
        // コンボ同士をつなげる連結エフェクト
        [SerializeField] private Animator comboLineAnimator;
        // カードの周りのエフェクト
        [SerializeField] private Animator comboSquareAnimator;
        // ハイライト制御のルートオブジェクト
        [SerializeField] private GameObject highlightRoot;
        // ハイライト色の画像
        [SerializeField] private Image highlightImage;
        // カード周りのエフェクト色
        [SerializeField] private Image[] squareLineImages;
        // 連結エフェクトの基準となるオブジェクト
        [SerializeField] private RectTransform lineEffectTop;
        // 連結時の接続球体アニメーション
        [SerializeField] private Animator comboSphere;
        // 連結エフェクトのベースサイズを計算するか
        [SerializeField] private bool isCalcLineSize = false;
        public RectTransform LineEffectTop => lineEffectTop;

        // 連結エフェクトのベースの長さ
        private float lineEffectLength = 0f; 
        
        private void Start()
        {
            // サイズ計算がいらない場合はとばす
            if (isCalcLineSize == false)
            {
                return;
            }
            // オブジェクトの幅を計算するために回転を消す
            lineEffectTop.transform.rotation = Quaternion.identity;
            // 連結エフェクトのベースオブジェクトの長さを計算する
            Vector3[] corners = new Vector3[4];
            // ４隅のワールド座標を取得
            lineEffectTop.GetWorldCorners(corners);
            // ワールド座標でのオブジェクトの幅を取得
            lineEffectLength = Mathf.Abs(corners[1].y - corners[0].y);
        }

        // カードの周りにエフェクトを発生させる
        public void PlayCardComboFrameEffect(bool isHighlight, bool isKeep, bool isSkipEffect = false)
        {
            comboSquareAnimator.gameObject.SetActive(true);
            // キャンセルされてトリガーが残る時があるので
            comboSquareAnimator.ResetAllTriggers();
            // ハイライト表示を行うか
            highlightRoot.gameObject.SetActive(isHighlight);
            
            // 演出を飛ばすなら最後のみ表示
            if (isSkipEffect)
            {
                comboSquareAnimator.Play(IdleKey, 0, 0);
            }
            else
            {
                comboSquareAnimator.SetTrigger(OpenKey);
            }

            // エフェクトを出し続ける場合はIdleに
            if (isKeep)
            {
                comboSquareAnimator.SetTrigger(IdleKey);
            }
        }

        //// <summary> カードの縁のエフェクト終了待機 </summary>
        public async UniTask CloseCardComboFrameEffectAsync(CancellationToken token, bool immediate)
        {
            // キャンセルされてトリガーが残る時があるので
            comboSquareAnimator.ResetAllTriggers();
            
            // 即時に終了させる場合
            if (immediate)
            {
                comboSquareAnimator.Play(CloseKey, 0, 1);
            }
            else
            {
                comboSquareAnimator.SetTrigger(CloseKey);
            }

            await AnimatorUtility.WaitStateFinishAsync(comboSquareAnimator, CloseKey, token);
            comboSquareAnimator.gameObject.SetActive(false);
        }

        //// <summary> カードコンボの連結エフェクト再生 </summary>
        public void PlayCardComboConnectLineEffect(bool isSkipEffect = false)
        {
            // 一部の演出ではLineが発生しない場合がありnullになってる
            if (comboLineAnimator == null)
            {
                return;
            }
            comboLineAnimator.gameObject.SetActive(true);
            // キャンセルされてトリガーが残る時があるので
            comboLineAnimator.ResetAllTriggers();

            // スキップするならIdleまで強制遷移
            if (isSkipEffect)
            {
                comboLineAnimator.Play(IdleKey, 0, 1);
            }
            else
            {
                comboLineAnimator.SetTrigger(OpenKey);
            }
        }

        //// <summary> カードコンボの連結エフェクト終了 </summary>
        public async UniTask CloseCardComboConnectLineEffectAsync(CancellationToken token, bool immediate)
        {
            // 一部の演出ではLineが発生しない場合がありnullになってる
            if (comboLineAnimator == null)
            {
                return;
            }
            
            // キャンセルされてトリガーが残る時があるので
            comboLineAnimator.ResetAllTriggers();
            
            // 即時に終了させる場合
            if (immediate)
            {
                comboLineAnimator.Play(CloseKey, 0,1);   
            }
            else
            {
                comboLineAnimator.SetTrigger(CloseKey);
            }
            await AnimatorUtility.WaitStateFinishAsync(comboLineAnimator, CloseKey, token);
            comboLineAnimator.gameObject.SetActive(false);
        }

        //// <summary> 連結エフェクトの連結調整 </summary>
        public void AdjustCardComboConnectLine(float distance, Quaternion rotation)
        {
            // サイズをセット
            comboLineAnimator.gameObject.transform.localScale = new Vector3(1, distance / lineEffectLength, 1);
            // 回転率を調整
            comboLineAnimator.gameObject.transform.rotation = rotation;
        }

        //// <summary> ハイライトカラーのセット </summary>
        public void SetHighlightColor(Color color)
        {
            highlightImage.color = color;
        }

        //// <summary> カード周りのエフェクト色のセット </summary>
        public void SetComboLineEffectColor(Color color)
        {
            foreach (Image image in squareLineImages)
            {
                image.color = color;
            }
        }

        //// <summary> コンボ連結用の球体オブジェクトのアクティブ切り替え </summary>
        public void SetActiveComboSphere(bool isActive)
        {
            comboSphere.gameObject.SetActive(isActive);
        }

        //// <summary> コンボ連結用の球体オブジェクトを即時に表示 </summary>
        public void ShowComboSphereImmediate()
        {
            // 球体オブジェクトが設定されていないならスルー
            if (comboSphere == null)
            {
                return;
            }
            comboSphere.Play(IdleKey);
        }
    }
}