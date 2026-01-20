using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb.Networking.API {

    public interface IAPISetting 
    {
        public int maxConnection{get;set;}
        public string baseURL{get;set;}
        public int timeoutSecond{get;set;}
        public bool isLocalMode{get;set;}
        // 最新のアセットバージョン
        public long latestAssetVersion{get;set;}
        // 現在のアセットバージョン
        public long assetVersion{get;set;}
        public long masterVersion{get;set;}
        
        public string sessionId{get;set;}
        public string loginId{get;set;}

        public string versionDownloadURL{get;set;}
        public string apiURL{get;set;}
        public string reviewURL{get;set;}

        public bool isEncrypt{get;}
        public string encryptKey{get;}


        //basic認証ユーザー名
        public string BCMa5vHjK7pC{get;set;}
        //basic認証パスワード
        public string STBE5HfShN8w{get;set;}
    }
}