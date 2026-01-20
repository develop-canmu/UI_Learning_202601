//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
プレゼントをひとつ指定して受け取る

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GiftBoxReceiveAPIPost : AppAPIPostBase {
		public long uGiftBoxId = 0; // プレゼントID

   }

   [Serializable]
   public class GiftBoxReceiveAPIResponse : AppAPIResponseBase {
		public long unreceivedGiftCount = 0; // 未受取のプレゼントの数
		public long unreceivedGiftLockedCount = 0; // 受け取り可能な時限式プレゼントの数
		public string newestGiftLockedAt = ""; // 直近受け取り可能な時限式プレゼントの時間
		public long unopenedGiftBoxCount = 0; // 未解放の時限式プレゼントの数

   }
      
   public partial class GiftBoxReceiveAPIRequest : AppAPIRequestBase<GiftBoxReceiveAPIPost, GiftBoxReceiveAPIResponse> {
      public override string apiName{get{ return "gift-box/receive"; } }
   }
}