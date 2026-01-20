using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CruFramework.UI;
using Cysharp.Threading.Tasks;
using Pjfb.Networking.API;


namespace Pjfb.Editor
{
    public class DebugToolApiConnector : IAPIConnecter
    {
        
        private IAPIConnecter defaultConnector = new APIConnecter();
        
        
        private DebugToolWindow editor = null;
        
        public void SetEditor(DebugToolWindow editor)
        {
            this.editor = editor;
        }
        
        async UniTask<APIResult> IAPIConnecter.Connect(string baseURL, IAPIRequest request )
        {
            
            // 固定レスポンスがあるか探す
            foreach(DebugToolResponseSaveData response in editor.SaveData.ResponseList)
            {
                if(response.EnableResponse && response.Name == request.apiName)
                {
                    // 強制的にエラーを出す
                    if(response.IsForceError)
                    {
                        throw new APIException(APIResultCode.APIError, 0, request, "デバッグツールによる強制エラー");
                    }
                    string json = response.Json;
                    return new APIResult(request, System.Text.Encoding.UTF8.GetBytes(json));
                }
            }
            
            return await defaultConnector.Connect(baseURL, request);
        }
        
        public void Abort( IAPIRequest request )
        {
            defaultConnector.Abort(request);
        }
    }
}