//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ユーザーランキングを取得する

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class RankingGetUserCommonRankListAPIPost : AppAPIPostBase {
		public long mRankingClientPreviewId = 0; // ランキングID

   }

   [Serializable]
   public class RankingGetUserCommonRankListAPIResponse : AppAPIResponseBase {
		public UserRankingUser ranking = null; // ランキング情報

   }
      
   public partial class RankingGetUserCommonRankListAPIRequest : AppAPIRequestBase<RankingGetUserCommonRankListAPIPost, RankingGetUserCommonRankListAPIResponse> {
      public override string apiName{get{ return "ranking/getUserCommonRankList"; } }
   }
}