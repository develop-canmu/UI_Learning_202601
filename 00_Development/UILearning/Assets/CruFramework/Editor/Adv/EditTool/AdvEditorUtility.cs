using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace CruFramework.Editor.Adv
{

    public static class AdvEditorUtility
    {
        public static FieldInfo GetField(Type type, string name)
        {
            while(true)
            {
                if(type == null || type == typeof(object))break;
                FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if(field != null)return field;
                type = type.BaseType;
            }
            
            return null;
        }
    }
}
