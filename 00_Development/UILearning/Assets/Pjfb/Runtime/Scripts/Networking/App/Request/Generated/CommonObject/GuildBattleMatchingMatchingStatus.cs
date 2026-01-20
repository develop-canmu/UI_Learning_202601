//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class GuildBattleMatchingMatchingStatus {
		public long gMasterId = 0; // ギルドID
		public long opponentGMasterId = 0; // 対戦相手ギルドID
		public string opponentGuildName = ""; // 対戦相手名
		public long resultType = 0; // バトル結果。1 => 勝利, 2 => 敗北, 3 => 引き分け, 4 => 未決着
		public string startAt = ""; // ギルドバトル開始時間

   }
   
}