using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pjfb.Networking.API {

    public class DefaultAPISetting : IAPISetting 
    {
        public int maxConnection {get;set;} = 1;
        public string baseURL{get;set;} = "";
        public int timeoutSecond{get;set;} = 15;
        public bool isLocalMode{get;set;} = false;
        public long latestAssetVersion{get;set;} = 0;
        public long assetVersion{get;set;}  = 0;
        public long masterVersion{get;set;} = 0;

        public string sessionId{get;set;} = "";
        public string loginId{get;set;} = "";
        
        public string versionDownloadURL{get;set;} = "https://check.pjfb.gochipon.net/native-api/";
        public string apiURL{get;set;} = "https://check.pjfb.gochipon.net/native-api/";
        public string reviewURL{get;set;}  = "https://check.pjfb.gochipon.net/native-api/";

        public bool isEncrypt{get;} = false;
        public string encryptKey{get;set;} = "";

        public string BCMa5vHjK7pC{get;set;} = "";
        public string STBE5HfShN8w{get;set;} = "";
    }
}