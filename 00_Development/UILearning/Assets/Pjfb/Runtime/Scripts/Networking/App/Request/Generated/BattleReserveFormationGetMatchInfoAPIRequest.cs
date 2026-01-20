//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
試合進行UIに表示する基本データを返却します。
　

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class BattleReserveFormationGetMatchInfoAPIPost : AppAPIPostBase {
		public long id = 0; // SBattleReserveFormationMatchingのID

   }

   [Serializable]
   public class BattleReserveFormationGetMatchInfoAPIResponse : AppAPIResponseBase {
		public BattleReserveFormationMatchInfo matchInfo = null; // 試合情報

   }
      
   public partial class BattleReserveFormationGetMatchInfoAPIRequest : AppAPIRequestBase<BattleReserveFormationGetMatchInfoAPIPost, BattleReserveFormationGetMatchInfoAPIResponse> {
      public override string apiName{get{ return "battle-reserve-formation/getMatchInfo"; } }
   }
}