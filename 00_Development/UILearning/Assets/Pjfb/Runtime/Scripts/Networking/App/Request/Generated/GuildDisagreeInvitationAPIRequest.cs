//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルドからの勧誘を拒否

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GuildDisagreeInvitationAPIPost : AppAPIPostBase {
		public long targetGMasterId = 0; // 勧誘を拒否する対象のギルドID

   }

   [Serializable]
   public class GuildDisagreeInvitationAPIResponse : AppAPIResponseBase {

   }
      
   public partial class GuildDisagreeInvitationAPIRequest : AppAPIRequestBase<GuildDisagreeInvitationAPIPost, GuildDisagreeInvitationAPIResponse> {
      public override string apiName{get{ return "guild/disagreeInvitation"; } }
   }
}