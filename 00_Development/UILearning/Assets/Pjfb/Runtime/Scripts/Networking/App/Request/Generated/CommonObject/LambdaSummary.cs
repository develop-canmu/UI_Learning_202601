//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class LambdaSummary {
		public long result = 0; // 試合の結果（-2 =>エラー等, 1=> 勝利, 2 => 敗北, 3 => 引き分け）
		public long pointGet = 0; // $pointGet 総得点
		public long pointLost = 0; // $pointLost 総失点
		public string errorMessage = ""; // エラーメッセージ

   }
   
}