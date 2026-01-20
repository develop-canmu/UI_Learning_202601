using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using System;
using System.Linq;
using UnityEngine.Events;

namespace CruFramework.UI
{
    [FrameworkDocument("UI", nameof(UIDrop), "ドラッグ&ドロップ拡張コンポーネント")]
    public class UIDrop: MonoBehaviour, IDropHandler
    {
        [SerializeField]
        private UnityEvent onDrop = new UnityEvent();
        /// <summary>ドロップ時通知</summary>
        public UnityEvent OnDrop{get{return onDrop;}}
        
        void IDropHandler.OnDrop(PointerEventData e)
        {
            OnDrop.Invoke();
        }
    }
}