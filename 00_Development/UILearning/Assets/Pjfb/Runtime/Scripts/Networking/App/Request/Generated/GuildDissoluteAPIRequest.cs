//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルド解散

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GuildDissoluteAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class GuildDissoluteAPIResponse : AppAPIResponseBase {

   }
      
   public partial class GuildDissoluteAPIRequest : AppAPIRequestBase<GuildDissoluteAPIPost, GuildDissoluteAPIResponse> {
      public override string apiName{get{ return "guild/dissolute"; } }
   }
}