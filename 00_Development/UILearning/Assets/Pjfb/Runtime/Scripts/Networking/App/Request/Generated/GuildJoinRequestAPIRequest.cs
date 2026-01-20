//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルド加入申請

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GuildJoinRequestAPIPost : AppAPIPostBase {
		public long targetGMasterId = 0; // 対象のギルドID

   }

   [Serializable]
   public class GuildJoinRequestAPIResponse : AppAPIResponseBase {

   }
      
   public partial class GuildJoinRequestAPIRequest : AppAPIRequestBase<GuildJoinRequestAPIPost, GuildJoinRequestAPIResponse> {
      public override string apiName{get{ return "guild/joinRequest"; } }
   }
}