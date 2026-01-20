//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TopDiscount {
		public long id = 0; // 割引ID
		public string description = ""; // 説明文
		public long price = 0; // 割引後の金額
		public long restCount = 0; // 残使用回数
		public long mGachaCategoryId = 0; // ガチャカテゴリID。クライアント側には渡らない
		public long sortNumber = 0; // ソート番号。クライアント側には渡らない
		public long useCountType = 0; // 実施回数換算区分。クライアント側には渡らない
		public long availableCount = 0; // 実施可能回数。クライアント側には渡らない
		public long conditionType = 0; // 条件種別。クライアント側には渡らない
		public long conditionValue = 0; // 条件値。クライアント側には渡らない
		public string conditionJson = ""; // 条件Json。クライアント側には渡らない

   }
   
}