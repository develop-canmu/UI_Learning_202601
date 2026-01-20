//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルド加入承認

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GuildAgreeJoinRequestAPIPost : AppAPIPostBase {
		public long gJoinRequestId = 0; // 加入申請のID

   }

   [Serializable]
   public class GuildAgreeJoinRequestAPIResponse : AppAPIResponseBase {

   }
      
   public partial class GuildAgreeJoinRequestAPIRequest : AppAPIRequestBase<GuildAgreeJoinRequestAPIPost, GuildAgreeJoinRequestAPIResponse> {
      public override string apiName{get{ return "guild/agreeJoinRequest"; } }
   }
}