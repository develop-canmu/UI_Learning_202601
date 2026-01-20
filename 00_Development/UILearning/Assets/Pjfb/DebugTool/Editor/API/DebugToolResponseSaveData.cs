
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Editor
{
    [System.Serializable]
    public class DebugToolResponseSaveData
    {
        [Flags]
        public enum OptionType
        {
            None = 0,
            UseResponse = 1 << 0,
            ForceError  = 1 << 1,
            EditCode     = 1 << 2, 
        }
        
        public DebugToolResponseSaveData(string name, string json, Type responseType)
        {
            this.name = name;
            this.json = json;
            this.responseTypeString = responseType.AssemblyQualifiedName;
        }
        
        [SerializeField]
        private OptionType options = OptionType.None;
        /// <summary>設定</summary>
        public OptionType Options{get{return options;}set{options = value;}}

        /// <summary>レスポンス固定</summary>
        public bool EnableResponse{get{return (options & OptionType.UseResponse) != OptionType.None;}}
        /// <summary>強制エラー</summary>
        public bool IsForceError{get{return (options & OptionType.ForceError) != OptionType.None;}}
        /// <summary>レスポンスコードを編集</summary>
        public bool CanEditCode{get{return (options & OptionType.EditCode) != OptionType.None;}}
        
        [SerializeField]
        private string json = string.Empty;
        /// <summary>レスポンスJson</summary>
        public string Json{get{return json;}set{json = value;}}
        
        [SerializeField]
        private string name = string.Empty;
        /// <summary>名前</summary>
        public string Name{get{return name;}set{name = value;}}
        
        [SerializeField]
        private string responseTypeString = string.Empty;
        
        private DebugToolClassView classView = null;
        /// <summary>クラス表示よう</summary>
        public DebugToolClassView ClassView
        {
            get
            {
                if(classView == null)
                {
                    classView = new DebugToolClassView();
                }
                return classView;
            }
        }
        
        private Type responseType = null;
        /// <summary>レスポンス</summary>
        public Type ResponseType
        {
            get
            {
                if(responseType == null)
                {
                    responseType = Type.GetType(responseTypeString);
                }
                return responseType;
                
            }
        }
    }
}