using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace CruFramework.Page
{
    [FrameworkDocument("Page", nameof(ModalWindow), "モーダル表示用クラス。(現)OnPreClose > (次)OnPreOpen > (次)OnOpen")]
    public abstract class ModalWindow : MonoBehaviour
    {
        
        private ModalManager modalManager = null;
        /// <summary>マネージャー</summary>
        public ModalManager Manager{get{return modalManager;}internal set{modalManager=value;}}
        
        private int modalId = 0;
        /// <summary>Id</summary>
        public int ModalId{get{return modalId;}}
        
        private object modalArguments = null;
        /// <summary>引数</summary>
        public object ModalArguments{get{return modalArguments;}}
        
        private ModalOptions options = ModalOptions.None;
        /// <summary>オプション</summary>
        public  ModalOptions Options{get{return options;}internal set{options = value;}}
        
        // 閉じたときに返すパラメータ
        private object closedParameter = null;
        
        /// <summary>Id</summary>
        internal void SetId(int id)
        {
            modalId = id;
        }
        
        internal void SetArguments(object args)
        {
            modalArguments = args;
        }
        
        [FrameworkDocument("モーダルが開かれたときに呼ばれる")]
        protected virtual UniTask OnOpen(CancellationToken token)
        {
            return default;
        }
        
        [FrameworkDocument("モーダルが開く前の処理")]
        protected virtual UniTask OnPreOpen(object args, CancellationToken token)
        {
            return default;
        }
        
        [FrameworkDocument("モーダルを閉じる前の処理")]
        protected virtual UniTask OnPreClose(CancellationToken token)
        {
            return default;
        }

        internal async UniTask OnActiveInternal(CancellationToken token)
        {
            await OnOpen(token);
        }

        internal async UniTask OnPreOpenInternal(object args, CancellationToken token)
        {
            await OnPreOpen(args, token);
        }
        
        internal async UniTask OnCloseInternal(CancellationToken token)
        {
            await OnPreClose(token);
            
        }
        
        /// <summary>閉じるときに渡すパラメータをセット</summary>
        public void SetCloseParameter(object value)
        {
            closedParameter = value;
        }
        
        /// <summary>閉じる</summary>
        [FrameworkDocument("モーダルを閉じる")]
        public void Close(Action onCompleted)
        {
            modalManager.OnCloseModal(this, onCompleted);
        }
        
        /// <summary>閉じる</summary>
        public void Close()
        {
            modalManager.OnCloseModal(this, null);
        }
        
        /// <summary>閉じる</summary>
        public async UniTask<object> CloseAsync()
        {
            await modalManager.OnCloseModalAsync(this);
            return closedParameter;
        }
        
        /// <summary>閉じた</summary>
        public bool IsClosed()
        {
            return this == null;
        }
        
        /// <summary>閉じるまで待機</summary>
        public async UniTask<object> WaitCloseAsync(CancellationToken token = default)
        {
            await UniTask.WaitWhile(()=>this != null, cancellationToken: token);
            return closedParameter;
        }

        /// <summary>エラー通知</summary>
        protected virtual void OnError(Exception e)
        {
            
        }

        internal void OnErrorInternal(Exception e)
        {
            OnError(e);
        }
    }
}