//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class ColosseumScoreBattleTurn {
		public long sColosseumEventId = 0; // シーズンID
		public long turnNumber = 0; // ターン番号。1始まり
		public string startAt = ""; // 開始日時
		public string endAt = ""; // 終了日時
		public WrapperIntList[] bonusRankList = null; //  int[][]構造。ターン終了時のスコアボーナスがかかる順位を予め控えておく。[[60, 1000], [1, 2000]]なら、60位に対して1.1、1位に対して1.2の補正がかかる。

   }
   
}