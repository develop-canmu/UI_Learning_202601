//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class ColosseumShiftMatchInfo {
		public long sColosseumGroupStatusId = 0; // 対戦相手のグループID
		public string name = ""; // 対戦相手の名称
		public long mGuildEmblemId = 0; // 対戦相手のギルドエンブレムID
		public long gradeNumber = 0; // 対戦相手のグレード（入れ替え戦前段階。昇格戦か降格戦か。試合の結果、昇格か降格か残留か…などは一旦送らない想定なので、自軍のグレードとこの値を比較して出してもらう想定）
		public long result = 0; // 結果（1 => 勝利, 2 => 敗北, 3 => 引き分け, 99 => 未実施）

   }
   
}