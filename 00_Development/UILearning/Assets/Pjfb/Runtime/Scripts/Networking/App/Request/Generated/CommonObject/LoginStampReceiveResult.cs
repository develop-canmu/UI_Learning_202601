//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class LoginStampReceiveResult {
		public long mLoginStampId = 0; // どのログインボーナス種別か
		public long prizeNumber = 0; // どこまで進んでいるか（1始まり。対象のログインボーナスをどこまで受け取っているか）
		public long effectDate = 0; // web側で使用する、ログボの表示領域情報（APIレスポンスには含まれません）
		public bool isVisible = false; // 表示フラグ

   }
   
}