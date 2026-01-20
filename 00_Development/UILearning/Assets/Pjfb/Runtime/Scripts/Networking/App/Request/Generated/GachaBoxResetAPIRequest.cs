//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ボックスガチャのリセット。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GachaBoxResetAPIPost : AppAPIPostBase {
		public long mGachaCategoryId = 0; // ガチャカテゴリID

   }

   [Serializable]
   public class GachaBoxResetAPIResponse : AppAPIResponseBase {
		public long boxContentCount = 0; // リセット後のボックスに入っているアイテムの合計数
		public bool canResetBox = false; // リセット後のボックスが既にリセット可能なら真

   }
      
   public partial class GachaBoxResetAPIRequest : AppAPIRequestBase<GachaBoxResetAPIPost, GachaBoxResetAPIResponse> {
      public override string apiName{get{ return "gacha/boxReset"; } }
   }
}