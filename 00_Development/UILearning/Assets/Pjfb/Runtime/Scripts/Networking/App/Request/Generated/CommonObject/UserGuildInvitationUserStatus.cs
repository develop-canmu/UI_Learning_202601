//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class UserGuildInvitationUserStatus {
		public long uMasterId = 0; // ユーザID
		public string name = ""; // ユーザ名
		public long mIconId = 0; // 設定しているアイコンID
		public string maxCombatPower = ""; // 最大戦力
		public string lastLogin = ""; // ログイン日時
		public long mTitleId = 0; // 設定している称号ID。称号が未設定なら 0 になる
		public long maxDeckRank = 0; // 最大ランク
		public long guildRank = 0; // 希望ギルドランク
		public long playStyleType = 0; // 希望プレイスタイル
		public long guildBattleType = 0; // 希望ギルドバトルのプレイスタイル
		public string message = ""; // 勧誘アピールコメント
		public long participationPriorityType = 0; // ギルド参加優先度種別

   }
   
}