//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class GuildGuildStatus {
		public long gMasterId = 0; // ギルドID
		public string name = ""; // ギルド名
		public string introduction = ""; // ギルド紹介
		public long mGuildEmblemId = 0; // 設定しているギルドエンブレムID
		public long mGuildPlayStyleId = 0; // 活動方針
		public long autoEnrollmentFlg = 0; // 入団条件
		public string combatPower = ""; // 総戦力
		public string tactics = ""; // 戦略 所属していないギルドの場合は返却しない
		public long mGuildRankId = 0; // 現在のギルドランクID
		public long nextGuildRankPoint = 0; // 次のギルドランクポイント
		public long guildRankPoint = 0; // 現在のギルドランクポイント
		public long guildBattleStyleType = 0; // ギルドバトルマッチ方針種別
		public long membersWantedFlg = 0; // メンバー募集フラグ
		public string membersWantedComment = ""; // メンバー募集メッセージ
		public GuildMemberMemberStatus[] guildMemberList = null; // ギルドメンバーリスト
		public long[] mGuildEmblemIdList = null; // ギルドエンブレムIDリスト
		public UserGuildJoinRequestUserStatus[] joinRequestList = null; // ギルド加入申請リスト
		public string myJoinAt = ""; // ギルド加入日時
		public long participationPriorityType = 0; // 参加優先度種別

   }
   
}