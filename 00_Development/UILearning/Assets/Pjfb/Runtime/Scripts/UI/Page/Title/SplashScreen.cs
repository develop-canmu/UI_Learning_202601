using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb.Title
{
    public class SplashScreen : MonoBehaviour
    {
        [Serializable]
        private class Data
        {
            public Image splashImage = null;
            public float waitDuration = 0.5f;
        }
        
        [SerializeField]
        private Image backgroundImage = null;
        
        [SerializeField]
        private float fadeDuration = 1.0f;
        
        [SerializeField]
        private Data[] datas = null;
        
        private float timer = 0;
        
        private bool IsWaiting
        {
            get { return timer > 0; }
        }

        public async UniTask PlayAsync(CancellationToken token)
        {
            for(int i = 0; i < datas.Length; i++)
            {
                // 表示
                await ShowSplashAsync(datas[i].splashImage, token);
                // 待機
                timer = datas[i].waitDuration;
                await UniTask.WaitWhile(() => IsWaiting, cancellationToken: token);
                // 非表示
                await HideSplashAsync(datas[i].splashImage, token);
            }
        }
        
        public async UniTask HideAsync(CancellationToken token)
        {
            // 背景を非表示
            await HideSplashAsync(backgroundImage, token);
            // 再生が終わったら非表示
            gameObject.SetActive(false);
        }
        
        private async UniTask ShowSplashAsync(Image image, CancellationToken token)
        {
            // アルファを0にする
            Color color = image.color;
            color.a = 0;
            image.color = color;
            // 表示 
            image.gameObject.SetActive(true);
            // 待機
            await image.DOFade(1, fadeDuration).ToUniTask(cancellationToken: token);
        }
        
        private async UniTask HideSplashAsync(Image image, CancellationToken token)
        {
            // 待機
            await image.DOFade(0, fadeDuration).ToUniTask(cancellationToken: token);
            // 非表示
            image.gameObject.SetActive(false);
        }

        private void Update()
        {
            if(IsWaiting)
            {
                timer -= Time.deltaTime;
            }
        }

        /// <summary>uGUI</summary>
        public void OnClickSkipButton()
        {
            timer = 0;
        }
    }
}
