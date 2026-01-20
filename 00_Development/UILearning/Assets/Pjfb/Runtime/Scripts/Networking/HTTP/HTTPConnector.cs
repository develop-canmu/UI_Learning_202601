using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Pjfb.Networking.HTTP {
    public class HTTPConnector : IDisposable {
        public int timeoutSecond { get;set; }
        public float progress { get; private set; }


        UnityWebRequest _request = null;
        bool _isAbort = false;
        CancellationTokenSource _cancellationSource = null;



        public HTTPConnector( string url, HTTPMethod method ) {
            _request = new UnityWebRequest( url, method.ToString().ToUpper() );
            //タイムアウトの自前制御
            _request.timeout = 0;
        }


        /// <summary>
        /// リクエストヘッダーの追加
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddHeader( string name, string value ){
            _request.SetRequestHeader(name, value);
        }

        /// <summary>
        /// ポストデータ設定
        /// </summary>
        /// <param name="data"></param>
        public void SetPostData( byte[] data ){
            _request.uploadHandler = new UploadHandlerRaw(data);
        }


        /// <summary>
        /// 接続中止
        /// </summary>
        public void Abort(){
            _isAbort = true;
            _request.Abort();
            _cancellationSource.Cancel();
        }


        /// <summary>
        /// 接続開始
        /// </summary>
        /// <returns></returns>
        public async UniTask<HTTPResponseData> Connect(){

            if( Application.internetReachability == NetworkReachability.NotReachable ) {
                return CreateErrorResponse( HTTPResult.NoneConnectNetwork, _request );
            }

            _cancellationSource = new CancellationTokenSource();
            _cancellationSource.CancelAfterSlim(TimeSpan.FromSeconds(timeoutSecond));
            var token = _cancellationSource.Token;
            try {
                _request.downloadHandler = new DownloadHandlerBuffer();
                var asyncOperation = await _request.SendWebRequest().
                ToUniTask(Progress.Create<float>(x => progress = x), cancellationToken:_cancellationSource.Token);

            } catch (OperationCanceledException) {
                //明示的なabortな場合 
                if( _isAbort ) {
                    return CreateErrorResponse( HTTPResult.Abort, _request );    
                }
                //明示的なキャンセルがよばれていないのでtimeoutによるcancelとしてあつかう
                return CreateErrorResponse( HTTPResult.TimeOut, _request );

            } catch (Exception e) {
                //念の為明示的なキャンセル判定
                if( _isAbort ) {
                    return CreateErrorResponse( HTTPResult.Abort, _request );    
                }

                //念の為キャンセルがが実行されているかチェック
                if( _cancellationSource.IsCancellationRequested ) {
                    return CreateErrorResponse( HTTPResult.TimeOut, _request );
                }

                CruFramework.Logger.LogError(" HTTPConnect Error : " + e.Message);
                return CreateErrorResponse( HTTPResult.Error, _request );
            } 

            //abortが呼ばれて実行されるまでの間に終了してしまったときよう
            if( _isAbort ) {
                return CreateErrorResponse( HTTPResult.Abort, _request );    
            }

            //CancellationTokenSourceのcancelが呼ばれて実行されるまでの間に終了してしまったときよう。Cancelされていたら
            //timeoutとしてあつかう
            if( _cancellationSource.IsCancellationRequested ) {
                return CreateErrorResponse( HTTPResult.TimeOut, _request );
            }
            
            if( _request.result != UnityWebRequest.Result.Success ) {
                return CreateErrorResponse( HTTPResult.Error, _request );    
            }

            if( CheckSuccessResponseCode( _request.responseCode ) ) {
                return CreateResponse( HTTPResult.Success, _request );
            } else {
                return CreateErrorResponse( HTTPResult.Error, _request );    
            }
        }

        /// <summary>
        /// レスポンスコードから成功チェック
        /// </summary>
        /// <param name="responseCode"></param>
        /// <returns></returns>
        bool CheckSuccessResponseCode( long responseCode ){
            return ( responseCode >= 200 && responseCode < 300 ) || responseCode == 304;
        }


        /// <summary>
        /// レスポンスデータ作成
        /// </summary>
        /// <param name="result"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        HTTPResponseData CreateResponse( HTTPResult result, UnityWebRequest request ){
            var statusCode = request.responseCode;
            byte[] data = null;
            if( request.downloadHandler != null ) {
                data = request.downloadHandler.data;
            }
            var headers = request.GetResponseHeaders();
            return new HTTPResponseData(result, statusCode, data, headers); 
        }

        /// <summary>
        /// エラーレスポンスデータ作成
        /// </summary>
        /// <param name="result"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        HTTPResponseData CreateErrorResponse( HTTPResult result, UnityWebRequest request ){
            var response = CreateResponse(result, request);
            var message = request.error;
            return new HTTPErrorResponseData(response, message); 
        }

        void IDisposable.Dispose(){
            if( _request != null ) {
                _request.Dispose();
            }
        }
    }
}
