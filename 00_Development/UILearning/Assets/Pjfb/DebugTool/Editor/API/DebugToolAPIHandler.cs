using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using CruFramework.UI;
using Pjfb.Networking.API;
using Pjfb.Networking.App;

namespace Pjfb.Editor
{
    public class DebugToolAPIHandler : PjfbAPIHandler
    {
        public class ApiResultData
        {
            private string name = string.Empty;
            /// <summary>名前</summary>
            public string Name{get{return name;}}
            
            private string json = string.Empty;
            /// <summary>結果のJson</summary>
            public string Json{get{return json;}}
            
            private string post = string.Empty;
            /// <summary>結果のJson</summary>
            public string Post{get{return post;}}
            
            private Type responseType = null;
            /// <summary>レスポンスタイプ</summary>
            public Type ResponseType{get{return responseType;}}
            
            private DebugToolClassView classView = new DebugToolClassView();
            /// <summary>クラス表示よう</summary>
            public DebugToolClassView ClassView{get{return classView;}}
            
            public ApiResultData(string name, string post, string json, Type responseType)
            {
                this.name = name;
                this.json = json;
                this.post = post;
                this.responseType = responseType;
            }
        }
        
        private List<ApiResultData> resultList = new List<ApiResultData>();
        /// <summary>結果リスト</summary>
        public List<ApiResultData> ResultList{get{return resultList;}}
        
        /// <summary>リスト初期化</summary>
        public void Clear()
        {
            resultList.Clear();
        }
        
        protected override void OnBeginConnect(IAPIRequest request)
        {

        }

        protected override void OnFinishConnect(IAPIRequest request, bool isError)
        {
            // Post
            string post = JsonUtility.ToJson(request.GetRootPostData());
            // Response
            string json = System.Text.Encoding.UTF8.GetString(request.rawResponseData);
            // レスポンスタイプ
            Type responseType = request.GetResponseType();
            // リストに追加
            resultList.Add( new ApiResultData(request.apiName, DebugToolUtility.AlignmentJson(post), DebugToolUtility.AlignmentJson(json), responseType) );
        }
    }
}