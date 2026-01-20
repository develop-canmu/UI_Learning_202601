using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CruFramework.Document
{ 
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public abstract class DocumentAttribute : Attribute
    {
        
        private string category = "Other";
        /// <summary>カテゴリ</summary>
        public string Category{get{return category;}}
        
        private string name = string.Empty;
        /// <summary>名前</summary>
        public string Name{get{return name;}}
        
        private string text = string.Empty;
        /// <summary>テキスト</summary>
        public string Text{get{return text;}}
        
        public DocumentAttribute()
        {
        }
        
        public DocumentAttribute(string category, string name, string text)
        {
            this.category = category;
            this.name = name;
            this.text = text.Replace("。", "。\n");
        }
        
        public DocumentAttribute(string name, string text)
        {
            this.name = name;
            this.text = text.Replace("。", "。\n");
        }
        
        public DocumentAttribute(string text)
        {
            this.text = text.Replace("。", "。\n");
        }
    }
}
