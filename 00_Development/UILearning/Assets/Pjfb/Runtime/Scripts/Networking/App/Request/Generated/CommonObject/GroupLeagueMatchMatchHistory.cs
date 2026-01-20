//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class GroupLeagueMatchMatchHistory {
		public long dayNumber = 0; // 試合番号
		public long matchType = 0; // 試合種別(1 => シーズン、 2 => 入れ替え)
		public long result = 0; // 試合結果（1 win 2 lose 3 draw）。試合が完了しきっていない場合は、一覧に出ない。
		public long winCount = 0; // 勝利数
		public long loseCount = 0; // 勝利数
		public long drawCount = 0; // 分け数
		public long winningPoint = 0; // 勝ち点
		public long winningPointOpponent = 0; // 対戦相手の勝ち点
		public string opponentName = ""; // 対戦相手名称
		public long opponentMGuildEmblemId = 0; // 対戦相手mGuildEmblemId
		public long groupType = 0; // groupType
		public long groupId = 0; // groupId
		public ResultDetailResultDetailArgs resultDetailArgs = null; // 対戦結果詳細
		public ResultDetailResultDetailArgs resultDetailArgsOpponent = null; // 対戦相手の対戦結果詳細
		public string battleStartAtSub = ""; // 対戦開始サブ時刻

   }
   
}