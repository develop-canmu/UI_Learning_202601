//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
受取可能なプレゼントを全て受け取る

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GiftBoxReceiveAllAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class GiftBoxReceiveAllAPIResponse : AppAPIResponseBase {
		public long unreceivedGiftCount = 0; // 未受取のプレゼントの数
		public long unreceivedGiftLockedCount = 0; // 受け取り可能な時限式プレゼントの数
		public string newestGiftLockedAt = ""; // 直近受け取り可能な時限式プレゼントの時間
		public long unopenedGiftBoxCount = 0; // 未解放の時限式プレゼントの数

   }
      
   public partial class GiftBoxReceiveAllAPIRequest : AppAPIRequestBase<GiftBoxReceiveAllAPIPost, GiftBoxReceiveAllAPIResponse> {
      public override string apiName{get{ return "gift-box/receiveAll"; } }
   }
}