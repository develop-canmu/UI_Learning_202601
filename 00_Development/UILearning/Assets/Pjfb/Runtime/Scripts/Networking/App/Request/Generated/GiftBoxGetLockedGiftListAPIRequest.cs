//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
未受取の時限式プレゼントの一覧を取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GiftBoxGetLockedGiftListAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class GiftBoxGetLockedGiftListAPIResponse : AppAPIResponseBase {
		public GiftUnreceivedLockGift[] giftList = null; // 未受取のプレゼントの一覧
		public long unreceivedGiftLockedCount = 0; // 未受取の時限式プレゼントの数
		public string newestGiftLockedAt = ""; // 未受取の時限式プレゼントの直近の受け取り可能日時
		public long unopenedGiftBoxCount = 0; // 未解放の時限式プレゼントの数

   }
      
   public partial class GiftBoxGetLockedGiftListAPIRequest : AppAPIRequestBase<GiftBoxGetLockedGiftListAPIPost, GiftBoxGetLockedGiftListAPIResponse> {
      public override string apiName{get{ return "gift-box/getLockedGiftList"; } }
   }
}