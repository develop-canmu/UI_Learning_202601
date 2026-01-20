using System.Collections.Generic;
using Pjfb.Networking.HTTP;
using System.Threading;
using Cysharp.Threading.Tasks;


namespace Pjfb.Networking.API {
    

    public class APIConnecter : IAPIConnecter {

        Dictionary<IAPIRequest, HTTPConnector> connectRequest = new Dictionary<IAPIRequest, HTTPConnector>();
        
        async UniTask<APIResult> IAPIConnecter.Connect( string baseURL, IAPIRequest request ){
            var url = System.IO.Path.Combine( baseURL, request.apiName);
            using(var connecter = new HTTPConnector( url, HTTPMethod.Post )){
                try{
                    //timeout時間
                    connecter.timeoutSecond = request.timeoutSecond;
                    //ヘッダー
                    foreach( var header in request.headers ){
                        connecter.AddHeader(header.Key, header.Value);
                    }
                    //postData       
                    if( request.isEncrypt ) {
                        connecter.SetPostData(request.CreateRawEncryptPostData());  
                    } else {
                        connecter.SetPostData(request.CreateRawPostData());
                    }
                    

                    if( !connectRequest.ContainsKey(request) ) {
                        connectRequest.Add(request, connecter);    
                    }
                    
                    var httpResponse = await connecter.Connect();   
                    if( httpResponse.result == HTTP.HTTPResult.Success ) {
                        //API成功
                        return new APIResult(request, httpResponse.data);
                    } else {
                        var errorResponse = httpResponse as HTTPErrorResponseData;
                        if( errorResponse.result == HTTPResult.Abort ) {
                            //キャンセル
                            throw new APIException( APIResultCode.Cancel, 0, request, "Cancel API" );    
                        } else if( errorResponse.result == HTTPResult.TimeOut ) {
                            //タイムアウト
                            throw new APIException( APIResultCode.TimeOut, 0,request,  errorResponse.message );    
                        } else if( errorResponse.result == HTTPResult.NoneConnectNetwork ){
                            //ネットワークに未接続
                            throw new APIException( APIResultCode.NoneConnectNetwork, 0,request,  errorResponse.message );    
                        } else {
                            throw new APIException( APIResultCode.HTTPError, (int)errorResponse.result, request, errorResponse.message );
                        }
                    }
                    
                } catch( APIException e) {
                    throw e;
                } catch( System.Exception e ) {
                    throw new APIException( APIResultCode.UnknownError, 0, request, "UnknownError API Error : " + e.Message );
                } finally {
                    if( connectRequest.ContainsKey(request) ) {
                        connectRequest.Remove(request);    
                    }
                }
            }
        }

        public void Abort( IAPIRequest request ){
            if( !connectRequest.ContainsKey(request) ) {
                return;    
            }
            connectRequest[request].Abort();
        }
    }
}