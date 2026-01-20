//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class GiftUnreceived {
		public long uGiftBoxId = 0; // プレゼントID（プレゼントを1件ずつ受け取りたい場合などに参照する）
		public string expireAt = ""; // 受取期限日時
		public bool isReceivable = false; // 受取可能なら真（受け取ることでアイテムの所持上限を超えてしまう場合などに受取不能になる）
		public string message = ""; // プレゼントの説明文言
		public PrizeJsonWrap[] prizeJson = null; // 報酬情報

   }
   
}