
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Networking.API;
using Pjfb.Networking.App.Request;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor;

namespace Pjfb.Editor
{
    public class DebugToolClassView
    {
        // Jsonシリアライズよう
        private abstract class SerializeJsonData
        {
            public long code;
            public abstract IResponseData GetData();
        }
        
        // Jsonシリアライズよう
        [System.Serializable]
        private class SerializeJsonData<T> : SerializeJsonData where T : IResponseData
        {
            
            public T data;
            public string dateTime;
            public bool isEncrypted;
            
            public override IResponseData GetData()
            {
                return data;
            }
        }
        
        private class SerializeData : ScriptableObject
        {
            [SerializeReference]
            public IResponseData response = null;
        }
        
        private Type classType = null;
        /// <summary>タイプ</summary>
        public Type ClassType{get{return classType;}}
        
        // Json
        private string json = string.Empty;
        
        // ルートのオブジェクト
        private SerializedObject serializeRootData = null;
        
        private SerializeData　serializeData = null;
        
        // Json
        private SerializeJsonData jsonData = null;
        // Serialize用
        private SerializedProperty serializedObject = null;
        // 表示するフィールド
        private SerializedProperty[] properties = null;
        
        /// <summary>Jsonをセット</summary>
        public void SetJson(Type classType, string json, bool forceUpdate = false)
        {
            // 同じデータ
            if(forceUpdate == false && this.json == json && this.classType == classType)return;
            // クラスタイプ
            this.classType = classType;
            // Jsonコピー
            this.json = json;
            // インスタンスの生成
            serializeData = ScriptableObject.CreateInstance<SerializeData>();
            // Jsonのシリアライズ
            Type serializeDataType = typeof(SerializeJsonData<>).MakeGenericType(classType);
            jsonData = ((SerializeJsonData)JsonUtility.FromJson(json, serializeDataType));
            serializeData.response = jsonData.GetData();
            // シリアライズよう
            serializeRootData = new SerializedObject(serializeData);
            serializedObject = serializeRootData.FindProperty("response");
            // 表示するフィールド
            FieldInfo[] fields = classType.GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            properties = new SerializedProperty[fields.Length];
            // フィールドのプロパティを取得
            for(int i=0;i<fields.Length;i++)
            {
                properties[i] = serializedObject.FindPropertyRelative(fields[i].Name);
            }
        }
        
        public string DrawGUI(bool errorCode, bool isReadOnly)
        {
            if(isReadOnly)
            {
                EditorGUI.BeginDisabledGroup(true);
            }
            
            serializeRootData.Update();
            
            // 変更チェック
            EditorGUI.BeginChangeCheck();
            
            // エラーコード
            if(errorCode)
            {
                jsonData.code = EditorGUILayout.LongField("code", jsonData.code);
            }
            
            // プロパティ表示
            foreach(SerializedProperty p in properties)
            {
                EditorGUILayout.PropertyField(p, true);
            }
            // 変更チェック
            if(EditorGUI.EndChangeCheck())
            {
                serializeRootData.ApplyModifiedProperties();
                json = DebugToolUtility.AlignmentJson( JsonUtility.ToJson(jsonData) );

            }
            
            if(isReadOnly)
            {
                EditorGUI.EndDisabledGroup();
            }

            return json;
        }
    }
}