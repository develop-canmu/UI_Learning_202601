//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class GuildSearchGuildStatus {
		public long gMasterId = 0; // ギルドID
		public string name = ""; // ギルド名
		public string combatPower = ""; // 総戦力
		public long mGuildPlayStyleId = 0; // プレイスタイルID
		public long mGuildEmblemId = 0; // 設定しているギルドエンブレムID
		public long introduction = 0; // 紹介文
		public long mGuildRankId = 0; // ギルドランクID
		public long autoEnrollmentFlg = 0; // 自動加入フラグ 1=>自動認証 2=>手動認証
		public long guildBattleStyleType = 0; // ギルドバトル方針種別
		public string membersWantedComment = ""; // メンバー募集コメント
		public long numberOfPeople = 0; // ギルド人数
		public long guildMasterMIconId = 0; // ギルドマスターのアイコンID
		public long membersWantedFlg = 0; // メンバー募集フラグ
		public long participationPriorityType = 0; // 参加優先度種別

   }
   
}