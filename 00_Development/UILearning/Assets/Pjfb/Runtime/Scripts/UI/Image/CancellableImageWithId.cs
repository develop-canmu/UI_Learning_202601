using System.Collections;
using System.Collections.Generic;
using CruFramework.ResourceManagement;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    public abstract class CancellableImageWithId : CancellableImage
    {
        /// <summary>キー取得</summary>
        protected abstract string GetKey(long id);
        
        /// <summary>画像セット</summary>
        public void SetTexture(long id)
        {
            string key = GetKey(id);
            SetTexture(key);
        }
        
        /// <summary>画像セット</summary>
        public void SetTexture(long id, ResourcesLoader resourcesLoader)
        {
            string key = GetKey(id);
            SetTexture(key, resourcesLoader);
        }

        /// <summary>画像セット</summary>
        public async UniTask SetTextureAsync(long id)
        {
            string key = GetKey(id);
            await SetTextureAsync(key);
        }
        
        /// <summary>画像セット</summary>
        public async UniTask SetTextureAsync(long id, ResourcesLoader resourcesLoader)
        {
            string key = GetKey(id);
            await SetTextureAsync(key, resourcesLoader);
        }

        /// <summary> Sprite取得 </summary>
        public Sprite GetSprite()
        {
            return Image.sprite;
        }
    }
}