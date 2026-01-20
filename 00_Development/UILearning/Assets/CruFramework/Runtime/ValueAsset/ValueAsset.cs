using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif

namespace CruFramework
{
    public abstract class ValueAsset : ScriptableObject
    {
        public abstract void AddValueData();
        public abstract void RemoveAtValueData(int index);
        public abstract string[] GetKeys();
        public abstract object GetValue(string key);
        public abstract void SetValue(string key, object value);
    }
    
    [FrameworkDocument("Tools", nameof(ValueAsset), "データの一元管理用クラス。オーバーライドして使用する")]
    public abstract class ValueAsset<T, TValue> : ValueAsset where T : ValueAsset<T, TValue>, new()
    {
        
        [System.Serializable]
        public class ValueData
        {
            public ValueData(string key, TValue value)
            {
                this.key = key;
                this.value = value;
            }
            
            [SerializeField]
            private string key = string.Empty;
            /// <summary>Key</summary>
            public string Key{get{return key;}set{key = value;}}
            
            [SerializeField]
            private TValue value = default(TValue);
            /// <summary>Id</summary>
            public TValue Value{get{return value;}set{this.value = value;}}
        }

        
        [SerializeField]
        private TValue defaultValue = default;
        /// <summary>デフォルト値</summary>
        public TValue DefaultValue{get{return defaultValue;}}
        
        [SerializeField]
        private List<ValueData> values = new List<ValueData>();
        /// <summary></summary>
        public IReadOnlyList<ValueData> Values{get{return values;}}
        
        
        public void AddValueData(string key, TValue value)
        {
            values.Add( new ValueData(key, value) );
        }
        
        public override void AddValueData()
        {
            values.Add( new ValueData(string.Empty, default(TValue)) );
        }

        public override void RemoveAtValueData(int index)
        {
            values.RemoveAt(index);
        }

        public override string[] GetKeys()
        {
            return values.Select(v=>v.Key).ToArray();
        }

        public override object GetValue(string key)
        {
            foreach(ValueData value in values)
            {
                if(value.Key == key)return value.Value;
            }
            return null;
        }

        public override void SetValue(string key, object setValue)
        {
            foreach(ValueData value in values)
            {
                if(value.Key == key)
                {
                    value.Value = (TValue)setValue;
                    return;
                }
            }
            
            // 新しく追加
            values.Add( new ValueData(key, (TValue)setValue ) );
        }
    }
}