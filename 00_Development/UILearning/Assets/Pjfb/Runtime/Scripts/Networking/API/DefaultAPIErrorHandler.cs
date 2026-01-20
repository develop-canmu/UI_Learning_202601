using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Pjfb.Networking.API {
    public class DefaultAPIErrorHandler : IAPIErrorHandler {
        
        /// レスポンスコードチェック
        void IAPIErrorHandler.CheckResponseCode( APIResult result, RootResponseData<ErrorAPIResponse> errorAPIResponse, long code ){
            if( code != 0 ){
                throw new APIException( APIResultCode.APIError, code, result.request, "API error : " + this );
            }
        }

         //タイムアウトエラー
        UniTask<bool> IAPIErrorHandler.OnTimeOutError( APIException exception ){
            CruFramework.Logger.LogError("API TimeOurError : " + exception.request );
            return new UniTask<bool>(true);
        }

        //未接続エラー
        UniTask<bool> IAPIErrorHandler.OnNoneConnectNetworkError( APIException exception ){
            CruFramework.Logger.LogError("API NoneConnectNetworkError : " + exception.request );
            return new UniTask<bool>(true);
        }

        //APIエラー
        UniTask<bool> IAPIErrorHandler.OnAPIError( APIException exception ){
            CruFramework.Logger.LogError("API Error : " + exception.request );
            return new UniTask<bool>(true);
        }

        UniTask<bool> IAPIErrorHandler.OnSystemError( System.Exception exception ){
            CruFramework.Logger.LogError("API System Error : " + exception.Message );
            return new UniTask<bool>(true);
        }
    }
}