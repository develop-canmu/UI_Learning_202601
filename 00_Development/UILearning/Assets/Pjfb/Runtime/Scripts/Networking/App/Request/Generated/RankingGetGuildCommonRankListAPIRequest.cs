//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルドランキングを取得する

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class RankingGetGuildCommonRankListAPIPost : AppAPIPostBase {
		public long mRankingClientPreviewId = 0; // ランキングID

   }

   [Serializable]
   public class RankingGetGuildCommonRankListAPIResponse : AppAPIResponseBase {
		public UserRankingGuild ranking = null; // ランキング情報

   }
      
   public partial class RankingGetGuildCommonRankListAPIRequest : AppAPIRequestBase<RankingGetGuildCommonRankListAPIPost, RankingGetGuildCommonRankListAPIResponse> {
      public override string apiName{get{ return "ranking/getGuildCommonRankList"; } }
   }
}