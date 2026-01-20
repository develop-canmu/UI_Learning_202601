//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingBoardCondition {
		public long conditionTier = 0; // コンディション帯
		public long minCondition = 0; // 最低コンディション値
		public TrainingBoardTurn[] boardTurnList = null; // 当コンディション帯のときに採用するマス盤情報

   }
   
}