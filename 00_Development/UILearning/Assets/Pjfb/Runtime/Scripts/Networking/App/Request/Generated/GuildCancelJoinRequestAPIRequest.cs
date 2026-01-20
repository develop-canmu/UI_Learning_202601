//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルド加入申請をキャンセルする

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GuildCancelJoinRequestAPIPost : AppAPIPostBase {
		public long targetGMasterId = 0; // 対象のギルドID

   }

   [Serializable]
   public class GuildCancelJoinRequestAPIResponse : AppAPIResponseBase {

   }
      
   public partial class GuildCancelJoinRequestAPIRequest : AppAPIRequestBase<GuildCancelJoinRequestAPIPost, GuildCancelJoinRequestAPIResponse> {
      public override string apiName{get{ return "guild/cancelJoinRequest"; } }
   }
}