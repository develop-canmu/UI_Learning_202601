using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Networking.API;
using UnityEditor;
using UnityEditor.AddressableAssets.Build.Layout;

namespace Pjfb.Editor
{
    public class DebugToolWindow : EditorWindow
    {
        /// <summary>保存先</summary>
        private static readonly string SaveDataPath = "PjfbTools/DebugTool";
        
        [MenuItem("Pjfb/DebugTool")]
        public static void Open()
        {
            GetWindow<DebugToolWindow>();
        }

        
        // 選択中のメニュー
        private int selectMenuIndex = 0;
        
        private DebugToolSaveData saveData = null;
        /// <summary>保存データ</summary>
        public DebugToolSaveData SaveData{get{return saveData;}}
        
        // 各メニュー
        private DebugToolMenuBase[] menus = null;
        // メニュー名
        private string[] menuNames = null;
        
        private Texture searchIcon = null;
        /// <summary>検索アイコン</summary>
        public Texture SearchIcon{get{return searchIcon;}}

        private void OnEnable()
        {
            searchIcon = EditorGUIUtility.IconContent("Search Icon").image;
            // データ読み込み
            LoadData();
            // メニューの生成
            CreateMenus();
            
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }
        
        private void OnPlayModeChanged(PlayModeStateChange state)
        {
            Save();
        }
        
        /// <summary>メニューの生成</summary>
        private void CreateMenus()
        {
            List<DebugToolMenuBase> menuList = new List<DebugToolMenuBase>();
            List<string> menuNameList = new List<string>();
            
            foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach(Type type in a.GetTypes())
                {
                    if(type.IsSubclassOf(typeof(DebugToolMenuBase)))
                    {
                        // インスタンス
                        DebugToolMenuBase m = (DebugToolMenuBase)System.Activator.CreateInstance(type);
                        // エディタ登録
                        m.SetEditor(this);
                        // 初期化
                        m.OnInitialize();
                        // リストに追加
                        menuList.Add(m);
                        menuNameList.Add(m.GetName());
                    }
                }
            }
            
            // 配列かして保持
            menuNames = menuNameList.ToArray();
            menus = menuList.ToArray();
        }
        
        /// <summary>保存</summary>
        public void Save()
        {
            // ディレクトリ
            string dir = Path.GetDirectoryName(SaveDataPath);
            // ディレクトリがない場合は作成
            if(Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }
            // Json
            string json = JsonUtility.ToJson(saveData);
            // 保存
            File.WriteAllText(SaveDataPath, json);
        }
        
        /// <summary>データの読み込み</summary>
        private void LoadData()
        {
            // 保存データ読み込み
            if(saveData == null)
            {
                if(File.Exists(SaveDataPath))
                {
                    saveData = JsonUtility.FromJson<DebugToolSaveData>(File.ReadAllText(SaveDataPath));
                }
                else
                {
                    saveData = new DebugToolSaveData();
                }
            }
        }


        private void Update()
        {
            // メニューごとに処理
            menus[selectMenuIndex].OnUpdate();
        }
        
        private void OnGUI()
        {
            // メニュー選択
            selectMenuIndex = GUILayout.Toolbar( selectMenuIndex, menuNames);
            // メニューごとに処理
            menus[selectMenuIndex].OnGUI();
        }
    }
}