//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ガチャ保留引き直し

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GachaPendingRetryAPIPost : AppAPIPostBase {
		public long uGachaResultPendingId = 0; // 引き直し保留ID
		public long index = 0; // 引き直す枠のindex（0始まり）
		public long[] mPointIdAlternativeList = null; // 代替ポイントのidリスト

   }

   [Serializable]
   public class GachaPendingRetryAPIResponse : AppAPIResponseBase {
		public PrizeJsonWrap[] contentList = null; // ガチャ排出品を意味する配列（もともとの対象indexの排出品に上書きする形で使用）
		public GachaPendingFrame frame = null; // 保留枠情報の更新情報

   }
      
   public partial class GachaPendingRetryAPIRequest : AppAPIRequestBase<GachaPendingRetryAPIPost, GachaPendingRetryAPIResponse> {
      public override string apiName{get{ return "gacha/pendingRetry"; } }
   }
}