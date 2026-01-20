//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
進行UI上のデッキ確認モーダルで出すやつ

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class BattleReserveFormationGetDeckDetailAPIPost : AppAPIPostBase {
		public long id = 0; // SBattleReserveFormationMatchingのID
		public long sideNumber = 0; // 1 => 左、 2 => 右
		public long roundNumber = 0; // 回戦番号

   }

   [Serializable]
   public class BattleReserveFormationGetDeckDetailAPIResponse : AppAPIResponseBase {
		public BattleReserveFormationPlayerInfo playerInfo = null; // 試合情報

   }
      
   public partial class BattleReserveFormationGetDeckDetailAPIRequest : AppAPIRequestBase<BattleReserveFormationGetDeckDetailAPIPost, BattleReserveFormationGetDeckDetailAPIResponse> {
      public override string apiName{get{ return "battle-reserve-formation/getDeckDetail"; } }
   }
}