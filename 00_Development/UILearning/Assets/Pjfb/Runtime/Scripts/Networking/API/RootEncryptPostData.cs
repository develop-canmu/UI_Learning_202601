using System;

namespace Pjfb.Networking.API {
    
    [Serializable]
    public class RootEncryptPostData: IPostData {
        public string iv = "";
        public string data = "";
    }

}