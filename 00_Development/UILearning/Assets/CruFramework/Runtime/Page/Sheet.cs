using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace CruFramework.Page
{
    [FrameworkDocument("Page", nameof(Sheet), "シート表示用クラス。(現)OnPreClose > (現)OnClosed > (次)OnPreOpen > (次)OnOpned")]
    public abstract class Sheet : MonoBehaviour
    {
        private SheetManager manager = null;
        /// <summary>Switcher</summary>
        public SheetManager Manager{get{return manager;}internal set{manager = value;}}
        
        [FrameworkDocument("シートを開く前に呼ばれる")]
        protected virtual UniTask OnPreOpen(object args)
        {
            return default;
        }
        
        [FrameworkDocument("シートを閉じる前に呼ばれる")]
        protected virtual UniTask OnPreClose()
        {
            return default;
        }
        
        [FrameworkDocument("シートが開かれた後に呼ばれる")]
        protected virtual UniTask OnOpen(object args)
        {
            return default;
        }
        
        [FrameworkDocument("シートが開かれた後に呼ばれる")]
        protected virtual void OnOpened(object args)
        {
        }
        
        [FrameworkDocument("シートが閉じた後に呼ばれる")]
        protected virtual void OnClosed()
        {
        }
        
        internal UniTask OnOpenInternal(object args)
        {
            return OnOpen(args);
        }
        
        internal void OnOpenedInternal(object args)
        {
            OnOpened(args);
        }
        
        internal void OnClosedInternal()
        {
            OnClosed();
        }
        
        internal UniTask OnPreOpenInternal(object args)
        {
            return OnPreOpen(args);
        }
        
        internal UniTask OnPreCloseInternal()
        {
            return OnPreClose();
        }
    }
}