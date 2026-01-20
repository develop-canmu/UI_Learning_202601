using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CruFramework.Adv
{
    public abstract class AdvSelectRoot : MonoBehaviour
    {
        private AdvManager parentManager = null;
        /// <summary>マネージャー</summary>
        public AdvManager ParentManager{get{return parentManager;}internal set{parentManager = value;}}
        
        /// <summary>ボタン選択</summary>
        public abstract void Select(int index);
        /// <summary>選択肢の表示</summary>
        public abstract UniTask OpenAsync(AdvManager manager, string[] messages, int[] nos);
        /// <summary>閉じる</summary>
        public abstract void Close();
    }
}
