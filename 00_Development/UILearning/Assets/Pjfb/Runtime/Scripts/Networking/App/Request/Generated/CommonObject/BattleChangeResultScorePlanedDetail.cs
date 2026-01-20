//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleChangeResultScorePlanedDetail {
		public long scoreBase = 0; // 該当順位で得られるベーススコア
		public long intervalMinutes = 0; // 経過分数
		public long defenseCount = 0; // 防衛回数
		public long timeBonus = 0; // 経過時間ボーナス
		public long defenseBonus = 0; // 返り討ち数ボーナス
		public long turnBonus = 0; // ターンボーナス
		public long scorePlaned = 0; // 獲得予定スコア
		public long bonusRate = 0; // 合計ボーナス割合
		public long[] scoreList = null; // 獲得予定スコア部分に記載される、スコアの一覧
		public long[] turnRangeList = null; // 獲得予定スコア部分に記載される、ターンの一覧
		public bool isFuture = false; // 次の攻撃でまとめて得られるものか、1ターン経過後に得られるものか

   }
   
}