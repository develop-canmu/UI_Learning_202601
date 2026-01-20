//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
交換所の商品を購入する。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class ShopBuyStoreAPIPost : AppAPIPostBase {
		public long[] idList = null; // 交換所商品IDの配列
		public long[] countList = null; // 交換数量の配列

   }

   [Serializable]
   public class ShopBuyStoreAPIResponse : AppAPIResponseBase {
		public StoreBuyingInfo[] storeBuyingInfoList = null; // ユーザの交換所商品購入情報

   }
      
   public partial class ShopBuyStoreAPIRequest : AppAPIRequestBase<ShopBuyStoreAPIPost, ShopBuyStoreAPIResponse> {
      public override string apiName{get{ return "shop/buyStore"; } }
   }
}