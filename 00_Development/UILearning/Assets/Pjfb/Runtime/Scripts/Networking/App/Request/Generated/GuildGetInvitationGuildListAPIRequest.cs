//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
勧誘ギルド一覧取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GuildGetInvitationGuildListAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class GuildGetInvitationGuildListAPIResponse : AppAPIResponseBase {
		public GuildInvitationInvitationStatus[] invitationList = null; // ギルド情報
		public long allowsGuildInvitation = 0; // ギルドからの勧誘許可設定

   }
      
   public partial class GuildGetInvitationGuildListAPIRequest : AppAPIRequestBase<GuildGetInvitationGuildListAPIPost, GuildGetInvitationGuildListAPIResponse> {
      public override string apiName{get{ return "guild/getInvitationGuildList"; } }
   }
}