//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingGoal {
		public long goalTurnCount = 0; // 目標のターン数
		public long turnCountDiff = 0; // 前の目標からのターン数の差分
		public long timing = 0; // 目標のタイミング
		public long restTurnCount = 0; // 残りターン数
		public long restFirstAddedTurnCount = 0; // 初期延長分の残り加算ターン数
		public long restAllAddedTurnCount = 0; // 全体延長分の残り加算ターン数
		public long addedTurn = 0; // 加算ターン数
		public long firstAddedTurn = 0; // 初回に追加されるターン数
		public long mTrainingEventId = 0; // イベントID
		public string goalDescription = ""; // 目標の簡易説明
		public long mCharaVariableConditionId = 0; // イベントの発生条件ID
		public long state = 0; // 目標の達成状態。1: 達成済み。2: 未達成。3: 未達成のまま先へ進行中
		public string bgmCueName = ""; // BGMキュー名

   }
   
}