//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
シナリオ別追加報酬情報の取得。サーバ側で1分間のキャッシュを取っている

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class FestivalGetPrizeStatusListAPIPost : AppAPIPostBase {
		public long mTrainingScenarioId = 0; // シナリオID

   }

   [Serializable]
   public class FestivalGetPrizeStatusListAPIResponse : AppAPIResponseBase {
		public FestivalPrizeStatus[] prizeStatusList = null; // イベントごとの追加報酬情報リスト

   }
      
   public partial class FestivalGetPrizeStatusListAPIRequest : AppAPIRequestBase<FestivalGetPrizeStatusListAPIPost, FestivalGetPrizeStatusListAPIResponse> {
      public override string apiName{get{ return "festival/getPrizeStatusList"; } }
   }
}