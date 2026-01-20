using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CruFramework.Addressables;
using CruFramework.ResourceManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UI;

namespace Pjfb
{
    public abstract class CancellableImageBase : CancellableResource
    {
        /// <summary>画像セット</summary>
        public void SetTexture(string key)
        {
            SetTextureAsync(key, PageResourceLoadUtility.resourcesLoader).Forget();
        }
        
        /// <summary>画像セット</summary>
        public void SetTexture(string key, ResourcesLoader resourcesLoader)
        {
            SetTextureAsync(key, resourcesLoader).Forget();
        }
        
        /// <summary>画像セット</summary>
        public UniTask SetTextureAsync(string key)
        {
            return SetTextureAsync(key, PageResourceLoadUtility.resourcesLoader);
        }
        
        /// <summary>画像セット</summary>
        public abstract UniTask SetTextureAsync(string key, ResourcesLoader resourcesLoader);

        protected virtual void OnPreLoadTexture()
        {
        }
        
        protected virtual void OnPostLoadTexture()
        {
        }
        
        public abstract void SetActiveImage(bool value);
        
        public abstract void SetColor(Color color);
    }
}
