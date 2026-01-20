//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
デッキ設定

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class BattleReserveFormationSetDeckAPIPost : AppAPIPostBase {
		public long id = 0; // SBattleReserveFormationMatchingのID
		public long roundNumber = 0; // 回戦番号
		public long partyNumber = 0; // デッキ番号指定。0を指定すると、デッキを除外

   }

   [Serializable]
   public class BattleReserveFormationSetDeckAPIResponse : AppAPIResponseBase {
		public BattleReserveFormationMatchInfo matchInfo = null; // 試合情報

   }
      
   public partial class BattleReserveFormationSetDeckAPIRequest : AppAPIRequestBase<BattleReserveFormationSetDeckAPIPost, BattleReserveFormationSetDeckAPIResponse> {
      public override string apiName{get{ return "battle-reserve-formation/setDeck"; } }
   }
}