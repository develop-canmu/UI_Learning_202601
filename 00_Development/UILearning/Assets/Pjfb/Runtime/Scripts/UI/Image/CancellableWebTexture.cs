using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.ResourceManagement;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Pjfb
{
    public class CancellableWebTexture : MonoBehaviour
    {
        [SerializeField]
        private RawImage rawImage = null;
        
        protected CancellationTokenSource source = null;

        // ロード中か？
        protected bool isLoading = false;
        
        /// <summary>画像セット</summary>
        public void SetTexture(string key)
        {
            SetTextureAsync(key).Forget();
        }
        
        public async UniTask SetTextureAsync(string key)
        {
            // 画像非表示
            rawImage.gameObject.SetActive(false);
            // 画像読み込みキャンセル
            Cancel();
            // トークン生成
            source = new CancellationTokenSource();
            
            // タイムアウト処理が必要そうなら
            // source.CancelAfterSlim(timeout);
            
            CancellationToken token = source.Token;

            if (isLoading)
            {
                // 読み込み処理が重複しないようにロード中は待機する
                await UniTask.WaitUntil(() => isLoading == false, cancellationToken: token);
            }

            // ロード中に
            isLoading = true;
            
            Texture texture = null;
            
            try
            {
                // テクスチャ読み込み
                texture = await WebTextureManager.GetTextureAsync(key, token);
            }
            finally
            {
                // ロード完了
                isLoading = false;
            }
            
            // キャンセルされているならリターン
            if (token.IsCancellationRequested)
            {
                return;
            }
            
            // 画像表示
            if(texture != null && rawImage != null)
            {
                rawImage.texture = texture;
                rawImage.gameObject.SetActive(true);
            }
        }
        
        /// <summary>キャンセル</summary>
        public virtual void Cancel()
        {
            // キャンセル
            if(source != null)
            {
                source.Cancel();
                source.Dispose();
                source = null;
            }
        }
        
        protected virtual void OnDestroy()
        {
            // ゲームオブジェクト削除時にキャンセル
            Cancel();
        }
    }
}