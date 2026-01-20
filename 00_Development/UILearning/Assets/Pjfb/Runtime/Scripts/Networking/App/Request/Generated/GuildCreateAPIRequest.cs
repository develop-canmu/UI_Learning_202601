//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルド設立

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GuildCreateAPIPost : AppAPIPostBase {
		public string name = ""; // 名前
		public long mGuildEmblemId = 0; // エンブレムID
		public string introduction = ""; // ギルド紹介文
		public long mGuildPlayStyleId = 0; // 活動方針
		public long membersWantedFlg = 0; // メンバー募集フラグ 1=>募集する 2=>募集しない
		public string membersWantedComment = ""; // メンバー募集コメント
		public long autoEnrollmentFlg = 0; // 自動加入フラグ 1=>自動認証 2=>手動認証
		public long guildBattleStyleType = 0; // ギルドバトルマッチ方針種別
		public long participationPriorityType = 0; // 参加優先度種別

   }

   [Serializable]
   public class GuildCreateAPIResponse : AppAPIResponseBase {
		public GuildGuildStatus guild = null; // ギルド情報

   }
      
   public partial class GuildCreateAPIRequest : AppAPIRequestBase<GuildCreateAPIPost, GuildCreateAPIResponse> {
      public override string apiName{get{ return "guild/create"; } }
   }
}