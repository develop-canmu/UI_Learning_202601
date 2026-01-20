//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class ColosseumRankingGroup {
		public long id = 0; // ID
		public long rankingChangeType = 0; // ランキング変動区分(1 => なし、 2 => 上昇、 3 => 下降、 4 => 変動なし)
		public long ranking = 0; // 順位
		public long scoreRanking = 0; // 順位
		public string name = ""; // 名前
		public long mGuildEmblemId = 0; // ギルドエンブレムID
		public string combatPower = ""; // シーズン開始時の戦力値
		public long score = 0; // スコア
		public long groupType = 0; // $groupType 1 => ギルド, 2 => NPCグループ（現在は1のみ対応）
		public long groupId = 0; // $groupId groupTypeに紐づくid（ギルドIDなど）
		public long winCount = 0; // 勝利数
		public long winCountSub = 0; // サブ勝利数

   }
   
}