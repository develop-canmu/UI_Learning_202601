//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingTrainingEvent {
		public long mTrainingEventId = 0; // イベントID
		public string name = ""; // 名前
		public long eventType = 0; // イベントタイプ。1: いわゆる「イベント」（シナリオや行動後イベント）、2: 行動、3: 休息、4: バトル（シナリオ由来カード由来の両方）
		public string scenarioNumber = ""; // 再生するシナリオの番号
		public long mTrainingBattleId = 0; // (eventType=4のみ)起きるバトルのID
		public long[] choiceList = null; // 選択可能な選択肢のリスト
		public long priorityChoiceId = 0; // 優先される選択肢
		public long[] choicePrizeList = null; // 選択可能な選択肢報酬のリスト

   }
   
}