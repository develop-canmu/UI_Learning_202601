using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pjfb.Networking.HTTP;

namespace Pjfb.Networking.API {
    
    public class APIException : System.Exception {
        public APIResultCode result{get; private set;}
        public long errorParam{get; private set;}
        public IAPIRequest request{get; private set;}
        public string apiErrorMessage{get; private set;}

        public APIException( APIResultCode result, long errorParam, IAPIRequest request, string message ) : base("[API] " + message) {
            this.result  = result;
            this.errorParam = errorParam;
            this.request = request;
            this.apiErrorMessage = message;
        }

        public APIException( APIResultCode result, long errorParam, IAPIRequest request, string message, string apiErrorMessage ) : base("[API] " + message) {
            this.result  = result;
            this.errorParam = errorParam;
            this.request = request;
            this.apiErrorMessage = apiErrorMessage;
        }
    }


}