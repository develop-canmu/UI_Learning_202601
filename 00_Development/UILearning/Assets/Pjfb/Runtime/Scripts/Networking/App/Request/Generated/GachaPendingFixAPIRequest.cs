//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ガチャ保留確定

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GachaPendingFixAPIPost : AppAPIPostBase {
		public long uGachaResultPendingId = 0; // 引き直し保留ID

   }

   [Serializable]
   public class GachaPendingFixAPIResponse : AppAPIResponseBase {
		public long enabled = 0; // ガチャが活性状態かどうか。0 => 非活性、1 => 活性
		public bool canShowReview = false; // レビューの強制表示を行うか。0 => しない、1 => する

   }
      
   public partial class GachaPendingFixAPIRequest : AppAPIRequestBase<GachaPendingFixAPIPost, GachaPendingFixAPIResponse> {
      public override string apiName{get{ return "gacha/pendingFix"; } }
   }
}