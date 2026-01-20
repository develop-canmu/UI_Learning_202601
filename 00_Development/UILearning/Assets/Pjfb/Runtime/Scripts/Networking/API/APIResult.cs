using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pjfb.Networking.HTTP;

namespace Pjfb.Networking.API {
    
    public class APIResult {
        public IAPIRequest request{get; private set;} = null;
        public byte[] responseData { get; private set;} = null;

        public APIResult( IAPIRequest request,byte[] responseData ){
            this.request = request;
            this.responseData = responseData;
        }
    }
}