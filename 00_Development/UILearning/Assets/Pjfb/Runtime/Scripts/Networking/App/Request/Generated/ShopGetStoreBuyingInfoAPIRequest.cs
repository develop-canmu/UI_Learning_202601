//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ユーザの交換所商品購入情報を取得する（全店分をまとめて取得する）

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class ShopGetStoreBuyingInfoAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class ShopGetStoreBuyingInfoAPIResponse : AppAPIResponseBase {
		public StoreBuyingInfo[] storeBuyingInfoList = null; // ユーザの交換所商品購入情報の配列

   }
      
   public partial class ShopGetStoreBuyingInfoAPIRequest : AppAPIRequestBase<ShopGetStoreBuyingInfoAPIPost, ShopGetStoreBuyingInfoAPIResponse> {
      public override string apiName{get{ return "shop/getStoreBuyingInfo"; } }
   }
}