//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingAutoUserStatus {
		public long[] mTrainingAutoCostIdList = null; // 実行済みのm_training_auto_costのidリスト
		public long freeCompleteRemainCount = 0; // 即完了の無料残り回数
		public long completeRemainCount = 0; // 即完了の残り回数（無料含む）
		public long freeTimeShortenCount = 0; // 時間短縮の無料残り回数
		public long timeShortenCount = 0; // 時間短縮の残り回数（無料含む）
		public long freeAddStaminaCount = 0; // スタミナ回復の無料残り回数
		public long addStaminaCount = 0; // スタミナ回復の残り回数（無料含む）
		public long slotCount = 0; // 解放されているトレーニングの枠数
		public long maxSlotCount = 0; // 最大トレーニング枠数

   }
   
}