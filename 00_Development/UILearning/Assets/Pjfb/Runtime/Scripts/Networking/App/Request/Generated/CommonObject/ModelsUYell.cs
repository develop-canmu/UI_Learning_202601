//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class ModelsUYell {
		public long id = 0;
		public string createdAt = ""; // レコード登録日時
		public string updatedAt = ""; // レコード更新日時
		public long uMasterId = 0; // エール受け取りユーザーID
		public long fromUMasterId = 0; // エール送信ユーザーID
		public long cureStaminaValue = 0; // エールされる側の回復量
		public long fromCureStaminaValue = 0; // エールする側の回復量
		public long curedFlg = 0; // エールによる回復が完了したかどうか

   }
   
}