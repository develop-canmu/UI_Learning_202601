//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルド内ポイントランキングを取得する

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class RankingGetGuildInternalAPIPost : AppAPIPostBase {
		public long mPointId = 0; // ポイントID

   }

   [Serializable]
   public class RankingGetGuildInternalAPIResponse : AppAPIResponseBase {
		public RankingGuildInternal ranking = null; // ランキング情報

   }
      
   public partial class RankingGetGuildInternalAPIRequest : AppAPIRequestBase<RankingGetGuildInternalAPIPost, RankingGetGuildInternalAPIResponse> {
      public override string apiName{get{ return "ranking/getGuildInternal"; } }
   }
}