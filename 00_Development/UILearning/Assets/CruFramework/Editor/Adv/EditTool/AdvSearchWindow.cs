using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Reflection;
using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace CruFramework.Editor.Adv
{
    public abstract class AdvSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        // ツリー構造データ
        private class TreeData
        {
            // 表示名
            public string name = string.Empty;
            // タイプ
            public object value = null;
            // 子改装
            public Dictionary<string, TreeData> children = new Dictionary<string, TreeData>();
        }
        
        /// <summary>名前</summary>
        protected abstract string TreeName{get;}
        /// <summary>パスと中身を取得</summary>
        protected abstract Dictionary<string, object> GetTreeDatas();
        /// <summary>選択した</summary>
        protected abstract void OnSelectEntry(object value, SearchWindowContext context);

        /// <summary>表示ツリーを生成</summary>
        List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
        {
            // ツリーデータ
            List<SearchTreeEntry> entries = new List<SearchTreeEntry>();
            // ルートグループ
            entries.Add(new SearchTreeGroupEntry(new GUIContent(TreeName)));
            // ツリーデータ
            TreeData treeData = new TreeData();
            
            // ツリーデータを取得
            Dictionary<string, object> treeDatas = GetTreeDatas();

            foreach(KeyValuePair<string, object> data in treeDatas)
            {
                // パス
                string[] paths = data.Key.Split('/');
                // ツリー
                TreeData tree = treeData;
                // パスごとにデータを作る
                foreach(string path in paths)
                {
                    // 子
                    if(tree.children.TryGetValue(path, out TreeData child) == false)
                    {
                        child = new TreeData();
                        tree.children.Add(path, child);
                    }
                    // 名前
                    child.name = path;
                    // 子供に
                    tree = child;
                }
                // 値
                tree.value = data.Value;
            }
            
            foreach(KeyValuePair<string, TreeData> child in treeData.children)
            {
                AddEntries(child.Value, 1, entries);
            }
            return entries;
        }
        
        private void AddEntries(TreeData treeData, int level, List<SearchTreeEntry> entries)
        {
            // メニューの追加
            if(treeData.value != null)
            {
                entries.Add(new SearchTreeEntry(new GUIContent(treeData.name)) { level = level, userData = treeData.value });
                return;
            }
            
            // グループの追加
            entries.Add(new SearchTreeGroupEntry(new GUIContent(treeData.name)) { level = level});

            foreach(KeyValuePair<string, TreeData> child in treeData.children)
            {
                AddEntries(child.Value, level + 1, entries);
            }
        }
        
        

        bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            OnSelectEntry(searchTreeEntry.userData, context);
            return true;
        }
    }

}