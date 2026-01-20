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
    public class PageDebugCategoryAttribute : PageDebugAttribute
    {
        private string categoryName = string.Empty;
        public string CategoryName => categoryName;
        
        public PageDebugCategoryAttribute(string categoryName)
        {
            this.categoryName = categoryName;
        }
        
        public PageDebugCategoryAttribute(string categoryName, string methodName) : base(methodName)
        {
            this.categoryName = categoryName;
        }
        
        public PageDebugCategoryAttribute(string categoryName, string methodName, string message) : base(methodName, message)
        {
            this.categoryName = categoryName;
        }
    }
}