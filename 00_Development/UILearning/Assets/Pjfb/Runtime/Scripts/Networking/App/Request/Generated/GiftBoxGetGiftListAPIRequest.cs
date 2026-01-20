//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
未受取のプレゼントの一覧を取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GiftBoxGetGiftListAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class GiftBoxGetGiftListAPIResponse : AppAPIResponseBase {
		public GiftUnreceived[] giftList = null; // 未受取のプレゼントの一覧

   }
      
   public partial class GiftBoxGetGiftListAPIRequest : AppAPIRequestBase<GiftBoxGetGiftListAPIPost, GiftBoxGetGiftListAPIResponse> {
      public override string apiName{get{ return "gift-box/getGiftList"; } }
   }
}