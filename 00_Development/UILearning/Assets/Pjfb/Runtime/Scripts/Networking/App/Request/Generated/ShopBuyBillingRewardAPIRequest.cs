//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
課金パックを購入する。
これを呼び出す直前に native-api/shop/selectBillingReward を呼び出すことが前提となる。

スムーズなデバッグを行うためにレシート検証をスキップしたい場合は下記リンクから設定値を変更する。
<a href="/setting/searchResult?keyword=skipsReceiptValidation&type=1">skipsReceiptValidation</a>

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class ShopBuyBillingRewardAPIPost : AppAPIPostBase {
		public string receipt = ""; // ストアからのレシート
		public string productKey = ""; // プロダクトキー（レシート検証がスキップされる場合にのみ使用される）

   }

   [Serializable]
   public class ShopBuyBillingRewardAPIResponse : AppAPIResponseBase {
		public long monthPayment = 0; // 今月の課金額

   }
      
   public partial class ShopBuyBillingRewardAPIRequest : AppAPIRequestBase<ShopBuyBillingRewardAPIPost, ShopBuyBillingRewardAPIResponse> {
      public override string apiName{get{ return "shop/buyBillingReward"; } }
   }
}