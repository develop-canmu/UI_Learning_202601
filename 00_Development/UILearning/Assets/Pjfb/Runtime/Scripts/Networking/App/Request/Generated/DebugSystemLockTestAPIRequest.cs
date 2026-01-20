//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
システムロックのデバッグ実施

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugSystemLockTestAPIPost : AppAPIPostBase {
		public long value = 0; // 値

   }

   [Serializable]
   public class DebugSystemLockTestAPIResponse : AppAPIResponseBase {

   }
      
   public partial class DebugSystemLockTestAPIRequest : AppAPIRequestBase<DebugSystemLockTestAPIPost, DebugSystemLockTestAPIResponse> {
      public override string apiName{get{ return "debug/systemLockTest"; } }
   }
}