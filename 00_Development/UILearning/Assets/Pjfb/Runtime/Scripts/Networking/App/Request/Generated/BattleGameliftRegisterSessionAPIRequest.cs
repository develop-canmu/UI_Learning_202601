//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
GameLiftサーバからセッションの登録を行う

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class BattleGameliftRegisterSessionAPIPost : AppAPIPostBase {
		public BattleGameliftSession session = null; // 登録したいセッションに関する情報

   }

   [Serializable]
   public class BattleGameliftRegisterSessionAPIResponse : AppAPIResponseBase {

   }
      
   public partial class BattleGameliftRegisterSessionAPIRequest : AppAPIRequestBase<BattleGameliftRegisterSessionAPIPost, BattleGameliftRegisterSessionAPIResponse> {
      public override string apiName{get{ return "battle-gamelift/registerSession"; } }
   }
}