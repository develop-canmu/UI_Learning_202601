using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Pjfb
{
    public abstract class ItemIconBase : IconImageBase
    {
        /// <summary>Idの登録</summary>
        public async UniTask SetIconIdAsync(long id)
        {
            // id変更通知
            OnSetId(id);
            // テクスチャセット
            await SetIconTextureAsync(id);
        }
        
        /// <summary>Idの登録</summary>
        public void SetIconId(long id)
        {
            SetIconIdAsync(id).Forget();
        }

        /// <summary>テクスチャ更新</summary>
        protected virtual UniTask SetIconTextureAsync(long id)
        {
            return SetTextureAsync(id);
        }
        
        /// <summary>Id変更通知</summary>
        protected virtual void OnSetId(long id) 
        {
        }
    }
}