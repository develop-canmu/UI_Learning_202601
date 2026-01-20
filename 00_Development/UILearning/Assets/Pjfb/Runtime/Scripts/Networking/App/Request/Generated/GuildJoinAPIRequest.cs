//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルド加入

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GuildJoinAPIPost : AppAPIPostBase {
		public long targetGMasterId = 0; // 対象のギルドID

   }

   [Serializable]
   public class GuildJoinAPIResponse : AppAPIResponseBase {

   }
      
   public partial class GuildJoinAPIRequest : AppAPIRequestBase<GuildJoinAPIPost, GuildJoinAPIResponse> {
      public override string apiName{get{ return "guild/join"; } }
   }
}