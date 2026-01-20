//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルド勧誘設定

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GuildUpdateUserInvitationAPIPost : AppAPIPostBase {
		public bool allowsGuildInvitation = false; // 勧誘を受け取るか false=>受け取らない、ture=>受け取る
		public long guildInvitationGuildRank = 0; // 希望のギルドランク
		public long guildInvitationPlayStyleType = 0; // 希望のプレイスタイル
		public long guildInvitationGuildBattleType = 0; // ギルドバトルの参加希望種別
		public string guildInvitationMessage = ""; // 勧誘アピールコメント
		public long participationPriorityType = 0; // ギルド参加優先度種別

   }

   [Serializable]
   public class GuildUpdateUserInvitationAPIResponse : AppAPIResponseBase {

   }
      
   public partial class GuildUpdateUserInvitationAPIRequest : AppAPIRequestBase<GuildUpdateUserInvitationAPIPost, GuildUpdateUserInvitationAPIResponse> {
      public override string apiName{get{ return "guild/updateUserInvitation"; } }
   }
}