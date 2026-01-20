using System.Collections;
using System.Collections.Generic;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Pjfb
{
    public abstract class CancellableRawImageWithId : CancellableRawImage
    {
        /// <summary>キー取得</summary>
        protected abstract string GetKey(long id);
        
        /// <summary>画像セット</summary>
        public void SetTexture(long id)
        {
            SetTextureAsync(id).Forget();
        }
        
        /// <summary>画像セット</summary>
        public void SetTexture(long id, ResourcesLoader resourcesLoader)
        {
            SetTextureAsync(id, resourcesLoader).Forget();
        }

        /// <summary>画像セット</summary>
        public UniTask SetTextureAsync(long id)
        {
            return SetTextureAsync(id, PageResourceLoadUtility.resourcesLoader);
        }
        
        /// <summary>画像セット</summary>
        public virtual async UniTask SetTextureAsync(long id, ResourcesLoader resourcesLoader)
        {
            string key = GetKey(id);
            await SetTextureAsync(key, resourcesLoader);
        }
    }
}
