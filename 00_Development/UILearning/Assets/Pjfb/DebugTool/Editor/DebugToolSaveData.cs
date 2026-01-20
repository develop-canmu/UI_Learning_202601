using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;

namespace Pjfb.Editor
{
    [System.Serializable]
    public class DebugToolSaveData
    {
        [SerializeField]
        private List<string> openApiNames = new List<string>();
        [SerializeField]
        private List<string> openResponseNames = new List<string>();
        [SerializeField]
        private List<string> openPostNames = new List<string>();
        
        [SerializeField]
        private List<DebugToolResponseSaveData> responseList = new List<DebugToolResponseSaveData>();
        /// <summary>レスポンス</summary>
        public List<DebugToolResponseSaveData> ResponseList{get{return responseList;}}
        
        [SerializeField]
        private DebugToolApi.ViewMode apiViewMode = DebugToolApi.ViewMode.Class;
        /// <summary>表示モード</summary>
        public DebugToolApi.ViewMode ApiViewMode{get{return apiViewMode;}set{apiViewMode = value;}}
        
        /// <summary>Apiを開いた状態</summary>
        public bool IsOpenApi(string name)
        {
            return openApiNames.Contains(name);
        }
        
        /// <summary>Apiの開閉状態を設定</summary>
        public void SetOpenApi(string name, bool isOpen)
        {
            if(isOpen)
            {
                if(openApiNames.Contains(name) == false)
                {
                    openApiNames.Add(name);
                }
            }
            else
            {
                openApiNames.Remove(name);
            }
        }
        
        /// <summary>Responseを開いた状態</summary>
        public bool IsOpenResponse(string name)
        {
            return openResponseNames.Contains(name);
        }
        
        /// <summary>Responseの開閉状態を設定</summary>
        public void SetOpenResponse(string name, bool isOpen)
        {
            if(isOpen)
            {
                if(openResponseNames.Contains(name) == false)
                {
                    openResponseNames.Add(name);
                }
            }
            else
            {
                openResponseNames.Remove(name);
            }
        }
        
        /// <summary>Postを開いた状態</summary>
        public bool IsOpenPost(string name)
        {
            return openPostNames.Contains(name);
        }
        
        /// <summary>Postの開閉状態を設定</summary>
        public void SetOpenPost(string name, bool isOpen)
        {
            if(isOpen)
            {
                if(openPostNames.Contains(name) == false)
                {
                    openPostNames.Add(name);
                }
            }
            else
            {
                openPostNames.Remove(name);
            }
        }
    }
}