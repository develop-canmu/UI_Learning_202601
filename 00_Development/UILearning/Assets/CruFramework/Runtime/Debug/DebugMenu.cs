#if CRUFRAMEWORK_DEBUG && CRUFRAMEWORK_SRDEBUGGER_SUPPORT

using System;
using System.Collections;
using System.Collections.Generic;
using SRDebugger;
using UnityEngine;

namespace CruFramework
{
    public static class DebugMenu
    {
        private static Dictionary<string, Dictionary<string, OptionDefinition>> cache = new Dictionary<string, Dictionary<string, OptionDefinition>>();
        
        public static void Initialize()
        {
            if(SRDebug.IsInitialized) return;
            SRDebug.Init();
        }
        
        /// <summary>デバッグメニューを表示</summary>
        public static void Show(DefaultTabs tab = DefaultTabs.Options)
        {
            SRDebug.Instance.ShowDebugPanel(tab);
        }
        
        /// <summary>デバッグメニューを非表示</summary>
        public static void Hide()
        {
            SRDebug.Instance.HideDebugPanel();
        }
        
        /// <summary>メニューに追加</summary>
        public static void AddOption(string category, string name, Action callback, int sortPriority = 0)
        {
            // カテゴリ追加
            if(!cache.ContainsKey(category))
            {
                cache.Add(category, new Dictionary<string, OptionDefinition>());
            }
            
            // 既にメニューが登録済み
            if(cache[category].ContainsKey(name)) return;
            
            // デバッグオプション作成
            OptionDefinition debugOption = OptionDefinition.FromMethod(name, callback, category, sortPriority);
            // メニューに追加
            SRDebug.Instance.AddOption(debugOption);
            // キャッシュに追加
            cache[category].Add(name, debugOption);
        }
        
        /// <summary>メニューに追加</summary>
        public static void AddOption<T>(string category, string name, Func<T> getter, Action<T> setter, int sortPriority = 0)
        {
            // カテゴリ追加
            if(!cache.ContainsKey(category))
            {
                cache.Add(category, new Dictionary<string, OptionDefinition>());
            }
            
            // 既にメニューが登録済み
            if(cache[category].ContainsKey(name)) return;
            
            // デバッグオプション作成
            OptionDefinition debugOption = OptionDefinition.Create(name, getter, setter, category, sortPriority);
            // メニューに追加
            SRDebug.Instance.AddOption(debugOption);
            // キャッシュに追加
            cache[category].Add(name, debugOption);
        }
        
        /// <summary>メニューから削除</summary>
        public static void RemoveOption(string category)
        {
            // カテゴリがない
            if(!cache.ContainsKey(category)) return;
            // メニューから削除
            foreach (OptionDefinition option in cache[category].Values)
            {
                SRDebug.Instance.RemoveOption(option);
            }
            // キャッシュから削除
            cache[category].Clear();
        }
        
        /// <summary>メニューから削除</summary>
        public static void RemoveOption(string category, string name)
        {
            // カテゴリがない
            if(!cache.ContainsKey(category)) return;
            // メニューがない
            if(!cache[category].ContainsKey(name)) return;
            // メニューから削除
            SRDebug.Instance.RemoveOption(cache[category][name]);
            // キャッシュから削除
            cache[category].Remove(name);
        }
    }
}

#endif