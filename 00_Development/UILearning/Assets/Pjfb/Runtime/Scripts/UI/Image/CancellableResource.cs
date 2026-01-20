using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    public class CancellableResource : MonoBehaviour
    {
        protected CancellationTokenSource source = null;
        
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