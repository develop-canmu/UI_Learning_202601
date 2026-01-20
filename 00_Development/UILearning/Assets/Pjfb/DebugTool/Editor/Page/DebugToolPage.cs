using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Editor;
using UnityEditor;
using System;
using System.Reflection;

namespace Pjfb.Editor
{
    public class DebugToolPage : DebugToolMenuBase
    {
        /// <summary> GUI表示Interface </summary>
        private interface IDebugMethodOnGUI
        {
            public void DrawGUI(CruFramework.Page.Page page);
        }
        
        /// <summary> Group化したメソッドの情報 </summary>
        private class CallMethodGroupData<T> : IDebugMethodOnGUI where T : IDebugMethodOnGUI
        {
            public T[] groupInfos;
            public string GroupName;
            private bool isFold = true;
            
            public CallMethodGroupData(string groupName, T[] groupInfos)
            {
                this.groupInfos = groupInfos;
                this.GroupName = groupName;
            }

            public void DrawGUI(CruFramework.Page.Page page)
            {
                EditorGUILayout.LabelField(GroupName, GUILayout.ExpandWidth(true));
                isFold = EditorGUILayout.BeginFoldoutHeaderGroup(isFold, string.Empty);
                if (isFold == false)
                {
                    foreach (var info in groupInfos)
                    {
                        info.DrawGUI(page);
                    }
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
        
        // 呼び出すメソッド情報
        private class CallMethodData : IDebugMethodOnGUI
        {
            // メソッド
            public MethodInfo methodInfo;
            public MethodCache methodCache;
            // 呼び出しオブジェクト
            public object parent;
            
            public CallMethodData(MethodInfo methodInfo, MethodCache methodCache, object parent)
            {
                this.methodInfo = methodInfo;
                this.methodCache = methodCache;
                this.parent = parent;
            }

            public void DrawGUI(CruFramework.Page.Page page)
            {
                DrawMethod(page);
            }
            
            private void DrawMethod(CruFramework.Page.Page page)
            {
                // 表示名
                string name = string.IsNullOrEmpty(methodCache.attribute.Name) ? methodInfo.Name : methodCache.attribute.Name;
            
                // メソッド名
                if(GUILayout.Button(name))
                {
                    if(parent is DebugPage debugPage)
                    {
                        debugPage.SetPageObject(page);
                    }
                    methodInfo.Invoke(parent, methodCache.arguments);
                }
                // インデント
                EditorGUI.indentLevel++;
                // 引数があれば
                for(int i=0;i<methodCache.parameters.Length;i++)
                {
                    ParameterInfo p = methodCache.parameters[i];
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(p.Name, GUILayout.ExpandWidth(true));
                    methodCache.arguments[i] = GetValueGUI(methodCache.arguments[i]);
                    EditorGUILayout.EndHorizontal();
                }
            
                // インデント
                EditorGUI.indentLevel--;
            
                // 説明
                if (methodCache.attribute.Message != string.Empty)
                {
                    EditorGUILayout.TextArea(methodCache.attribute.Message);
                }

                EditorGUILayout.Space();
            }
        }
        
        // ページタイプごとのキャッシュ
        private class TypeCache
        {
            // 対象のメソッド
            public IDebugMethodOnGUI[] callMethods = null;
            // エディタ側のクラス
            public List<DebugPage> debugPages = new List<DebugPage>();
        }
        
        // メソッドごとのキャッシュ
        private class MethodCache
        {
            // パラメータ
            public ParameterInfo[] parameters = null;
            // 引数
            public object[] arguments = null;
            // アトリビュート
            public PageDebugAttribute attribute = null;
        }
        
        // ページタイプごとのキャッシュ
        private Dictionary<Type, TypeCache> typeCaches = new Dictionary<Type, TypeCache>();
        // ページタイプごとのキャッシュ
        //private Dictionary<MethodInfo, MethodCache> methodCaches = new Dictionary<MethodInfo, MethodCache>();

        private Vector2 scrollPosition = Vector2.zero;
        
        public override string GetName()
        {
            return "Page";
        }

        public override void OnGUI()
        {
            if(AppManager.Instance == null || AppManager.Instance.UIManager.PageManager.CurrentPageObject == null)return;

            // スクロール対応
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            
            // ページの表示
            DrawPage(AppManager.Instance.UIManager.PageManager.CurrentPageObject);

            GUILayout.EndScrollView();
        }
        
        
        private object GetDefaultValue(Type type)
        {
            if(type == typeof(int))return 0;
            if(type == typeof(long))return 0L;
            if(type == typeof(float))return 0.0f;
            if(type == typeof(string))return string.Empty;
            if(type == typeof(bool))return false;
            if (type.IsEnum) return default;
            
            return 0;
        }
        
        private static object GetValueGUI(object value)
        {
            switch (value)
            {
                case int v:
                    return EditorGUILayout.IntField(v);
                case long v:
                    return EditorGUILayout.LongField(v);
                case bool v:
                    return EditorGUILayout.Toggle(v);
                case float v:
                    return EditorGUILayout.FloatField(v);
                case string v:
                    return EditorGUILayout.TextField(v);
                case Enum v:
                    return EditorGUILayout.EnumPopup(v);
            }
            
            return value;
        }
        
        
        private MethodCache CreateMethodCache(MethodInfo m)
        {
            // 属性
            PageDebugAttribute attribute = m.GetCustomAttribute<PageDebugAttribute>();
            // 属性なし
            if(attribute == null)return null;
            // 対応する引数か
            ParameterInfo[] parameters = m.GetParameters();
                
            bool isCheckParam = true;
            // タイプをチェック
            foreach(ParameterInfo p in parameters)
            {
                if(
                    p.ParameterType != typeof(int) && 
                    p.ParameterType != typeof(long) && 
                    p.ParameterType != typeof(bool) && 
                    p.ParameterType != typeof(float) && 
                    p.ParameterType != typeof(string) && 
                    p.ParameterType.IsEnum == false
                )
                {
                    isCheckParam = false;
                    break;
                }
            }
            if(isCheckParam == false)return null;
            
            // メソッドのキャッシュ
            MethodCache methodCache = new MethodCache();
            // 引数
            methodCache.parameters = parameters;
            methodCache.arguments = new object[parameters.Length];
            methodCache.attribute = attribute;
                
            // 引数初期化
            for(int i=0;i<parameters.Length;i++)
            {
                // デフォルト引数があるならその値でセット
                if(parameters[i].HasDefaultValue)
                {
                    methodCache.arguments[i] = parameters[i].DefaultValue;
                }
                else
                {
                    methodCache.arguments[i] = GetDefaultValue(parameters[i].ParameterType);
                }
            }
            
            return methodCache;
        }
        
        private IDebugMethodOnGUI[] GetTargetMethods(CruFramework.Page.Page page)
        {
            Type type = page.GetType();
            
            if(typeCaches.TryGetValue(type, out TypeCache cache))
            {
                return cache.callMethods;
            }
            
            cache = new TypeCache();
            
            // 呼び出すメソッド
            List<CallMethodData> methodInfos = new List<CallMethodData>();
            
            try
            {
                // デバッグページを探す
                Type debugPageType = typeof(DebugPage<>).MakeGenericType(type);
                foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach(Type t in assembly.GetTypes())
                    {
                        if(t.IsSubclassOf(debugPageType))
                        {
                            cache.debugPages.Add( (DebugPage)System.Activator.CreateInstance(t) );
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            
            
            
            // エディタ側のチェック
            foreach(DebugPage debugPage in cache.debugPages)
            {
                foreach(MethodInfo m in debugPage.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) )
                {
                    MethodCache c = CreateMethodCache(m);
                    if(c != null)
                    {
                        methodInfos.Add( new CallMethodData(m, c, debugPage) );
                    }
                }
            }
            
            // ページ側のチェック
            foreach(MethodInfo m in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                MethodCache c = CreateMethodCache(m);
                if(c != null)
                {
                    methodInfos.Add( new CallMethodData(m, c, page) );
                }
            }

            Dictionary<string, List<CallMethodData>> categoryMethodList = new Dictionary<string, List<CallMethodData>>();
            
            // Groupアトリビュート毎に分類
            foreach (var method in methodInfos)
            {
                string categoryName = string.Empty;
                if (method.methodCache.attribute is PageDebugCategoryAttribute categoryAttribute)
                {
                    categoryName = categoryAttribute.CategoryName;
                }
                
                if (categoryMethodList.TryGetValue(categoryName, out List<CallMethodData> methodInfoList) == false)
                {
                    methodInfoList = new List<CallMethodData>();
                    categoryMethodList.Add(categoryName, methodInfoList);
                }
                methodInfoList.Add(method);
            }

            // グループごとの描画クラスを構築
            List<IDebugMethodOnGUI> debugMethods = new List<IDebugMethodOnGUI>();
            
            foreach (KeyValuePair<string,List<CallMethodData>> category in categoryMethodList)
            {
                string categoryName = category.Key == string.Empty ? "デバック処理" : category.Key;
                debugMethods.Add(new CallMethodGroupData<CallMethodData>(categoryName, category.Value.ToArray()));
            }
            
            cache.callMethods = debugMethods.ToArray();
            typeCaches.Add(type, cache);
            return cache.callMethods;
        }
        
        private void DrawPage(CruFramework.Page.Page page)
        {
            Type pageType = page.GetType();
            // ページ名表示
            EditorGUILayout.LabelField(pageType.Name);
            
            // 対象のメソッドを取得
            IDebugMethodOnGUI[] methods = GetTargetMethods(page);
            
            EditorGUILayout.BeginVertical("TextArea");
            {
                foreach(IDebugMethodOnGUI m in methods)
                {
                    // インデント
                    EditorGUI.indentLevel++;
                    
                    EditorGUILayout.BeginVertical("TextArea");
                    m.DrawGUI(page);
                    EditorGUILayout.EndVertical();
                    // インデント
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUILayout.EndVertical();
            
            // 子ページを表示
            if(page is CruFramework.Page.PageManager manager && manager.CurrentPageObject != null)
            {
                DrawPage(manager.CurrentPageObject);
            }
        }
    }
}