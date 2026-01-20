using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Method)]
    public class PageDebugAttribute : Attribute
    {
        
        protected string name = string.Empty;
        /// <summary>名前</summary>
        public string Name{get{return name;}}
        
        protected string message = string.Empty;
        /// <summary>表示するメッセージ</summary>
        public string Message{get{return message;}}
        
        
        public PageDebugAttribute()
        {
        }
        
        public PageDebugAttribute(string name)
        {
            this.name = name;
        }
        
        public PageDebugAttribute(string name, string message)
        {
            this.name = name;
            this.message = message;
        }
    }
}