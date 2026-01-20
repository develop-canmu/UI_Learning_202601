//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
勧誘を取り下げる

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GuildCancelInvitationAPIPost : AppAPIPostBase {
		public long targetUMasterId = 0; // 取り下げる対象のユーザーID

   }

   [Serializable]
   public class GuildCancelInvitationAPIResponse : AppAPIResponseBase {

   }
      
   public partial class GuildCancelInvitationAPIRequest : AppAPIRequestBase<GuildCancelInvitationAPIPost, GuildCancelInvitationAPIResponse> {
      public override string apiName{get{ return "guild/cancelInvitation"; } }
   }
}