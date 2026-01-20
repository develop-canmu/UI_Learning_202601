//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
WEBサーバのIPアドレスを取得する

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugGetIpAddressAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class DebugGetIpAddressAPIResponse : AppAPIResponseBase {

   }
      
   public partial class DebugGetIpAddressAPIRequest : AppAPIRequestBase<DebugGetIpAddressAPIPost, DebugGetIpAddressAPIResponse> {
      public override string apiName{get{ return "debug/getIpAddress"; } }
   }
}