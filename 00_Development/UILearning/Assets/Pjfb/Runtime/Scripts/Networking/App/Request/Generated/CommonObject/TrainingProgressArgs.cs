//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingProgressArgs {
		public long mTrainingIntentionalEventId = 0; // 任意イベントID。任意イベントを選択する場合のみ使用する
		public long activityRank = 0; // 活躍度順位。バトルイベントの結果を送信する場合のみ使用する
		public long isScenarioSkip = 0; // シナリオスキップするか。スキップする場合は選択肢を自動で選択する。1=>スキップする、2=>しない

   }
   
}