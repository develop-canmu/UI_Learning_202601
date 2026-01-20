using System;
using CodeStage.AntiCheat.ObscuredTypes;

namespace Pjfb.Networking.API {
    public class APIEncryptKey {

        ObscuredString MasterKey = "EsOxwoTDvHsNwpc=";
        ObscuredString EncryptApiKey = "DisOTDpYFQw3LgUGHx1VVhcSGjM5LAcwET4gPScKFW8=";

        public string Key {
            get {
                if( string.IsNullOrEmpty(_key) ) {
                    _key = ObscuredString.Decrypt(CodeStage.AntiCheat.Utils.StringUtils.BytesToChars(Convert.FromBase64String(EncryptApiKey)), MasterKey);
                }
                return _key;
            }
        }

        public ObscuredString _key = string.Empty;
    }
}