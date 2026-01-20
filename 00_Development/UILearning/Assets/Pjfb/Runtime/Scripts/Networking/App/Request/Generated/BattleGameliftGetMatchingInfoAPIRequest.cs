//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
GameLiftサーバから対戦に必要なデッキなどの情報取得を行う

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class BattleGameliftGetMatchingInfoAPIPost : AppAPIPostBase {
		public long id = 0; // 開催したい試合の sBattleGameliftMatchingId

   }

   [Serializable]
   public class BattleGameliftGetMatchingInfoAPIResponse : AppAPIResponseBase {
		public BattleGameliftMatchingInfo matching = null; // GameLift対戦の1試合に必要なデータ

   }
      
   public partial class BattleGameliftGetMatchingInfoAPIRequest : AppAPIRequestBase<BattleGameliftGetMatchingInfoAPIPost, BattleGameliftGetMatchingInfoAPIResponse> {
      public override string apiName{get{ return "battle-gamelift/getMatchingInfo"; } }
   }
}