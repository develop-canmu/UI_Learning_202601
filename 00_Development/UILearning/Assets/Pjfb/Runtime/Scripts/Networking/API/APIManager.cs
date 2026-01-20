using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Pjfb.Networking.API {

    [CruFramework.FrameworkDocument("API", nameof(APIManager), "AP接続管理クラス。APIRequestを使用してAPIに接続します。")]
    public class APIManager : CruFramework.Utils.Singleton<APIManager>
    {
        class RequestWrapper{
            public IAPIRequest request = null;
            public bool isDone = false;
            public System.Exception exception = null;
            public RequestWrapper( IAPIRequest request ){
                this.request = request;
            }
        }

        [CruFramework.FrameworkDocument("接続中か")]
        public bool isConnecting => _connectingRequestList.Count > 0;

        public IAPISetting setting =>_setting;
        public IAPIConnecter connecter => _connecter;
        public IAPIHandler handler => _handler;
        public IAPIErrorHandler errorHandler => _errorHandler;



        IAPISetting _setting = new DefaultAPISetting();
        IAPIConnecter _connecter = new APIConnecter();
        IAPIHandler _handler = new DefaultAPIHandler();
        IAPIErrorHandler _errorHandler = new DefaultAPIErrorHandler();


        List<RequestWrapper> _requestList = new List<RequestWrapper>();
        List<RequestWrapper> _retryRequestList = new List<RequestWrapper>();
        List<RequestWrapper> _connectingRequestList = new List<RequestWrapper>();
        IAPISerializer<EmptyPostData, EmptyResponseData> _rootSerializer = new JsonSerializer<EmptyPostData, EmptyResponseData>();
        IAPISerializer<EmptyPostData, ErrorAPIResponse> _errorSerializer = new JsonSerializer<EmptyPostData, ErrorAPIResponse>();

        SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// 設定クラスのセット
        /// </summary>
        [CruFramework.FrameworkDocument("設定クラスをセットします。IAPISettingを実装する必要があります")]
        public void SetSetting( IAPISetting setting ) {
            if( setting == null ) {
                CruFramework.Logger.LogError( "set setting is error" );
                return;
            }
            _setting = setting;
        }

         /// <summary>
        /// handlerクラスのセット
        /// </summary>
        [CruFramework.FrameworkDocument("handlerをセットします。IAPIHandlerを実装する必要があります")]
        public void SetHandler( IAPIHandler handler ) {
            if( handler == null ) {
                CruFramework.Logger.LogError( "set handler is error" );
                return;
            }
            _handler = handler;
        }

        /// <summary>
        /// 接続クラスのセット
        /// </summary>
        [CruFramework.FrameworkDocument("接続クラスをセットします。IAPIConnecterを実装する必要があります")]
        public void SetConnecter( IAPIConnecter connecter ) {
            if( connecter == null ) {
                CruFramework.Logger.LogError( "set connecter is error" );
                return;
            }
            _connecter = connecter;
        }

        /// <summary>
        /// エラーハンドラのセット
        /// </summary>
        /// <param name="handler"></param>
        [CruFramework.FrameworkDocument("エラーのハンドルクラスをセットします。IAPIErrorHandlerを実装する必要があります")]
        public void SetErrorHandler( IAPIErrorHandler handler ) {
            if( handler == null ) {
                CruFramework.Logger.LogError( "set handler is error" );
                return;
            }
            _errorHandler = handler;
        }

        /// <summary>
        /// API接続
        /// </summary>
        /// <param name="request"></param>
        [CruFramework.FrameworkDocument("API接続を行います。エラー発生時はAPIExceptionがthrowされます。")]
        public async UniTask<T> Connect<T>( T request ) where T : IAPIRequest {
            request.timeoutSecond = _setting.timeoutSecond;
            await _semaphore.WaitAsync();
            RequestWrapper wrapper = null; 
            try {
                wrapper = AddRequest( _requestList, request );
                if( _connectingRequestList.Count < _setting.maxConnection ) {
                    StartConnectNextRequest( _requestList );
                }
            } finally {
                _semaphore.Release();
            }

            await UniTask.WaitUntil(()=>wrapper.isDone);
            _handler.OnFinishConnect(wrapper.request, wrapper.exception != null );
            if( wrapper.exception != null ) {
                CruFramework.Logger.LogError( "API Exception StackTrace : " + wrapper.exception.StackTrace );
                throw wrapper.exception;
            }
            return request;
        }


        /// <summary>
        /// APIのリトライ処理   
        /// </summary>
        /// <param name="request"></param>
        /// <typeparam name="T"></typeparam>
        [CruFramework.FrameworkDocument("APIのリトライ接続を行います。内部でリトライ回数、フラグが更新されます。Requestは再利用されます")]
        public void RetryAPI<T>( T request ) where T : IAPIRequest {
            var wrapper = FindRequestWrapper( _connectingRequestList, request );
            if( wrapper == null ) {
                CruFramework.Logger.LogError( "not find request in connectingRequestList. In order to retry, need return false in ErrorHandler");
                return;
            } 

            request.Retry();
            request.timeoutSecond = _setting.timeoutSecond;
            wrapper.exception = null;
            ConnectTask(wrapper).Forget();
        }


        /// <summary>
        /// リクエストのキャンセル
        /// </summary>
        /// <param name="request"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [CruFramework.FrameworkDocument("すべてのAPIの接続をキャンセルします")]
        public void CancelRequestAll() {
            _requestList.Clear();
            foreach(var requestWrapper in _connectingRequestList ){
                CancelRequest(requestWrapper.request);
            }
        }

        [CruFramework.FrameworkDocument("指定Requestの接続をキャンセルします")]
        public void CancelRequest( IAPIRequest request ) {
            request.Abort();
            _connecter.Abort(request);
            foreach(var requestWrapper in _requestList ){
                if( requestWrapper.request == request ) {
                    _requestList.Remove(requestWrapper);
                    break;
                }
            }
        }

        /// <summary>
        /// APISettingの最新バージョンでアセットバージョンを更新
        /// </summary>
        /// <param name="request"></param>
        public void UpdateAssetVersionWithSettingLatestVersion() {
            setting.assetVersion = setting.latestAssetVersion;
        }

        /// <summary>
        /// API接続
        /// </summary>
        /// <param name="request"></param>
        void StartConnectNextRequest( List<RequestWrapper> requestList  ) {
            if( requestList.Count() <= 0 ){
                return;
            }

            var wrapper = requestList.First();
            _connectingRequestList.Add(wrapper);
            requestList.Remove(wrapper);
            ConnectTask(wrapper).Forget();
        }
        

        /// <summary>
        /// API接続
        /// </summary>
        /// <param name="request"></param>
        async UniTask ConnectTask( RequestWrapper wrapper ) {
            try {

                //接続処理はメインスレッドで行うように
                await UniTask.Yield();

#if PJFB_ENABLE_API_LOG
                CruFramework.Logger.Log("[API]connect start : " + wrapper.request);
#endif            

                AddRequestHeader(ref wrapper);
                SettingRootPostParam(ref wrapper);

                //暗号化設定
                wrapper.request.isEncrypt = setting.isEncrypt;
                wrapper.request.encryptKey = setting.encryptKey;
                _handler.OnBeginConnect(wrapper.request);
                var apiResult = await _connecter.Connect( _setting.baseURL, wrapper.request );
                var rawResponseData = apiResult.responseData;

                var rootResponse = _rootSerializer.Deserialize(rawResponseData);

#if PJFB_ENABLE_API_LOG
                var connectUrl = System.IO.Path.Combine( _setting.baseURL, wrapper.request.apiName);
                var logText = "[API]send : " + connectUrl + "\n";

                byte[] rawData = wrapper.request.GetCacheRawPostData();
                if(rawData != null)
                {
                    var rawPostJson = System.Text.Encoding.UTF8.GetString(rawData);
                    logText += "raw post  : " + rawPostJson + "\n";
                }

                var rawResponseJson = System.Text.Encoding.UTF8.GetString(rawResponseData);
                logText += "response  : " + rawResponseJson;
                var limitLength = 10000;
                if( logText.Length >= limitLength ) {
                    //文字数制限超えていたら分割
                    var count = (logText.Length  / limitLength) + 1;
                    for( int i=0; i<count; ++i ) {
                        if( (i*limitLength + limitLength) > logText.Length ) {
                            CruFramework.Logger.Log(logText.Substring(i*limitLength, logText.Length - i*limitLength));
                        } else {
                            CruFramework.Logger.Log(logText.Substring(i*limitLength, limitLength));
                        }
                        
                    }
                    
                } else {
                    CruFramework.Logger.Log(logText);
                }

                //暗号化の場合は複合化されたものを出力
                if( rootResponse.isEncrypted ) {
                    logText = "";
                    var postData = wrapper.request.GetRootPostData();
                    var postJson = JsonUtility.ToJson(postData);
                    logText += "decrypt post  : " + postJson + "\n";
                    
                    var responseJson = DecryptResponseData( rawResponseData, setting.encryptKey );
                    logText += "decrypt response  : " + responseJson;
                    if( logText.Length >= limitLength ) {
                        //文字数制限超えていたら分割
                        var count = (logText.Length  / limitLength) + 1;
                        for( int i=0; i<count; ++i ) {
                            if( (i*limitLength + limitLength) > logText.Length ) {
                                CruFramework.Logger.Log(logText.Substring(i*limitLength, logText.Length - i*limitLength));
                            } else {
                                CruFramework.Logger.Log(logText.Substring(i*limitLength, limitLength));
                            }
                        }
                    } else {
                        CruFramework.Logger.Log(logText);
                    }
                }
                
#endif            


                RootResponseData<ErrorAPIResponse> errorResponse = null;
                if( rootResponse.isEncrypted ) {
                    errorResponse = _errorSerializer.DeserializeWithCrypt(rawResponseData, setting.encryptKey);    
                } else {
                    errorResponse = _errorSerializer.Deserialize(rawResponseData);    
                }

                apiResult.request.SetRawResponseData(rawResponseData);
                apiResult.request.SetRootResponseData(rootResponse);
                errorHandler.CheckResponseCode( apiResult, errorResponse , rootResponse.code );
                apiResult.request.OnReceivedData( rawResponseData, rootResponse.isEncrypted );
                FinishedConnect(wrapper);
            } catch( System.Exception e ) {
                await ErrorFinishedConnect(wrapper, e);
            } finally{
                if( wrapper.isDone ) {
                    _connectingRequestList.Remove(wrapper);
                    StartConnectNextRequest( _requestList );
                }
            }
        }

        
        /// <summary>
        /// リクエスト登録
        /// </summary>
        RequestWrapper AddRequest(List<RequestWrapper> list, IAPIRequest request ) {
            var wrapper = CreateWrapper(request);
            list.Add(wrapper);   
            return wrapper;
        }

        /// <summary>
        /// RequestWrapperをrequestから検索
        /// </summary>
        RequestWrapper FindRequestWrapper( List<RequestWrapper> list, IAPIRequest request ) {
            foreach( var wrapper in list ){
                if( wrapper.request == request ) {
                    return wrapper;
                }
            }
            
            return null;
        }

        /// <summary>
        /// Wrapperの作成
        /// </summary>
        RequestWrapper CreateWrapper( IAPIRequest request ) {
            return new RequestWrapper(request);;
        }

        /// <summary>
        /// レクエストにヘッダー追加
        /// </summary>
        /// <param name="wrapper"></param>
        void AddRequestHeader( ref RequestWrapper wrapper ) {
            var header = new Dictionary<string,string>();
            AddBasicAuthHeader(ref header);
            wrapper.request.SetHeaders(header);
        }

        /// <summary>
        /// RootParamの設定
        /// </summary>
        void SettingRootPostParam( ref RequestWrapper wrapper ){
            var postData = wrapper.request.GetRootPostData();
            var request = wrapper.request;
            postData.reqCount = request.retryCount + 1;
            postData.isRetry = request.retryCount > 0;
            
#if UNITY_IOS
            postData.osType = 1;

#elif UNITY_ANDROID
            postData.osType = 2;
#else
            postData.osType = 1;
            CruFramework.Logger.LogWarning("The platform may be Standalone. Check your platform");
#endif      
            postData.appVer = Application.version;
            postData.assetVer = setting.assetVersion;
            postData.sessionId = setting.sessionId;
            postData.loginId = setting.loginId;
            postData.masterVer = setting.masterVersion;
            postData.oneTimeToken = request.oneTimeToken;
        }

        /// <summary>
        /// Basic認証ヘッダー追加
        /// </summary>
        void AddBasicAuthHeader( ref Dictionary<string,string> header ) {
            string auth = setting.BCMa5vHjK7pC + ":" + setting.STBE5HfShN8w;
            auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
            auth = "Basic " + auth;
            header.Add("AUTHORIZATION", auth);
        }


        /// <summary>
        /// 接続終了
        /// </summary>
        void FinishedConnect( RequestWrapper wrapper ) {
            wrapper.isDone = true;
        }

        /// <summary>
        /// エラー終了
        /// </summary>
        async UniTask ErrorFinishedConnect( RequestWrapper wrapper,  System.Exception e ) {
            try {
                if( _errorHandler == null ) {
                    CruFramework.Logger.LogError( "error handler is error" );
                    _errorHandler = new DefaultAPIErrorHandler();
                }

                wrapper.exception = e;

                var apiException = e as APIException;
                if( apiException == null ) {
                    wrapper.isDone = await _errorHandler.OnSystemError(e);
                    return;
                }

                if( apiException.result == APIResultCode.TimeOut ) {
                    wrapper.isDone = await _errorHandler.OnTimeOutError( apiException );
                } else if( apiException.result == APIResultCode.NoneConnectNetwork ) {
                    wrapper.isDone = await _errorHandler.OnNoneConnectNetworkError( apiException );
                } else {
                    wrapper.isDone = await _errorHandler.OnAPIError( apiException );
                }
            } catch(System.Exception innerException ) {
                CruFramework.Logger.LogError( "ErrorFinishedConnect inner error : " + e.Message );
                wrapper.exception = innerException;
                wrapper.isDone = true;
            }
        }
#if PJFB_ENABLE_API_LOG
        /// <summary>
        /// レスポンス複合化
        /// </summary>
        string DecryptResponseData( byte[] data, string key ) {
            var rawJson = System.Text.Encoding.UTF8.GetString(data);
            var cryptRoot = JsonUtility.FromJson<RootEncryptResponseData>(rawJson);    
            var cryptBytes = System.Convert.FromBase64String(cryptRoot.data.data);   
            var decryptJson = Pjfb.Storage.Crypt.Decrypt(cryptBytes, cryptRoot.data.iv, key);
            return decryptJson;
        }
#endif
    }
}