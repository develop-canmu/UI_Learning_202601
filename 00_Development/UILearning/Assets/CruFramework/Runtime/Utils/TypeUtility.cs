using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;

namespace CruFramework
{
    public static class TypeUtility
    {
        /// <summary>MethodInfo</summary>
        public static FieldInfo GetField(Type type, string fieldName)
        {
            while(true)
            {
                if(type == typeof(object) || type == null)break;
                
                FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if(field != null)return field;
                type = type.BaseType;
            }
            return null;
        }

        /// <summary>MethodInfo</summary>
        public static MethodInfo GetMethod(Type type, string methodName)
        {
            while(true)
            {
                if(type == typeof(object) || type == null)break;
                
                MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if(method != null)return method;
                type = type.BaseType;
            }
            return null;
        }
        
        /// <summary>すべてのメソッドを取得</summary>
        public static MethodInfo[] GetMethods(Type type)
        {
            List<MethodInfo> result = new List<MethodInfo>();
            
            while(true)
            {
                if(type == typeof(object) || type == null)break;
                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                result.AddRange(methods);
                type = type.BaseType;
            }
            
            return result.ToArray();
        }
        
        public static bool IsPrimitive(Type type)
        {
            if(type.IsPrimitive)return true;
            if(type == typeof(string))return true;
            if(type.IsEnum)return true;
            return false;
        }
    }
}