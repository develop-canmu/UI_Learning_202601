//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルド勧誘許可ユーザー検索

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GuildSearchInvitationUserAPIPost : AppAPIPostBase {
		public string name = ""; // 名前
		public string combatPowerFrom = ""; // 総戦力の開始値
		public string combatPowerTo = ""; // 総戦力の終了値
		public long guildRank = 0; // 希望のギルドランク
		public long playStyleType = 0; // 希望のプレイスタイル
		public long guildBattleType = 0; // ギルドバトルの参加希望種別
		public long participationPriorityType = 0; // 参加優先度種別

   }

   [Serializable]
   public class GuildSearchInvitationUserAPIResponse : AppAPIResponseBase {
		public UserGuildInvitationUserStatus[] userList = null; // ユーザー情報

   }
      
   public partial class GuildSearchInvitationUserAPIRequest : AppAPIRequestBase<GuildSearchInvitationUserAPIPost, GuildSearchInvitationUserAPIResponse> {
      public override string apiName{get{ return "guild/searchInvitationUser"; } }
   }
}