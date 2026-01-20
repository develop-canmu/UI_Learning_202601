//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
時限式プレゼントをひとつ指定して受け取る

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GiftBoxReceiveLockedGiftAPIPost : AppAPIPostBase {
		public long uGiftBoxLockedId = 0; // プレゼントID

   }

   [Serializable]
   public class GiftBoxReceiveLockedGiftAPIResponse : AppAPIResponseBase {
		public long unreceivedGiftLockedCount = 0; // 未受取の時限式プレゼントの数
		public string newestGiftLockedAt = ""; // 未受取の時限式プレゼントの直近の受け取り可能日時
		public long unopenedGiftBoxCount = 0; // 未解放の時限式プレゼントの数

   }
      
   public partial class GiftBoxReceiveLockedGiftAPIRequest : AppAPIRequestBase<GiftBoxReceiveLockedGiftAPIPost, GiftBoxReceiveLockedGiftAPIResponse> {
      public override string apiName{get{ return "gift-box/receiveLockedGift"; } }
   }
}