//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
設定値更新

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugUpdateConfigAPIPost : AppAPIPostBase {
		public string keyName = ""; // 更新したい config の keyName
		public string value = ""; // config の value に設定したい値

   }

   [Serializable]
   public class DebugUpdateConfigAPIResponse : AppAPIResponseBase {

   }
      
   public partial class DebugUpdateConfigAPIRequest : AppAPIRequestBase<DebugUpdateConfigAPIPost, DebugUpdateConfigAPIResponse> {
      public override string apiName{get{ return "debug/updateConfig"; } }
   }
}