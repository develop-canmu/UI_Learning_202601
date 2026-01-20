using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Pjfb.Networking.API {
    public interface IAPIErrorHandler {

        /// <summary>
        /// レスポンスコードチェック
        /// </summary>
        void CheckResponseCode( APIResult request, RootResponseData<ErrorAPIResponse> errorAPIResponse, long code );

        /// <summary>
        /// タイムアウトエラー
        /// </summary>
        /// <returns>APIを終了させるか。trueで終了（isDoneがtrueになるのでawaitを抜ける）. retryを行う場合はfalseを返して接続中にする必要があります。</returns>
        UniTask<bool> OnTimeOutError( APIException exception );
        
        /// <summary>
        /// 未接続エラー
        /// </summary>
        /// <returns>APIを終了させるか。trueで終了（isDoneがtrueになるのでawaitを抜ける）. retryを行う場合はfalseを返して接続中にする必要があります。</returns>
        UniTask<bool> OnNoneConnectNetworkError( APIException exception );

        /// <summary>
        /// APIエラー
        /// </summary>
        /// <returns>APIを終了させるか。trueで終了（isDoneがtrueになるのでawaitを抜ける）. retryを行う場合はfalseを返して接続中にする必要があります。</returns>

        UniTask<bool> OnAPIError( APIException exception );

        /// <summary>
        /// API以外のExceptionエラー
        /// </summary>
        /// <returns>APIを終了させるか。trueで終了（isDoneがtrueになるのでawaitを抜ける）. retryを行う場合はfalseを返して接続中にする必要があります。</returns>
        UniTask<bool> OnSystemError( System.Exception exception );
    }
}