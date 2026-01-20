//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ボックスガチャの詳細情報を取得。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GachaBoxDetailAPIPost : AppAPIPostBase {
		public long mGachaCategoryId = 0; // ガチャカテゴリID

   }

   [Serializable]
   public class GachaBoxDetailAPIResponse : AppAPIResponseBase {
		public BoxInfo boxGachaInfo = null; // ガチャ提供割合情報

   }
      
   public partial class GachaBoxDetailAPIRequest : AppAPIRequestBase<GachaBoxDetailAPIPost, GachaBoxDetailAPIResponse> {
      public override string apiName{get{ return "gacha/boxDetail"; } }
   }
}