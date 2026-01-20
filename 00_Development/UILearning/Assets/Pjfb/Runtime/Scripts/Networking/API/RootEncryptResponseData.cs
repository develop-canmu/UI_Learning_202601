using System;

namespace Pjfb.Networking.API {

    [Serializable]
    public class RootEncryptResponseData : RootResponseData<EncryptResponseData>  {
    }

    [Serializable]
    public class EncryptResponseData : IResponseData {
        public string iv = "";
        public string data = "";
    }

}