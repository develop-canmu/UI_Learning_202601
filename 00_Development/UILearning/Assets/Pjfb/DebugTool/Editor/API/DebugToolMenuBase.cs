using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Editor
{
    public abstract class DebugToolMenuBase
    {
        
        public void SetEditor(DebugToolWindow editor)
        {
            this.editor = editor;
        }
        
        private DebugToolWindow editor = null;
        /// <summary>エディタ</summary>
        public DebugToolWindow Editor{get{return editor;}}
        
        /// <summary>保存データ</summary>
        public DebugToolSaveData SaveData{get{return editor.SaveData;}}
        
        /// <summary>初期化</summary>
        public virtual void OnInitialize(){}
        /// <summary>更新</summary>
        public virtual void OnUpdate(){}
        /// <summary>名前</summary>
        public abstract string GetName();
        /// <summary>表示処理</summary>
        public abstract void OnGUI();
    }
}