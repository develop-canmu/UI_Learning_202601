using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    public abstract class CancellableWebTextureWithId : CancellableWebTexture
    {
        protected abstract string GetURL(long id);
        
        /// <summary>画像セット</summary>
        public void SetTexture(long id)
        {
            string url = GetURL(id);
            SetTexture(url);
        }

        /// <summary>画像セット</summary>
        public async UniTask SetTextureAsync(long id)
        {
            string url = GetURL(id);
            await SetTextureAsync(url);
        }
    }
}