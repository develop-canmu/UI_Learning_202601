using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using CruFramework.Extensions;

namespace Pjfb.Networking.API {
    [CruFramework.FrameworkDocument("API", nameof( APIRequest<PostType, ResponseType>), "API接続用クラス")]

    public abstract class APIRequest<PostType, ResponseType> : IAPIRequest 
    where PostType : class, IPostData 
    where ResponseType : class, IResponseData, new()  {
        
        //APIの命名規則が不明なので一旦abstract
        public abstract string apiName{get;}  
        // public virtual string apiName{  
        //     get {
        //         return GetType().Name
        //         .Replace("Request", "")
        //         .CamelToSnake()
        //         .Replace("_", "/")
        //         .ToLower();
        //     }
        // }

        /// <summary>
        /// 暗号化を行うか
        /// </summary>
        public virtual bool isEncrypt{get; set;} = false;

        /// <summary>
        /// 暗号化キー
        /// </summary>
        /// <value></value>
        public string encryptKey{get; set;} = "";


        /// <summary>
        /// タイムアウト時間
        /// </summary>
        public int timeoutSecond{ get;set; } = 30;  //デフォルト

        /// <summary>
        /// ワンタイムトークン"user/generateOneTimeToken"で取得したtoken
        /// </summary>
        public string oneTimeToken{ get;set; } = "";

        /// <summary>
        /// エラーが発生したか
        /// </summary>
        public bool isError{get; private set;} = false; 

        /// <summary>
        /// リトライ回数
        /// </summary>
        public int retryCount{get; private set;} = 0; 

        /// <summary>
        /// 中止したか
        /// </summary>
        public bool isAbort{get; private set;} = false;

        /// <summary>
        /// byte状態のresponseを取得
        /// </summary>
        /// <value></value>
        public byte[] rawResponseData => _rawResponseData;

        /// <summary>
        /// ヘッダー
        /// </summary>
        public Dictionary<string,string> headers{get; private set;} = new Dictionary<string,string>();
     
        RootPostData<PostType> _rootPostData = new RootPostData<PostType>();
        byte[] _rawData = null;

        byte[] _rawResponseData = null;
        RootResponseData _rootResponseData = null;
        RootResponseData<ResponseType> _responseData = null;

        IAPISerializer<PostType, ResponseType> _serializer = null;

        protected abstract IAPISerializer<PostType, ResponseType> CreateSerializer();
        

        /// <summary>
        /// APIを受信した
        /// </summary>
        /// <param name="responseData"></param>
        protected virtual void OnAPIReceived( ResponseType responseData ) {

        }


        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="baseURL"></param>
        public APIRequest(){
            _serializer = CreateSerializer();
        }

        /// <summary>
        /// ヘッダー設定
        /// </summary>
        /// <param name="baseURL"></param>
        [CruFramework.FrameworkDocument("ヘッダーを設定します")]

        public void SetHeaders( Dictionary<string,string> headers ){
            this.headers = headers;
        }


        /// <summary>
        /// postDataの取得
        /// </summary>
        /// <returns></returns>
        [CruFramework.FrameworkDocument("設定済みのPostDataを取得します")]
        public PostType GetPostData(){
            return _rootPostData.param;
        }


        /// <summary>
        /// 送信データ設定
        /// </summary>
        /// <param name="baseURL"></param>
        [CruFramework.FrameworkDocument("PostDataを設定します")]
        public void SetPostData( PostType data ){
            _rootPostData.param = data;
        }

        /// <summary>
        /// 送信データ設定
        /// </summary>
        /// <param name="baseURL"></param>
        [CruFramework.FrameworkDocument("byte配列でPostDataを設定します")]
        public void SetRawPostData( byte[] data ){
            _rawData = data;
        }

        /// <summary>
        /// レスポンスデータ取得
        /// </summary>
        [CruFramework.FrameworkDocument("ResponseDataを取得します")]
        public ResponseType GetResponseData(){
            return _responseData.data;
        }


        /// <summary>
        /// リトライ回数加算
        /// </summary>
        [CruFramework.FrameworkDocument("APIManager管理用")] 
        public void Retry(){
            ++retryCount;
        }

        /// <summary>
        /// postDataの取得
        /// </summary>
        /// <returns></returns>
        public byte[] CreateRawPostData(){
            _rawData = _serializer.Serialize(_rootPostData);
            return _rawData;
        }
        

        /// <summary>
        /// 暗号化されたpostDataの取得
        /// </summary>
        /// <returns></returns>
        public byte[] CreateRawEncryptPostData(){
            _rawData = _serializer.SerializeWithCrypt(_rootPostData, encryptKey);
            return _rawData;
        }

        /// <summary>
        /// キャッシュされたポストデータの取得
        /// </summary>
        /// <returns></returns>
        public byte[] GetCacheRawPostData(){
            return _rawData;
        }

        /// <summary>
        /// ルートレスポンス取得
        /// </summary>
        public RootPostData GetRootPostData(){
            return _rootPostData;
        }

        /// <summary>
        /// ルートレスポンス取得
        /// </summary>
        public RootResponseData GetRootResponse(){
            return _rootResponseData;
        }

        /// <summary>
        /// ルートレスポンスの設定
        /// </summary>
        public void SetRootResponseData( RootResponseData rootResponseData ){
            _rootResponseData = rootResponseData;
        }
        
        /// <summary>
        /// byte[]の受信データセット
        /// </summary>
        public void SetRawResponseData( byte[] data ){
            _rawResponseData = data;
        }

        /// <summary>
        /// 受信した
        /// </summary>
        public void OnReceivedData( byte[] data, bool isResponseEncrypted ){
            _rawResponseData = data;
            
            if( isResponseEncrypted ) {
                _responseData = _serializer.DeserializeWithCrypt( data, encryptKey );
            } else {
                _responseData = _serializer.Deserialize( data );
            }

            OnAPIReceived(_responseData.data);
        }

        public void Abort(){
            isAbort = true;
        }
        
        public Type GetResponseType()
        {
            return typeof(ResponseType);
        }
        
    }
}