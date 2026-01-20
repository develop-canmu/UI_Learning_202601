using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pjfb.Networking.App.Request;
using Pjfb.Networking.API;
using Cysharp.Threading.Tasks;
using PolyQA;

namespace Pjfb.Networking.App {
    
    public enum APIResponseCode : int {
        Success = 0,

        UnknownError = 1000, //	不明なエラー
        Maintenance = 1001, //メンテナンス中
        OSTypeError = 1002, //OS種別が不適切（osType に想定していない値が渡されている）
        AppVerError = 1003,	//アプリバージョンが不適切（アプリのアップデートが必要）,
        AssetVerError = 1004, //アセットバージョンが不適切（最新アセットのダウンロードが必要）
        MasterVerError = 1005,	//マスターデータバージョンが不適切（最新マスタデータのダウンロードが必要）
        SessionError = 1010,    //セッション切れ（同アカウントから他端末にログインした場合など）
        ForbiddenError = 1011,    //アクセス権限がない（開発者向けのAPIを許可IPアドレス外から実行した場合など）

        Failed = 2000,    //処理失敗

        InAppEmpty = 92001,    //課金レシートのin_appが空
        PvpLateMessage = 93005,    //メッセージ："残りのシーズン時間が不足しています。次回のシーズンよりご参加をお願いします。"
        ShopMaintenance = 93098,    //課金メンテナンス
        GachaMaintenance = 93099,    //ガチャメンテナンス
    }


    /// <summary>
    /// エラーモーダルタイプ
    /// </summary>
    public enum ErrorModalType : int {
        ToTitle = 1,    //  タイトルにもどる
        MessageOnly = 2,    // メッセージのみ
        ToHome = 3,    // ホームに戻る

        Unique = 90,    // 独自
    }

    /// <summary>
    /// メンテナンス用レスポンスクラス
    /// </summary>
    [Serializable]
    public class MaintenanceAPIResponse : AppAPIResponseBase {
        public string errorMessage = null;
        public string maintenanceStartAt = null;
        public string maintenanceEndAt = null;
        public string twitterUrl = null;
    }

    /// <summary>
    /// アプリバージョン用レスポンスクラス
    /// </summary>
    [Serializable]
    public class AppVersionErrorAPIResponse : AppAPIResponseBase {
        public string errorMessage = null;
        public string appleStoreUrl = null;
        public string googleStoreUrl = null;
    }

    /// <summary>
    /// アセットバージョン用のExeption
    /// </summary>
    public class APIAssetVersionErrorException : APIException {
        public RootResponseData<ErrorAPIResponse> errorResponse{get; private set;}

        public APIAssetVersionErrorException( APIResultCode result, long errorParam, IAPIRequest request, string message, RootResponseData<ErrorAPIResponse> response ) 
        : base(result, errorParam, request, message, response.data.errorMessage) {
            errorResponse = response;
        }
    }


    /// <summary>
    /// アセットバージョン更新情報
    /// </summary>
    [Serializable]
    public class UpdateAssetVersionInfo {
        public long updateVersion = 0;
        public string dateTime = string.Empty;
    }


    public class PjfbAPIErrorHandler : IAPIErrorHandler
    {
        const int retryMax = 3;
        const int retryInterval = 3;

        readonly int uniqueErrorTypeOffset = (int)ErrorModalType.Unique * APIUtility.errorTypeOffset;
        const int uniqueModalErrorTypeOffset = 100;


        // 独自処理のエラーハンドリング
        // エラーコード毎に設定
        Dictionary<long,Func<IAPIRequest, long, string, UniTask>> _uniqueErrorHandleFunc = new Dictionary<long,Func<IAPIRequest, long, string, UniTask>>();

