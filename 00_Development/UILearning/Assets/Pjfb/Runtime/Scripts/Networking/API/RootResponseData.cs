using System;

namespace Pjfb.Networking.API {
    
    public interface IResponseData {

    }

    [Serializable]
    public class RootResponseData : IResponseData {
        public long code = 0;
        public string dateTime = "";
        public bool isEncrypted = true;
        

        public void SerRootResponseData( RootResponseData date ){
            this.code = date.code;
            this.dateTime = date.dateTime;
            this.isEncrypted = date.isEncrypted;
        }

    }

    [Serializable]
    public class RootResponseData<Data> : RootResponseData where Data : class {
        public Data data = null;
    }

    [Serializable]
    public class EmptyResponseData : IResponseData {

    }


    [Serializable]
    public class ErrorAPIResponse : IResponseData{
        public string errorMessage = "";
        public long assetVer = 0;
    }
}