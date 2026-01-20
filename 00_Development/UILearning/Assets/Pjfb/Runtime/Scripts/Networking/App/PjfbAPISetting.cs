using System.Collections;
using System.Collections.Generic;
using Pjfb.Storage;

namespace Pjfb.Networking.API {

    public class PjfbAPISetting : IAPISetting 
    {
        public int maxConnection {get;set;} = 1;
        public string baseURL{get;set;} = "";
        public int timeoutSecond{get;set;} = 15;
        public bool isLocalMode{get;set;} = false;
        // 最新のアセットバージョン
        public long latestAssetVersion{get;set;}  = 0;
        // 現在のアセットバージョン
        public long assetVersion{get;set;}  = 0;
          
        
        public long masterVersion{
            get {
                return LocalSaveManager.saveData.masterVersion;
            }
            set{
                LocalSaveManager.saveData.masterVersion = value;
                LocalSaveManager.Instance.SaveData();
            } 
        }  

        public string sessionId{get;set;} = "";
        public string loginId{get;set;} = "";


        public string versionDownloadURL{get;set;} = AppEnvironment.VersionDownloadURL;
        public string apiURL{get;set;} = AppEnvironment.APIURL;
        public string reviewURL{get;set;}  = AppEnvironment.APIReviewURL;

        public bool isEncrypt{get;set;} = true;
        public string encryptKey => _encryptKey.Key;

        public string BCMa5vHjK7pC{get;set;} = "crz7_api";
        public string STBE5HfShN8w{get;set;} = "E2iDhZJv";


        APIEncryptKey _encryptKey = new APIEncryptKey();
    }
}