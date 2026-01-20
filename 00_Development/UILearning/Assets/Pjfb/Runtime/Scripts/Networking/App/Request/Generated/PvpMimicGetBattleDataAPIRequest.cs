//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
対戦相手を指定しバトルデータを得る

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class PvpMimicGetBattleDataAPIPost : AppAPIPostBase {
		public long opponentUMasterId = 0; // 対戦相手のユーザーID

   }

   [Serializable]
   public class PvpMimicGetBattleDataAPIResponse : AppAPIResponseBase {
		public BattleV2ClientData clientData = null; // バトル時に使う共通構造

   }
      
   public partial class PvpMimicGetBattleDataAPIRequest : AppAPIRequestBase<PvpMimicGetBattleDataAPIPost, PvpMimicGetBattleDataAPIResponse> {
      public override string apiName{get{ return "pvp-mimic/getBattleData"; } }
   }
}