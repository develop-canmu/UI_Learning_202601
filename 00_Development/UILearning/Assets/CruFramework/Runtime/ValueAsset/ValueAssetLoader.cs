using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using CruFramework;
using CruFramework.Addressables;
using UnityEditor;

namespace CruFramework
{
    
    public abstract class ValueAssetLoader
    {
        public abstract object GetValue(string key);
        public abstract string[] GetKeyss();
        public abstract ValueAsset GetValueAsset();
    }
    
    [FrameworkDocument("Tools", nameof(ValueAssetLoader), "ValueAssetの取得クラス。オーバーライドして使用する")]
    public abstract class ValueAssetLoader<TLoader, T, TValue> : ValueAssetLoader where T : ValueAsset<T, TValue>, new() where TLoader : ValueAssetLoader<TLoader, T, TValue>, new()
    {
        
        private static TLoader instance = new TLoader();
        /// <summary>インスタンス</summary>
        public static TLoader Instance{get{return instance;}}
        
        
        
        public ValueAssetLoader()
        {
            T asset = GetAsset();
            if(asset == null)return;
            // キーバリュー用のデータ作る
            foreach(var value in asset.Values)
            {
#if UNITY_EDITOR
                if(values.ContainsKey(value.Key))
                {
                    Logger.LogError($"{asset.GetType().Name}のKeyが重複しています。key = {value.Key}", asset);
                    continue;
                }
#endif
                values.Add(value.Key, value.Value);
            }
        }
        
        // デフォルト値
        private TValue defaultValue = default(TValue);
        // キーバニュー
        private Dictionary<string, TValue> values = new Dictionary<string, TValue>();

        public int ValueCount{get{return values.Count;}}
        
        /// <summary>Idがあるか</summary>
        [FrameworkDocument("Idがあるか調べる")]
        public bool HasKey(string key)
        {
            return values.ContainsKey(key);
        }
        
        /// <summary>値の取得</summary>
        public TValue this[string key]
        {
            get
            {
                if(values.TryGetValue(key, out TValue result))
                {
                    return result;
                }
                return defaultValue;
            }
        }

        public override string[] GetKeyss()
        {
            return values.Keys.ToArray();
        }
        
        [FrameworkDocument("値を取得する。ValueAssetLoader.Instance[key]でも取得可能")]
        public override object GetValue(string key)
        {
            values.TryGetValue(key, out TValue value);
            return value;
        }

        protected abstract string GetAddress();
        
        private T GetAsset()
        {
#if UNITY_EDITOR
            string path = CruFramework.EditorExtension.Addressables.AddressablesUtility.GetAssetPathFromAddress(GetAddress());
            return AssetDatabase.LoadAssetAtPath<T>(path);
#else
            return AddressablesManager.LoadAsset<T>(GetAddress()).Result;
#endif
        }

        public sealed override ValueAsset GetValueAsset()
        {
            return GetAsset();
        }
        
        /// <summary>上書き</summary>
        public void OverwriteValues(T asset)
        {
            // キーバリュー用のデータ作る
            foreach(var value in asset.Values)
            {
                if(values.ContainsKey(value.Key))
                {
                    values[value.Key] = value.Value;
                }
                else
                {
                    values.Add(value.Key, value.Value);
                }
            }
        }
    }
}