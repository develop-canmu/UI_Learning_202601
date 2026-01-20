//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルドからの勧誘を承認

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GuildAgreeInvitationAPIPost : AppAPIPostBase {
		public long targetGMasterId = 0; // 承認するギルドID

   }

   [Serializable]
   public class GuildAgreeInvitationAPIResponse : AppAPIResponseBase {

   }
      
   public partial class GuildAgreeInvitationAPIRequest : AppAPIRequestBase<GuildAgreeInvitationAPIPost, GuildAgreeInvitationAPIResponse> {
      public override string apiName{get{ return "guild/agreeInvitation"; } }
   }
}