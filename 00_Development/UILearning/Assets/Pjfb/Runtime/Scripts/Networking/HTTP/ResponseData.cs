
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Pjfb.Networking.HTTP {


    public class HTTPResponseData {
        public HTTPResult result { get; private set;} = HTTPResult.None;
        public long statusCode { get; private set;} = 0;
        public byte[] data { get; private set;} = null;
        public Dictionary<string,string> headers { get; private set;} = null;

        public HTTPResponseData( HTTPResult result, long statusCode, byte[] data, Dictionary<string,string> headers ){
            this.result = result;
            this.statusCode = statusCode;
            this.data = data;
            this.headers = headers;
        }
    }

    public class HTTPErrorResponseData : HTTPResponseData {
        public string message { get; private set;} = string.Empty;

        public HTTPErrorResponseData( HTTPResult result, int statusCode, byte[] data, Dictionary<string,string> headers, string message ) 
        : base(result, statusCode, data, headers){
            this.message = message;   
        }

        public HTTPErrorResponseData( HTTPResponseData response, string message ) 
        : base(response.result, response.statusCode, response.data, response.headers){
            this.message = message;   
        }
    }
}