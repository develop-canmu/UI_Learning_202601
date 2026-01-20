using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;

namespace Pjfb
{
    public abstract class CancellableRawImageWithEnum<T> : CancellableRawImage where T : System.Enum
    {
        public void SetTexture(T type)
        {
            SetTextureAsync(type).Forget();
        }
        
        public virtual UniTask SetTextureAsync(T type)
        {
            return SetTextureAsync(GetAddress(type));
        }
        
        protected abstract string GetAddress(T type);
    }
}