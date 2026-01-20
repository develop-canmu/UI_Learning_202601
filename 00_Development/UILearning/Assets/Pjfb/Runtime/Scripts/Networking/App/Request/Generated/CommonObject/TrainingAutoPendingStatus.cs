//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingAutoPendingStatus {
		public long slotNumber = 0; // 枠番号
		public long mTrainingScenarioId = 0; // m_training_scenarioのid
		public long mCharaId = 0; // m_charaのid
		public long statusType = 0; // 育成方針
		public string finishAt = ""; // トレーニング終了時間
		public TrainingSupport[] supportDetailList = null; // サポートの詳細情報
		public bool isShorten = false; // 時間短縮しているか

   }
   
}