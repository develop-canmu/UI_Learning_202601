using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Training;

namespace Pjfb.Editor
{
    
    public abstract class DebugPage
    {
        public abstract void SetPageObject(CruFramework.Page.Page page);
    }
    
    public abstract class DebugPage<T> : DebugPage where T : CruFramework.Page.Page
    {
        private T pageObject = null;
        /// <summary>ページオブジェクト</summary>
        public T PageObject{get{return (T)pageObject;}}
        
        public TView Get<TView>(string name)
        {
            FieldInfo field = pageObject.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if(field != null)
            {
                return (TView)field.GetValue(pageObject);
            }
            return default;
        }

        public override sealed void SetPageObject(CruFramework.Page.Page page)
        {
            pageObject = (T)page;
        }
    }
}