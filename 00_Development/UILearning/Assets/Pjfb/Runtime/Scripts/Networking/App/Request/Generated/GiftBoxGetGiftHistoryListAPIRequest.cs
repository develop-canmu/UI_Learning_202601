//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
受取済みのプレゼントの一覧を取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GiftBoxGetGiftHistoryListAPIPost : AppAPIPostBase {
		public long page = 0; // 何ページ目のデータを取得するか（デフォルト値：1）

   }

   [Serializable]
   public class GiftBoxGetGiftHistoryListAPIResponse : AppAPIResponseBase {
		public GiftReceived[] giftList = null; // 受取済みのプレゼントの一覧
		public long maxPage = 0; // page の最大値（ページャの表示などに使う用）

   }
      
   public partial class GiftBoxGetGiftHistoryListAPIRequest : AppAPIRequestBase<GiftBoxGetGiftHistoryListAPIPost, GiftBoxGetGiftHistoryListAPIResponse> {
      public override string apiName{get{ return "gift-box/getGiftHistoryList"; } }
   }
}