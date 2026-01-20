using System;

namespace Pjfb.Networking.API {
    
    public interface IPostData {

    }

    [Serializable]
    public class EmptyPostData : IPostData {

    }


    [Serializable]
    public class RootPostData: IPostData {
        public int reqCount = 0;
        public bool isRetry = false;
        public string sessionId = string.Empty;
        public string loginId = string.Empty;
        public int osType = 0;
        public string appVer = "";
        public long assetVer = 0;
        public long masterVer = 0;
        public string oneTimeToken = "";
    }


    [Serializable]
    public class RootPostData<ParamType> : RootPostData where ParamType : class {
        public ParamType param = null;
        public RootPostData( ){
        }
    }


}