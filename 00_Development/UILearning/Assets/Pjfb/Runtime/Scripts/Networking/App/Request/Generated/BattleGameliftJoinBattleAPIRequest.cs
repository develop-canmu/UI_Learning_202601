//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
バトル参加に必要な接続情報を返す

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class BattleGameliftJoinBattleAPIPost : AppAPIPostBase {
		public long id = 0; // 参加したい試合の sBattleGameliftMatchingId

   }

   [Serializable]
   public class BattleGameliftJoinBattleAPIResponse : AppAPIResponseBase {
		public BattleGameliftConnection connection = null; // GameLift対戦に参加する際に必要な接続情報

   }
      
   public partial class BattleGameliftJoinBattleAPIRequest : AppAPIRequestBase<BattleGameliftJoinBattleAPIPost, BattleGameliftJoinBattleAPIResponse> {
      public override string apiName{get{ return "battle-gamelift/joinBattle"; } }
   }
}