        public PjfbAPIErrorHandler(){
            //固定のユニークエラーコード設定
            
            SetUniqueErrorHandleFunc((long)APIResponseCode.InAppEmpty, InAppEmptyHandle);
            
            SetUniqueErrorHandleFunc((long)APIResponseCode.PvpLateMessage, PvpLateHandle);
            
            //ガチャメンテ
            SetUniqueErrorHandleFunc((long)APIResponseCode.GachaMaintenance, GachaMaintenanceHandle);   
            
            //課金メンテ
            SetUniqueErrorHandleFunc((long)APIResponseCode.ShopMaintenance, ShopMaintenanceHandle);   
            
        }

        /// <summary>
        /// 独自のエラーハンドリング関数の設定
        /// </summary>
        public void SetUniqueErrorHandleFunc( long errorCode ,Func<IAPIRequest, long, string, UniTask> handleFunc ){
            _uniqueErrorHandleFunc[errorCode] = handleFunc;
        }

        /// <summary>
        /// 独自のエラーハンドリング関数の解除
        /// </summary>
        public void RemoveUniqueErrorHandleFunc( long errorCode ){
            if( _uniqueErrorHandleFunc.ContainsKey(errorCode) ){
                _uniqueErrorHandleFunc.Remove(errorCode);
            }
        }

        /// レスポンスコードチェック
        void IAPIErrorHandler.CheckResponseCode( APIResult result, RootResponseData<ErrorAPIResponse> errorAPIResponse, long code ){
            if( code == (int)APIResponseCode.AssetVerError ){
                //アセットバージョンエラーの場合はそれ用のExeptionを投げる
                throw new APIAssetVersionErrorException( APIResultCode.APIError, code, result.request, "API ResponseCode Error : code = " + code + " mes : " + errorAPIResponse.data.errorMessage, errorAPIResponse );
            } else if( code != (int)APIResponseCode.Success ){
                throw new APIException( APIResultCode.APIError, code, result.request, "API ResponseCode Error : code = " + code + " mes : " + errorAPIResponse.data.errorMessage, errorAPIResponse.data.errorMessage );
            }
            
        }

        //タイムアウトエラー
        async UniTask<bool> IAPIErrorHandler.OnTimeOutError( APIException exception ){
            // QAイベント
            DataSender.Send("APIError", "TimeOut");
            CruFramework.Logger.Log("API TimeOurError : " + exception.request );
            var request = exception.request;
            if( request.retryCount < retryMax ) {
                await UniTask.Delay( retryInterval );
                AppManager.Instance.UIManager.System.TouchGuard.Hide();
                AppManager.Instance.UIManager.System.Loading.Hide();
                RetryAPI(exception.request);
                return false;
            }

            //タイトルに戻る
            var isDialogClose = false;
            ConfirmModalData data = new ConfirmModalData(
                StringValueAssetLoader.Instance["common.confirm"],
                StringValueAssetLoader.Instance["api.retry_over"],
                null,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.to_title"], (window) => {
                    isDialogClose = true;
                })
            );
            
            AppManager.Instance.UIManager.System.TouchGuard.Hide();
            AppManager.Instance.UIManager.System.Loading.Hide();
            await AppManager.Instance.UIManager.ErrorModalManager.OpenModalAsync(ModalType.Confirm, data);
            
