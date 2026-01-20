using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
    public class UIColorFade : MonoBehaviour, IFade
    {
        [SerializeField]
        private Image fadeImage = null;
        
        [SerializeField]
        private float duration = 1.0f;
        
        [SerializeField]
        private AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);
        
        /// <summary>フェードイン</summary>
        public async UniTask FadeInAsync(CancellationToken token)
        {
            // アルファを1にする
            Color color = fadeImage.color;
            color.a = 1;
            fadeImage.color = color;
            
            // フェード画像表示
            fadeImage.gameObject.SetActive(true);
            
            // 待機
            await fadeImage.DOFade(0, duration).SetEase(curve).ToUniTask(cancellationToken: token);
            
            // フェード画像非表示
            fadeImage.gameObject.SetActive(false);
        }
        
        /// <summary>フェードアウト</summary>
        public async UniTask FadeOutAsync(CancellationToken token)
        {
            // アルファを0にする
            Color color = fadeImage.color;
            color.a = 0;
            fadeImage.color = color;
            
            // フェード画像表示
            fadeImage.gameObject.SetActive(true);
            
            // 待機
            await fadeImage.DOFade(1, duration).SetEase(curve).ToUniTask(cancellationToken: token);
        }
    }
}
