//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
cacheノード取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugGetCacheNodeListAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class DebugGetCacheNodeListAPIResponse : AppAPIResponseBase {

   }
      
   public partial class DebugGetCacheNodeListAPIRequest : AppAPIRequestBase<DebugGetCacheNodeListAPIPost, DebugGetCacheNodeListAPIResponse> {
      public override string apiName{get{ return "debug/getCacheNodeList"; } }
   }
}