            await UniTask.WaitUntil(()=>isDialogClose);
            CruFramework.Logger.LogError("API retry count over");
            AppManager.Instance.BackToTitle();
            return true;
        }

        //未接続エラー
        async UniTask<bool> IAPIErrorHandler.OnNoneConnectNetworkError( APIException exception ){
            // QAイベント
            DataSender.Send("APIError", "NoneConnectNetwork");
            CruFramework.Logger.LogError("API NoneConnectNetworkError : " + exception.request );
            var isRetry = false;
            var isDialogClose = false;
            ModalWindow openWindow = null;
            ConfirmModalData data = new ConfirmModalData(
                StringValueAssetLoader.Instance["common.confirm"],
                StringValueAssetLoader.Instance["api.none_connect_retry"],
                null,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.ok"],( window ) =>{
                    openWindow = window;
                    isRetry = true;
                    isDialogClose = true;
                } ),
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.to_title"], (window) => {
                    isDialogClose = true;
                    isRetry = false;
                })
            );
            
            // タッチガードとローディングを非表示
            AppManager.Instance.UIManager.System.TouchGuard.Hide();
            AppManager.Instance.UIManager.System.Loading.Hide();
            await AppManager.Instance.UIManager.ErrorModalManager.OpenModalAsync(ModalType.Confirm, data);
            
            await UniTask.WaitUntil(()=>isDialogClose);
            if( isRetry ) {
                //コールバック内で閉じるとerrorが出るのでディレイをかける
                await openWindow.CloseAsync();

                var request = exception.request;
                RetryAPI(request);
                return false;
            } else {
                AppManager.Instance.BackToTitle();
                return true;
            }
        }

        //APIエラー
        async UniTask<bool> IAPIErrorHandler.OnAPIError( APIException exception ){
            //キャンセルの場合は何もしない
            if( exception.result == APIResultCode.Cancel ) {
                return true;   
            }

            var errorMessage = string.Empty;
            if( exception.result == APIResultCode.HTTPError || exception.result == APIResultCode.UnknownError ) {
                errorMessage = StringValueAssetLoader.Instance["api.connection_error"];
#if PJFB_ENABLE_API_LOG
                errorMessage += exception.apiErrorMessage;
#endif                
            } else {
                errorMessage = exception.apiErrorMessage;
            }

            CruFramework.Logger.LogError("API Error : " + exception.request );

            await OpenErrorModal( exception.request, exception.errorParam , errorMessage);   

            return true;
        }

        UniTask<bool> IAPIErrorHandler.OnSystemError( System.Exception exception ){
            CruFramework.Logger.LogError("API System Error : " + exception.Message );
            return new UniTask<bool>(true);
        }


        void RetryAPI( IAPIRequest request ) {
            CruFramework.Logger.Log(" retry API  : " + request );
            APIManager.Instance.RetryAPI(request);
        }

        /// <summary>
        /// エラーモーダルを開く
        /// </summary>
        async UniTask OpenErrorModal( IAPIRequest request, long errorCode, string errorMessage ) {

            //メンテナンス
            if( errorCode == (int)APIResponseCode.Maintenance ) {
                var rawResponse = request.rawResponseData;
                var serializer = new JsonSerializer<AppAPIPostBase, MaintenanceAPIResponse>();
                RootResponseData<MaintenanceAPIResponse> maintenanceResponse = null;
                if( request.GetRootResponse().isEncrypted ) {
                    maintenanceResponse = serializer.DeserializeWithCrypt(rawResponse, request.encryptKey);
                } else {
                    maintenanceResponse = serializer.Deserialize(rawResponse);
                }

                if( AppManager.Instance.UIManager.PageManager.CurrentPageType == PageType.Title ) {
                    await OpenMaintenanceModal( maintenanceResponse );

                } else {
                    await OpenToTitleErrorModal(maintenanceResponse.data.errorMessage);
                }
                
                return;
            } else if ( errorCode == (int)APIResponseCode.AppVerError ){
                await OnAppVersionError(request);
                return;
            }

            var errorType = (long)APIUtility.CalcErrorType(errorCode);
            if( errorType >= (int)ErrorModalType.Unique ) {
                if( _uniqueErrorHandleFunc.ContainsKey(errorCode) ) {
                    await _uniqueErrorHandleFunc[errorCode].Invoke(request, errorCode, errorMessage);
                } else {
                    //エラーコードが設定されてなかったら共通処理に流す
                    var offsetErrorCode = errorCode % uniqueErrorTypeOffset;
                    errorType = (long)APIUtility.CalcErrorType(offsetErrorCode);
                    await OpenErrorModalWithType(errorType, errorMessage);
                }
            } else {
                await OpenErrorModalWithType(errorType, errorMessage);
            }
        }

        /// <summary>
        /// タイプ別のエラーモーダルを開く
        /// </summary>
        async UniTask OpenErrorModalWithType( long errorType, string errorMessage ) {
            if( errorType == (int)ErrorModalType.MessageOnly ) {
                //エラーメッセージのみ
                await OpenMessageOnlyErrorModal(errorMessage);
            } else if( errorType == (int)ErrorModalType.ToHome ) {
                //ホームに戻る
                await OpenToHomeErrorModal(errorMessage);
            } else {
                //タイトルに戻る   
                await OpenToTitleErrorModal(errorMessage);
            }
        }

        /// <summary>
        /// エラーメッセージのみのエラーモーダルを開く
        /// </summary>
        async UniTask OpenMessageOnlyErrorModal( string errorMessage ) {
            // QAイベント
            DataSender.Send("APIError", "MessageOnly");
            var isTapButton = false;
            ConfirmModalData data = new ConfirmModalData(
                StringValueAssetLoader.Instance["common.confirm"],
                errorMessage,
                null,
                null,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.close"], (window) => {
                    isTapButton = true;
                })
            );
            
            AppManager.Instance.UIManager.System.TouchGuard.Hide();
            AppManager.Instance.UIManager.System.Loading.Hide();
            var modal = await AppManager.Instance.UIManager.ErrorModalManager.OpenModalAsync(ModalType.Confirm, data);
            await UniTask.WaitUntil(()=>isTapButton);
            modal.Close();

            APIManager.Instance.CancelRequestAll();
        }

        
        /// <summary>
        /// Homeに戻るエラーモーダルを開く
        /// </summary>
        async UniTask OpenToHomeErrorModal( string errorMessage ) {
            // QAイベント
            DataSender.Send("APIError", "ToHome");
            var isTapButton = false;
            //現在Homeにいるかどうかで挙動を変える
            var isToHome = AppManager.Instance.UIManager.PageManager.CurrentPageType != PageType.Home; 
            var buttonKey = isToHome ? "common.to_home" : "common.to_title";
            ConfirmModalData data = new ConfirmModalData(
                StringValueAssetLoader.Instance["common.confirm"],
                errorMessage,
                null,
                null,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance[buttonKey], (window) => {
                    AppManager.Instance.UIManager.ModalManager.CloseAllModalWindow();
                    isTapButton = true;
                })
            );
            
            AppManager.Instance.UIManager.System.TouchGuard.Hide();
            AppManager.Instance.UIManager.System.Loading.Hide();
            var modal = await AppManager.Instance.UIManager.ErrorModalManager.OpenModalAsync(ModalType.Confirm, data);
            
            await UniTask.WaitUntil(()=>isTapButton);

            APIManager.Instance.CancelRequestAll();

            if(isToHome){
                // ページが違うのでホームに遷移
                await modal.CloseAsync();
                AppManager.Instance.UIManager.PageManager.OpenPage(PageType.Home, true, null);
            }
            else {
                // すでにHomeにいる場合はタイトルへ
                AppManager.Instance.BackToTitle();
            }
        }

        /// <summary>
        /// タイトルに戻るエラーモーダルを開く
        /// </summary>
        async UniTask OpenToTitleErrorModal( string errorMessage ) {
            // QAイベント
            DataSender.Send("APIError", "ToTitle");
            var isTapButton = false;
            ConfirmModalData data = new ConfirmModalData(
                StringValueAssetLoader.Instance["common.confirm"],
                errorMessage,
                null,
                null,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.to_title"], (window) => {
                    isTapButton = true;
                })
            );
            
            AppManager.Instance.UIManager.System.TouchGuard.Hide();
            AppManager.Instance.UIManager.System.Loading.Hide();
            await AppManager.Instance.UIManager.ErrorModalManager.OpenModalAsync(ModalType.Confirm, data);
            
            await UniTask.WaitUntil(()=>isTapButton);

            APIManager.Instance.CancelRequestAll();
            AppManager.Instance.BackToTitle();
        }
        

        /// <summary>
        /// アプリバージョンエラー
        /// </summary>
        async UniTask OnAppVersionError( IAPIRequest request ) {
            var rawResponse = request.rawResponseData;
            var serializer = new JsonSerializer<AppAPIPostBase, AppVersionErrorAPIResponse>();
            RootResponseData<AppVersionErrorAPIResponse> response = null;
            if( request.GetRootResponse().isEncrypted ) {
                response = serializer.DeserializeWithCrypt(rawResponse, request.encryptKey);
            } else {
                response = serializer.Deserialize(rawResponse);
            }
            
            await OpenAppVersionErrorModal( response );
        }
        
        /// <summary>
        /// アプリバージョンエラーモーダルを開く
        /// </summary>
        async UniTask OpenAppVersionErrorModal( RootResponseData<AppVersionErrorAPIResponse> responseData ) {
            // QAイベント
            DataSender.Send("APIError", "AppVersion");
            ConfirmModalData data = new ConfirmModalData(
                StringValueAssetLoader.Instance["common.confirm"],
                responseData.data.errorMessage,
                null,
                null,
                new ConfirmModalButtonParams(StringValueAssetLoader.Instance["common.to_store"], (window) => {
#if UNITY_IOS
                    Application.OpenURL(responseData.data.appleStoreUrl);
#else
                    Application.OpenURL(responseData.data.googleStoreUrl);
#endif                    

                })
            );

            AppManager.Instance.UIManager.System.TouchGuard.Hide();
            AppManager.Instance.UIManager.System.Loading.Hide();
            await AppManager.Instance.UIManager.ErrorModalManager.OpenModalAsync(ModalType.Confirm, data);
            APIManager.Instance.CancelRequestAll();
        }


        /// <summary>
        /// メンテナンスモーダルを開く
        /// </summary>
        async UniTask OpenMaintenanceModal( RootResponseData<MaintenanceAPIResponse> maintenanceData ) {
            // QAイベント
            DataSender.Send("APIError", "Maintenance");
            // var isTapButton = false;
            var param = new MaintenanceModal.Param();
            param.errorMessage = maintenanceData.data.errorMessage;
            param.maintenanceStartAt = maintenanceData.data.maintenanceStartAt;
            param.maintenanceEndAt = maintenanceData.data.maintenanceEndAt;
            param.twitterUrl = maintenanceData.data.twitterUrl;
                
            AppManager.Instance.UIManager.System.TouchGuard.Hide();
            AppManager.Instance.UIManager.System.Loading.Hide();
            await AppManager.Instance.UIManager.ErrorModalManager.OpenModalAsync(ModalType.Maintenance, param);
            
            APIManager.Instance.CancelRequestAll();
        }

        async UniTask InAppEmptyHandle(IAPIRequest request, long errorCode, string message )
        {
            await OpenMessageOnlyErrorModal(message);
        }

        async UniTask PvpLateHandle(IAPIRequest request, long errorCode, string message ){
            var isCurrentHome = AppManager.Instance.UIManager.PageManager.CurrentPageType == PageType.Home; 
            if( isCurrentHome ) {
                await OpenMessageOnlyErrorModal(message);
            } else {
                await OpenToHomeErrorModal(message);
            }
        }
        
        async UniTask GachaMaintenanceHandle(IAPIRequest request, long errorCode, string message ){
            var isCurrentHome = AppManager.Instance.UIManager.PageManager.CurrentPageType == PageType.Home; 
            if( isCurrentHome ) {
                await OpenMessageOnlyErrorModal(message);
            } else {
                await OpenToHomeErrorModal(message);
            }
        }
        
        async UniTask ShopMaintenanceHandle(IAPIRequest request, long errorCode, string message )
        {
            // shop/getBillingRewardBonusList で 課金メンテナンスエラーが帰った場合のみホームに遷移しない
            var requestGetBillingRewardBonus = request is ShopGetBillingRewardBonusListAPIRequest;
            if( requestGetBillingRewardBonus ) {
                await OpenMessageOnlyErrorModal(message);
            } else {
                await OpenToHomeErrorModal(message);
            }
        }
    }
}