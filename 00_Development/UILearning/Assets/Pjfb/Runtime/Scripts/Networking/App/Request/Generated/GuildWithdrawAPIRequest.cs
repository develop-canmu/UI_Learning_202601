//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルド脱退

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GuildWithdrawAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class GuildWithdrawAPIResponse : AppAPIResponseBase {

   }
      
   public partial class GuildWithdrawAPIRequest : AppAPIRequestBase<GuildWithdrawAPIPost, GuildWithdrawAPIResponse> {
      public override string apiName{get{ return "guild/withdraw"; } }
   }
}