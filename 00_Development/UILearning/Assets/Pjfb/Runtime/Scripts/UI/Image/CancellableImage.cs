using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
    public class CancellableImage : CancellableImageBase
    {
        [SerializeField]
        private Image image = null;
        /// <summary>イメージ</summary>
        protected Image Image { get { return image; } }

        /// <summary>画像セット</summary>
        public override async UniTask SetTextureAsync(string key, ResourcesLoader resourcesLoader)
        {
            // 読み込み前通知
            OnPreLoadTexture();
            // 画像読み込みキャンセル
            Cancel();
            // トークン生成
            source = new CancellationTokenSource();
            
            // 画像読み込み
            await resourcesLoader.LoadAssetAsync<Sprite>(key,
                sprite =>
                {
                    // TODO 下記エラーがキャラ強化画面で吐かれるので一旦暫定対応
                    // MissingReferenceException: The object of type 'Image' has been destroyed but you are still trying to access it.
                    if(image == null) return;
                    
                    // テクスチャセット
                    image.sprite = sprite;
                    // 読み込み後通知
                    OnPostLoadTexture();
                },
                source.Token
            );
        }

        protected override void OnPreLoadTexture()
        {
            if(image != null)
            {
                // 画像非表示
                image.enabled = false;
            }
        }

        protected override void OnPostLoadTexture()
        {
            // 画像表示
            image.enabled = true;
        }

        public override void SetColor(Color color)
        {
            image.color = color;
        }

        public override void SetActiveImage(bool value)
        {
            image.gameObject.SetActive(value);
        }
    }
}
