//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
withOneTimeToken が指定されているAPIのテスト

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugWithOneTimeTokenTestAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class DebugWithOneTimeTokenTestAPIResponse : AppAPIResponseBase {

   }
      
   public partial class DebugWithOneTimeTokenTestAPIRequest : AppAPIRequestBase<DebugWithOneTimeTokenTestAPIPost, DebugWithOneTimeTokenTestAPIResponse> {
      public override string apiName{get{ return "debug/withOneTimeTokenTest"; } }
   }
}