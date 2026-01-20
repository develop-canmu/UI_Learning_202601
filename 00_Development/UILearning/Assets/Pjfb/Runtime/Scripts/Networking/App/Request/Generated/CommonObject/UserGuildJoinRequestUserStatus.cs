//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class UserGuildJoinRequestUserStatus {
		public long gJoinRequestId = 0; // ギルド加入申請ID
		public long uMasterId = 0; // ユーザID
		public string name = ""; // ユーザ名
		public long mIconId = 0; // 設定しているアイコンID
		public string maxCombatPower = ""; // 最大戦力
		public string lastLogin = ""; // ログイン日時
		public long mTitleId = 0; // 設定している称号ID。称号が未設定なら 0 になる
		public long maxDeckRank = 0; // 最大ランク
		public long participationPriorityType = 0; // ギルド参加優先度種別

   }
   
}