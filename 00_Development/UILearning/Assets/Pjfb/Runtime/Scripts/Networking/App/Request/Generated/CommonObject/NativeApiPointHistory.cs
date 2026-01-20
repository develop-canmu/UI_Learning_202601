//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class NativeApiPointHistory {
		public long mPointId = 0; // ポイントID
		public long value = 0; // 現在の所持数
		public string fluctuationRoute = ""; // 増減経路
		public bool isIncome = false; // true=>獲得、false=>消費
		public string createdAt = ""; // 日時

   }
   
}