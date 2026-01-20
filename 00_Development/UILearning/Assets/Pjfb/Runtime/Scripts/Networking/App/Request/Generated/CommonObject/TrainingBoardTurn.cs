//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingBoardTurn {
		public long turn = 0; // ターン
		public long forkCount = 0; // 枝分かれ本数
		public bool isAdded = false; // ターン延長によって追加されたターンかどうか
		public TrainingBoardPiece[] boardPieceList = null; // 該当ターンに存在するマスの情報リスト
		public long selectedIndex = 0; // 選択したマス情報。未到達のターンに関しては0を返す

   }
   
}