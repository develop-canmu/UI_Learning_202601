//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class GiftUnreceivedLockGift {
		public long uGiftBoxLockedId = 0; // 時限式のプレゼントID（プレゼントを1件ずつ受け取りたい場合などに参照する）
		public string message = ""; // メッセージ
		public PrizeJsonWrap[] prizeJson = null; // 報酬情報
		public string openAt = ""; // プレゼントが受け取れるようになる日時
		public string expireAt = ""; // 受取期限日時
		public bool isReceivable = false; // 受取可能なら真（受け取ることでアイテムの所持上限を超えてしまう場合などに受取不能になる）

   }
   
}