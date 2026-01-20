using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CruFramework.Editor
{
    public class AssetReferenceViewer : EditorWindow
    {
        [MenuItem("CruFramework/Tools/AssetReferenceViewer")]
        public static void Open()
        {
            GetWindow<AssetReferenceViewer>();
        }

        
        private class FindResultData
        {
            // 対象のアセット
            public UnityEngine.Object asset = null;
            // 参照しているアセット
            public UnityEngine.Object[] referenceAssets = null;
            // 参照されているアセット
            public UnityEngine.Object[] otherReferenceAssets = null;
        }
        
        // 検索結果
        private List<FindResultData> findResultList = new List<FindResultData>();        
        // 参照のキャッシュ
        private Dictionary<string, List<UnityEngine.Object>> referenceCache = new Dictionary<string, List<Object>>();
        // スクロール位置
        private Vector2 scrollValue = Vector2.zero;

        /// <summary>参照しているアセットを取得</summary>
        private void GetReference(string[] paths)
        {
            referenceCache.Clear();

            foreach(string path in paths)
            {
                referenceCache.Add(path, new List<Object>());
            }
            
            // すべてのアセットの参照をチェック
            string[] assetPaths = AssetDatabase.GetAllAssetPaths();
            int progress = 0;
            foreach(string assetPath in assetPaths)
            {
                if(EditorUtility.DisplayCancelableProgressBar("Find Reference", assetPath, (float)(progress++) / (float)assetPath.Length))
                {
                    break;
                }
                // 依存関係を取得
                string[] dependencies = AssetDatabase.GetDependencies(assetPath, true);
                // 依存関係にチェック対象があった場合
                foreach(string dependency in dependencies)
                {
                    // 自身は無視
                    if(dependency == assetPath)continue;
                    foreach(string path in paths)
                    {
                        if(dependency == path)
                        {
                            referenceCache[path].Add(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath));
                            break;
                        }
                    }
                }
            }
            
            EditorUtility.ClearProgressBar();
        }

        private void FindReference()
        {
            findResultList.Clear();
            
            List<string> paths = new List<string>();
            // パスを取得
            foreach(UnityEngine.Object target in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(target);
                if(string.IsNullOrEmpty(path))continue;
                paths.Add(path);
            }
            // 参照関係を取得
            GetReference(paths.ToArray());
            // 各アセットの参照を生成
            foreach(UnityEngine.Object target in Selection.objects)
            {
                FindReference(target);
            }            
        }

        private void FindReference(UnityEngine.Object target)
        {
            // パスを取得
            string targetPath = AssetDatabase.GetAssetPath(target);
            // パスなし
            if(string.IsNullOrEmpty(targetPath))return;
            // 参照しているパスを取得
            string[] targetDependencies = AssetDatabase.GetDependencies(targetPath, true);
            
            FindResultData dependenciesData = new FindResultData();
            dependenciesData.asset = target;
            List<UnityEngine.Object> dependenciesObject = new List<Object>();
            // 参照先のアセットを読み込み
            foreach(string path in targetDependencies)
            {
                UnityEngine.Object r = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                if(r == null || r == target)continue;
                dependenciesObject.Add(r);
            }            
            // 参照しているアセット
            dependenciesData.referenceAssets = dependenciesObject.ToArray();
            // 参照されているアセット
            dependenciesData.otherReferenceAssets = referenceCache[targetPath].ToArray();
            // リストに追加
            findResultList.Add(dependenciesData);
        }

        private void OnGUI()
        {
            if(GUILayout.Button("Find Reference"))
            {
                FindReference();
            }

            EditorGUILayout.Space();
            
            // スクロール
            scrollValue = EditorGUILayout.BeginScrollView(scrollValue);
            
            // 検索結果を表示
            foreach(FindResultData findResult in findResultList)
            {
                // 検索対象
                EditorGUILayout.ObjectField(findResult.asset, typeof(UnityEngine.Object), false);
                
                EditorGUILayout.LabelField("参照しているアセット");
                EditorGUI.indentLevel++;
                foreach(UnityEngine.Object asset in findResult.referenceAssets)
                {
                    EditorGUILayout.ObjectField(asset, typeof(UnityEngine.Object), false);
                }
                EditorGUI.indentLevel--;
                
                EditorGUILayout.LabelField("参照されているアセット");
                EditorGUI.indentLevel++;
                foreach(UnityEngine.Object asset in findResult.otherReferenceAssets)
                {
                    EditorGUILayout.ObjectField(asset, typeof(UnityEngine.Object), false);
                }
                EditorGUI.indentLevel--;
                
                EditorGUILayout.Space();
            }
            
            EditorGUILayout.EndScrollView();
        }
    }
}