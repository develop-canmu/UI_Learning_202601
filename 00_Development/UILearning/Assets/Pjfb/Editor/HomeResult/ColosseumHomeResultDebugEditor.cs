#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Pjfb.Colosseum;
using Pjfb.Master;
using Pjfb.Networking.App.Request;
using Pjfb.UserData;
using UnityEditor;

namespace Pjfb
{
    public class ColosseumHomeResultDebugEditor : EditorWindow
    {
        // 保存先
        private static readonly string SaveDataPath = "PjfbTools/HomeResultDebug";
        
        [MenuItem("Pjfb/ColosseumHomeResultDebug")]
        public static void Open()
        {
            GetWindow<ColosseumHomeResultDebugEditor>();
            
        }
        
        private void OnGUI()
        {
            serializedObject ??= new SerializedObject(this);
            serializedProperty = serializedObject.FindProperty("debugData");
            DrawGUI();
            
        }

        [Serializable]
        private class DebugData
        {
            [SerializeField] private ColosseumUserSeasonStatus[] seasonStatusList;
            public ColosseumUserSeasonStatus[] SeasonStatusList
            {
                get { return seasonStatusList; }
                set { seasonStatusList = value; }
            }
        }
        
        [SerializeField] private DebugData debugData;
        
        
        private ColosseumClientHandlingType handlingType = ColosseumClientHandlingType.PvP;
        
        private bool isOpenResultModal = false;
        private Vector2 scrollPosition = Vector2.zero;
        
        private SerializedObject serializedObject;
        private SerializedProperty serializedProperty;

        private void OnEnable()
        {
            LoadData();
        }


        private void DrawGUI()
        {
            serializedObject.Update();
            handlingType = (ColosseumClientHandlingType)EditorGUILayout.EnumPopup("再生するリザルト", handlingType);
            // 変更チェック
            EditorGUI.BeginChangeCheck();
            
            // リストは長いのでスクロール対応させる
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.PropertyField(serializedProperty, true);
            if (EditorGUI.EndChangeCheck())
            {
                // sColosseum関係のidは-1にする
                foreach (ColosseumUserSeasonStatus seasonStatus in debugData.SeasonStatusList)
                {
                    seasonStatus.sColosseumGroupStatusId = -1;
                    seasonStatus.sColosseumEventId = -1;
                    seasonStatus.groupSeasonStatus.sColosseumEventId = -1;
                }
                serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.EndScrollView();
            
            // ボタン周り
            if (UserDataManager.Instance.user.uMasterId == 0)
            {
                EditorGUILayout.LabelField("実行してログインしてください");
                isOpenResultModal = false;
                return;
            }
            
            if(isOpenResultModal == false)
            {
                if (GUILayout.Button("Play Result"))
                {
                    isOpenResultModal = true;
                    PlayResult();
                }
            }
            else
            {
                EditorGUILayout.LabelField("リザルト再生中...");
            }
        }
        
        // リザルトの再生処理
        private void PlayResult()
        {
            List<ColosseumUserSeasonStatus> userSeasonStatusList = new List<ColosseumUserSeasonStatus>();
            foreach (ColosseumUserSeasonStatus seasonStatus in debugData.SeasonStatusList)
            {
                // 再生するリザルトと同じものだけを取得,クラブマッチはmasterが別のため追加
                if(handlingType != ColosseumClientHandlingType.PvP && handlingType != MasterManager.Instance.colosseumEventMaster.FindData(seasonStatus.mColosseumEventId).clientHandlingType) continue;
                userSeasonStatusList.Add(seasonStatus);
            }

            // リストが空なら処理しない
            if (userSeasonStatusList.Count == 0)
            {
                CruFramework.Logger.LogError("not found mColosseumEventId");
                isOpenResultModal = false;
                return;
            }
            
            ColosseumManager.OpenColosseumResultModal(userSeasonStatusList.ToArray(), handlingType, OnFinishResult);
        }

        private void OnFinishResult()
        {
            isOpenResultModal = false;
        }
        
        
        private void OnDestroy()
        {
            // データの保存
            SaveData();
        }
        
        private void SaveData()
        {
            // ディレクトリ
            string directory = Path.GetDirectoryName(SaveDataPath);
            // ディレクトリがない場合は作成
            if(Directory.Exists(directory) == false)
            {
                Directory.CreateDirectory(directory);
            }
            // Json
            string json = JsonUtility.ToJson(debugData);
            // 保存
            File.WriteAllText(SaveDataPath, json);
        }
        
        /// <summary>データの読み込み</summary>
        private void LoadData()
        {
            // 保存データ読み込み
            if(debugData == null)
            {
                if(File.Exists(SaveDataPath))
                {
                    debugData = JsonUtility.FromJson<DebugData>(File.ReadAllText(SaveDataPath));
                }
                else
                {
                    debugData = new DebugData();
                }
            }
        }

    }
}
#endif