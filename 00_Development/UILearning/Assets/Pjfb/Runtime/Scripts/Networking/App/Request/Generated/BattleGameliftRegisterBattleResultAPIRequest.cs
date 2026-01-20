//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
GameLiftサーバから対戦結果の登録を行う

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class BattleGameliftRegisterBattleResultAPIPost : AppAPIPostBase {
		public BattleGameliftMatchingResult gameliftMatchingResult = null; // GameLift対戦の試合結果

   }

   [Serializable]
   public class BattleGameliftRegisterBattleResultAPIResponse : AppAPIResponseBase {

   }
      
   public partial class BattleGameliftRegisterBattleResultAPIRequest : AppAPIRequestBase<BattleGameliftRegisterBattleResultAPIPost, BattleGameliftRegisterBattleResultAPIResponse> {
      public override string apiName{get{ return "battle-gamelift/registerBattleResult"; } }
   }
}