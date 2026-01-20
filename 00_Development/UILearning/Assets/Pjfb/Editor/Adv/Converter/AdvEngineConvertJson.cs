using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using CruFramework.UI;
using Unity.VisualScripting;

namespace CruFramework.Editor.Adv.Convert
{
    public class AdvEngineConvertJson
    {
        
        private class ScopeData
        {
            public int valueCount = 0;
            public int elementCount = 0;
        }
        
        // 文字列連結用
        private StringBuilder json = new StringBuilder();
        // スコープデータ
        private Stack<ScopeData> scopeDatas = new Stack<ScopeData>();
        
        private ScopeData currentScope = new ScopeData();
        
        
        public AdvEngineConvertJson()
        {
        }
        
        private void AddComma()
        {
            // 値がすでにある場合はカンマ
            if(currentScope.valueCount > 0)
            {
                json.Append(",");
            }
        }
        
        /// <summary>オブジェクト追加</summary>
        public void AddObject(string name)
        {
            // カンマが必要ならつける
            AddComma();
            
            currentScope.valueCount++;
            
            if(string.IsNullOrEmpty(name))
            {
                json.Append("{");
            }
            else
            {
                json.Append($"\"{name}\":{{");
            }
            
            
            scopeDatas.Push(currentScope);
            currentScope = new ScopeData();
        }
        
        /// <summary>オブジェクト追加</summary>
        public void AddArrayObject(string name)
        {
            // カンマが必要ならつける
            AddComma();
            currentScope.valueCount++;
            json.Append($"\"{name}\":[");
            scopeDatas.Push(currentScope);
            currentScope = new ScopeData();
        }
        
        /// <summary>値追加</summary>
        public void AddValue(string name, object value)
        {
            if(value is object[] objectArray)
            {
                AddArrayObject(name);
                foreach(object v in objectArray)
                {
                    AddObject(string.Empty);
                    FieldInfo[] fields = v.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    
                    foreach(FieldInfo field in fields)
                    {
                        if(field.GetCustomAttribute<SerializeField>() == null)continue;
                        AddValue(field.Name, field.GetValue(v));
                    }
                    
                    EndObject();
                }
                EndArrayObject();
            }
            else if(value is Vector2 v2)
            {
                AddObject(name);
                AddValue("x", v2.x);
                AddValue("y", v2.y);
                EndObject();
            }
            else if(value is Vector3 v3)
            {
                AddObject(name);
                AddValue("x", v3.x);
                AddValue("y", v3.y);
                AddValue("z", v3.z);
                EndObject();
            }
            else if(value is Color c)
            {
                AddObject(name);
                AddValue("r", c.r);
                AddValue("g", c.g);
                AddValue("b", c.b);
                AddValue("a", c.a);
                EndObject();
            }
            else
            { 
                // カンマが必要ならつける
                AddComma();
            
                if(value is bool)
                {
                    json.Append($"\"{name}\":\"{value.ToString().ToLower()}\"");
                }
                else if(value is string str)
                {
                    string s = str.Replace("\r\n", "<br>");
                    s = s.Replace("\n", "<br>");
                    s = s.Replace("\"", "\\\"");
                    
                    if(s.Contains("application/vnd.unity.graphview.elements"))
                    {
                        Debug.LogError($" String Error [{name} {s}]");
                        s = string.Empty;
                    }
                    
                    json.Append($"\"{name}\":\"{s.TrimEnd()}\"");
                }
                else
                {
                    if(value == null)
                    {
                        Debug.Log(name);
                    }
                    json.Append($"\"{name}\":{value}");
                }
            }
            
            currentScope.valueCount++;
        }
        
        
        /// <summary>要素開始</summary>
        public void BeginElement()
        {
            if(currentScope.elementCount > 0)
            {
                json.Append(",");
            }
            json.Append("{");
            currentScope.elementCount++;
            currentScope.valueCount = 0;
        }
        
        /// <summary>要素終了</summary>
        public void EndElement()
        {
            json.Append("}");
        }
        
        /// <summary>オブジェクト追加</summary>
        public void EndObject()
        {
            json.Append("}");
            currentScope = scopeDatas.Pop();
        }
        
        /// <summary>オブジェクト追加</summary>
        public void EndArrayObject()
        {
            json.Append("]");
            currentScope = scopeDatas.Pop();
        }

        public override string ToString()
        {
            return "{" + json.ToString() + "}";
        }
    }
}