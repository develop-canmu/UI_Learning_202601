//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルド検索

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GuildSearchGuildAPIPost : AppAPIPostBase {
		public string name = ""; // 名前
		public long mGuildRankIdFrom = 0; // ギルドランクID開始値
		public long mGuildRankIdTo = 0; // ギルドランクID終了値
		public long numberOfPeopleFrom = 0; // 所属人数開始値
		public long numberOfPeopleTo = 0; // 所属人数終了値
		public long mGuildPlayStyleId = 0; // 活動方針ID
		public long autoEnrollmentFlg = 0; // 自動加入フラグ 1=>自動認証 2=>手動認証
		public long membersWantedFlg = 0; // メンバー募集フラグ 1=>募集する 2=>募集しない
		public long guildBattleStyleType = 0; // ギルドバトルマッチ方針種別
		public long participationPriorityType = 0; // 参加優先度種別

   }

   [Serializable]
   public class GuildSearchGuildAPIResponse : AppAPIResponseBase {
		public GuildSearchGuildStatus[] guildList = null; // ギルド情報リスト

   }
      
   public partial class GuildSearchGuildAPIRequest : AppAPIRequestBase<GuildSearchGuildAPIPost, GuildSearchGuildAPIResponse> {
      public override string apiName{get{ return "guild/searchGuild"; } }
   }
}