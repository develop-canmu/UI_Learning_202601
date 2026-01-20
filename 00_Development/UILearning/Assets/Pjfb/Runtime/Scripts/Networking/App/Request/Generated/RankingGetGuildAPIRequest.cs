//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルド間ポイントランキングを取得する

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class RankingGetGuildAPIPost : AppAPIPostBase {
		public long mPointId = 0; // ポイントID

   }

   [Serializable]
   public class RankingGetGuildAPIResponse : AppAPIResponseBase {
		public RankingGuild ranking = null; // ランキング情報

   }
      
   public partial class RankingGetGuildAPIRequest : AppAPIRequestBase<RankingGetGuildAPIPost, RankingGetGuildAPIResponse> {
      public override string apiName{get{ return "ranking/getGuild"; } }
   }
}