//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
受取可能な時限式プレゼントを全て受け取る

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GiftBoxReceiveLockedGiftAllAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class GiftBoxReceiveLockedGiftAllAPIResponse : AppAPIResponseBase {
		public long unreceivedGiftLockedCount = 0; // 未受取の時限式プレゼントの数
		public string newestGiftLockedAt = ""; // 未受取の時限式プレゼントの直近の受け取り可能日時
		public long unopenedGiftBoxCount = 0; // 未解放の時限式プレゼントの数

   }
      
   public partial class GiftBoxReceiveLockedGiftAllAPIRequest : AppAPIRequestBase<GiftBoxReceiveLockedGiftAllAPIPost, GiftBoxReceiveLockedGiftAllAPIResponse> {
      public override string apiName{get{ return "gift-box/receiveLockedGiftAll"; } }
   }
}