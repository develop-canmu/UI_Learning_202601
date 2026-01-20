
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb.Networking.API {
    public interface IAPIRequest {
        
        /// <summary>
        /// 接続先
        /// </summary>
        string apiName{get;} 

        /// <summary>
        /// タイムアウト時間
        /// </summary>
        int timeoutSecond{get;set;} 

        /// <summary>
        /// ワンタイムトークン
        /// </summary>
        string oneTimeToken{get;set;} 

        /// <summary>
        /// エラーが発生したか
        /// </summary>
        bool isError{get;} 

        /// <summary>
        /// リトライ回数
        /// </summary>
        int retryCount{get;} 

        /// <summary>
        /// 中止したか
        /// </summary>
        bool isAbort{get;} 

        /// <summary>
        /// 暗号化を行うか
        /// </summary>
        bool isEncrypt{get;set;} 

        /// <summary>
        /// 暗号化キー
        /// </summary>
        string encryptKey{get;set;} 

        /// <summary>
        /// ヘッダー
        /// </summary>
        Dictionary<string,string> headers{get;} 

        /// <summary>
        /// byte[]型のレスポンス取得
        /// </summary>
        byte[] rawResponseData{get;} 

        /// <summary>
        /// ヘッダー設定
        /// </summary>
        /// <param name="baseURL"></param>
        void SetHeaders( Dictionary<string,string> headers );

        /// <summary>
        /// postDataの取得
        /// </summary>
        /// <returns></returns>
        byte[] CreateRawPostData();

        /// <summary>
        /// 暗号化されたpostDataの取得
        /// </summary>
        /// <returns></returns>
        byte[] CreateRawEncryptPostData();

        /// <summary>
        /// キャッシュされたポストデータの取得
        /// </summary>
        /// <returns></returns>
        byte[] GetCacheRawPostData();

        /// <summary>
        /// ルートレポスト取得
        /// </summary>
        RootPostData GetRootPostData();

        /// <summary>
        /// ルートレスポンス取得
        /// </summary>
        RootResponseData GetRootResponse();

        /// <summary>
        /// ルートレスポンス設定
        /// </summary>
        void SetRootResponseData( RootResponseData rootResponseData );

        /// <summary>
        ///  byte[]の受信データセット
        /// </summary>
        void SetRawResponseData( byte[] data );

        /// <summary>
        ///  データを受信した
        /// </summary>
        void OnReceivedData( byte[] data, bool isResponseEncrypted );

        /// <summary>
        /// 接続中止
        /// </summary>
        void Abort();

        /// <summary>
        /// リトライ処理
        /// </summary>
        void Retry();

        /// <summary>
        /// レスポンスのタイプを取得
        /// </summary>
        Type GetResponseType();
    }
}