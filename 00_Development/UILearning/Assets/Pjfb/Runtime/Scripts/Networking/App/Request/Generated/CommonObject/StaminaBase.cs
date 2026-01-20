//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class StaminaBase {
		public long mStaminaId = 0; // スタミナマスタID
		public long currentStamina = 0; // スタミナ現在値
		public string staminaCuredAt = ""; // スタミナ最終回復時刻
		public long additionStaminaUsed = 0; // 「追加使用日時」において使用した回数
		public string additionUseDate = ""; // 追加使用日時

   }
   
}