//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingIntentionalEvent {
		public long mTrainingIntentionalEventId = 0; // 任意イベント設定ID
		public long mTrainingEventId = 0; // イベントID
		public long rarity = 0; // 発生のレア度
		public long eventType = 0; // イベント種別
		public long mTrainingBattleId = 0; // 試合ID
		public long expectedMTrainingEventRewardId = 0; // イベントにより獲得できる mTrainingEventReward のID。mTrainingIntentionalEvent のプロパティを流用している
		public BattleV2ClientData clientData = null; // バトル用クライアントデータ

   }
   
}