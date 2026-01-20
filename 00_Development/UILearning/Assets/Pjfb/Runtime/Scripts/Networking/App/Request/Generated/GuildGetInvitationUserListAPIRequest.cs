//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
勧誘中ユーザー一覧取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GuildGetInvitationUserListAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class GuildGetInvitationUserListAPIResponse : AppAPIResponseBase {
		public UserGuildInvitationUserStatus[] userList = null; // 勧誘中ユーザーリスト

   }
      
   public partial class GuildGetInvitationUserListAPIRequest : AppAPIRequestBase<GuildGetInvitationUserListAPIPost, GuildGetInvitationUserListAPIResponse> {
      public override string apiName{get{ return "guild/getInvitationUserList"; } }
   }
}