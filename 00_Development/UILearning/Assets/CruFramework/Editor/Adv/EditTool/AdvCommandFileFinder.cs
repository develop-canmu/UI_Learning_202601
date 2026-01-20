using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using CruFramework.Adv;
using UnityEngine;
using CruFramework.UI;
using UnityEditor;

namespace CruFramework.Editor.Adv
{
    public class AdvCommandFileFinder : EditorWindow
    {
        public static void Open(string directory)
        {
            AdvCommandFileFinder w = GetWindow<AdvCommandFileFinder>();
            w.findDirectory = directory;
        }
        
        
        private class FindResultData
        {
            public UnityEngine.Object file = null;
            public string nodeIds = string.Empty;

            public FindResultData(UnityEngine.Object file, ulong[] nodeIds)
            { 
                this.file = file;
                this.nodeIds = string.Join(", ", nodeIds);
            }
        }
        
        // ファイルのディレクトリ
        private string findDirectory = string.Empty;
        
        // スクロール
        private Vector2 scrollValue = Vector2.zero;
        
        // コマンドタイプ
        private List<Type> commandTypes = new List<Type>();
        // タイプ名（メニュー表示用）
        private string[] typeNames = null;
        // メニューIndex
        private int[] typeIndex = null;
        // 選択中メニュー
        private int selectIndex = 0;
        
        // 検索対象文字
        private string targetString = string.Empty;
        
        // 結果
        private List<FindResultData> resultDatas = new List<FindResultData>();
        
        private void OnEnable()
        {
            // 対象のタイプを取得
            commandTypes.Clear();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach(Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach(Type type in types)
                {
                    if(type.IsAbstract)continue;
                    if(type.IsSubclassOf(typeof(AdvCommandNode)))
                    { 
                        commandTypes.Add(type);
                    }
                }
            }
            
            // メニューに必要なデータを作成
            typeNames = new string[commandTypes.Count];
            typeIndex = new int[commandTypes.Count];
            for(int i=0;i<commandTypes.Count;i++)
            {
                typeIndex[i] = i;
                typeNames[i] = commandTypes[i].FullName.Replace(".", "/");
                typeNames[i] = typeNames[i].Replace("/Editor/Adv", string.Empty);
                typeNames[i] = typeNames[i].Replace("/AdvGraphNode", "/");
            }
        }
        
        // コマンドを探す
        private void FindCommand()
        {
            resultDatas.Clear();
            
            // ファイルを取得
            string[] files = Directory.GetFiles(findDirectory, "*.adv", SearchOption.AllDirectories);
            
            Type targetType = commandTypes[selectIndex];
            List<ulong> nodeIds = new List<ulong>();
            
            foreach(string file in files)
            {
                // ファイル読み込み
                string json = File.ReadAllText(file);
                // データシリアライズ
                AdvEditorSaveData saveData = JsonUtility.FromJson<AdvEditorSaveData>(json);
                
                nodeIds.Clear();
                // 対象のコマンドを探す
                foreach(AdvCommandNode node in saveData.commandNodes)
                {
                    if(node.GetType() == targetType)
                    {
                        nodeIds.Add(node.NodeId);
                    }
                }
                // 一つでも見つかった場合はリストに追加
                if(nodeIds.Count > 0)
                {
                    resultDatas.Add( new FindResultData( AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(file), nodeIds.ToArray() ) );
                }
            }
        }
        
        // コマンドを探す
        private void FindMessage()
        {
            resultDatas.Clear();
            
            // ファイルを取得
            string[] files = Directory.GetFiles(findDirectory, "*.adv", SearchOption.AllDirectories);
            
            Type targetType = commandTypes[selectIndex];
            List<ulong> nodeIds = new List<ulong>();
            
            StringBuilder sb = new StringBuilder();

            foreach(string file in files)
            {
                // ファイル読み込み
                string json = File.ReadAllText(file);
                // データシリアライズ
                AdvEditorSaveData saveData = JsonUtility.FromJson<AdvEditorSaveData>(json);
                
                nodeIds.Clear();
                // 対象のコマンドを探す
                foreach(AdvCommandNode node in saveData.commandNodes)
                {
                    if(node is AdvGraphNodeMessage msg)
                    {
                        if(msg.Command.Message.Contains(targetString))
                        {
                           nodeIds.Add(node.NodeId);
                        }
                    }
                }
                // 一つでも見つかった場合はリストに追加
                if(nodeIds.Count > 0)
                {
                    resultDatas.Add( new FindResultData( AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(file), nodeIds.ToArray() ) );
                    sb.AppendLine(Path.GetFileNameWithoutExtension(file));
                }
            }
            
            Debug.Log(sb);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("コマンド検索");
            EditorGUILayout.BeginHorizontal();
            {
                selectIndex = EditorGUILayout.IntPopup("Target", selectIndex, typeNames, typeIndex);
                if(GUILayout.Button("Find", GUILayout.Width(60.0f)))
                {
                    FindCommand();
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.LabelField("メッセージ検索");
            EditorGUILayout.BeginHorizontal();
            {
                targetString = EditorGUILayout.TextField("Target", targetString);
                if(GUILayout.Button("Find", GUILayout.Width(60.0f)))
                {
                    FindMessage();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            scrollValue = EditorGUILayout.BeginScrollView(scrollValue);

            // 結果表示
            foreach(FindResultData result in resultDatas)
            {
                EditorGUILayout.ObjectField("File", result.file, typeof(UnityEngine.Object), false);
                EditorGUILayout.LabelField("NodeIds", result.nodeIds);                
                EditorGUILayout.Space();
            }
            
            EditorGUILayout.EndScrollView();
        }
    }
}