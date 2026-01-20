using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;


namespace Pjfb.Networking.API {
    

    public abstract class LocalAPIConnecter : IAPIConnecter {
        
        protected abstract int connectionVirtualMilliseconds{get;}
        
        public abstract string CreateDummyJsonData(IAPIRequest request);
        Dictionary<IAPIRequest, CancellationTokenSource> cancellationToken = new Dictionary<IAPIRequest, CancellationTokenSource>();

        
        async UniTask<APIResult> IAPIConnecter.Connect( string baseURL, IAPIRequest request ){

            try{
                var cancellationSource = new CancellationTokenSource();
                cancellationSource.CancelAfterSlim(TimeSpan.FromSeconds(request.timeoutSecond));
                if( !cancellationToken.ContainsKey(request) ) {
                    cancellationToken.Add(request, cancellationSource);    
                }
                await UniTask.Delay( connectionVirtualMilliseconds, cancellationToken:cancellationSource.Token );
                var json = CreateDummyJsonData(request);
                var data = System.Text.Encoding.UTF8.GetBytes(json);
                return new APIResult(request, data);
                
            } catch (OperationCanceledException e) {
                if( !cancellationToken.ContainsKey(request) ) {
                    throw new APIException( APIResultCode.UnknownError, 0, request, "UnknownError API Error : " + e.Message );
                }
                if( e.CancellationToken == cancellationToken[request].Token ) {
                    //タイムアウト
                    throw new APIException( APIResultCode.TimeOut, 0,request,  "TimeOut API" );    
                }
                throw new APIException( APIResultCode.UnknownError, 0, request, "UnknownError API Error : " + e.Message );
            } 
            catch( APIException e) {
                throw e;
            } catch( System.Exception e ) {
                throw new APIException( APIResultCode.UnknownError, 0, request, "UnknownError API Error : " + e.Message );
            } finally {
                if( cancellationToken.ContainsKey(request) ) {
                    cancellationToken.Remove(request);
                }
            }
        }

        public void Abort( IAPIRequest request ){
            if( !cancellationToken.ContainsKey(request) ) {
                return;
            }
            cancellationToken[request].Cancel();
        }

    }
}