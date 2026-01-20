//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ユーザーが送信しているギルド加入申請一覧取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GuildGetJoinRequestAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class GuildGetJoinRequestAPIResponse : AppAPIResponseBase {
		public GuildSearchGuildStatus[] guildList = null; // 自分が加入申請を送信したギルド情報一覧

   }
      
   public partial class GuildGetJoinRequestAPIRequest : AppAPIRequestBase<GuildGetJoinRequestAPIPost, GuildGetJoinRequestAPIResponse> {
      public override string apiName{get{ return "guild/getJoinRequest"; } }
   }
}