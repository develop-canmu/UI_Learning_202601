//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルド情報取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GuildGetGuildAPIPost : AppAPIPostBase {
		public long gMasterId = 0; // 対象のギルドID。自分の所属ギルド情報を取得する場合は0

   }

   [Serializable]
   public class GuildGetGuildAPIResponse : AppAPIResponseBase {
		public GuildGuildStatus guild = null; // ギルド情報
		public GuildBattleMatchingMatchingStatus[] guildBattleMatchingList = null; // ギルドバトルマッチングリスト
		public GuildSearchGuildStatus[] joinRequestGuildList = null; // 自分が加入申請を送信したギルド情報一覧

   }
      
   public partial class GuildGetGuildAPIRequest : AppAPIRequestBase<GuildGetGuildAPIPost, GuildGetGuildAPIResponse> {
      public override string apiName{get{ return "guild/getGuild"; } }
   }
}