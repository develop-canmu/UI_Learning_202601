//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
日替わりで商品が変わる交換所の商品情報を取得する。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class ShopGetLotteryStoreInfoAPIPost : AppAPIPostBase {
		public long mCommonStoreCategoryId = 0; // 交換所ID

   }

   [Serializable]
   public class ShopGetLotteryStoreInfoAPIResponse : AppAPIResponseBase {
		public long[] mCommonStoreIdList = null; // 購入可能な交換所商品のIDの配列
		public long lottedCount = 0; // ユーザが再抽選を行った回数（日付変更に伴う自動的な再抽選は回数に含めない）
		public string lastLottedAt = ""; // 最後に抽選が行われた日時

   }
      
   public partial class ShopGetLotteryStoreInfoAPIRequest : AppAPIRequestBase<ShopGetLotteryStoreInfoAPIPost, ShopGetLotteryStoreInfoAPIResponse> {
      public override string apiName{get{ return "shop/getLotteryStoreInfo"; } }
   }
}