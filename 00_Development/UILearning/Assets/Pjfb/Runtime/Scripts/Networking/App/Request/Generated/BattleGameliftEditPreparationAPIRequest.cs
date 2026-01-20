//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
GameLift対戦の事前準備設定を編集する

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class BattleGameliftEditPreparationAPIPost : AppAPIPostBase {
		public long mBattleGameliftId = 0; // 設定を行うGameLift対戦マスタID
		public long eventType = 0; // GameLift対戦のイベント開催形態。 colosseum なら 1 を指定する
		public long masterId = 0; // イベントID。 colosseum なら m_colosseum_event.id を指定する
		public PlayerGameliftOptionSetting setting = null; // 事前準備設定の内容

   }

   [Serializable]
   public class BattleGameliftEditPreparationAPIResponse : AppAPIResponseBase {

   }
      
   public partial class BattleGameliftEditPreparationAPIRequest : AppAPIRequestBase<BattleGameliftEditPreparationAPIPost, BattleGameliftEditPreparationAPIResponse> {
      public override string apiName{get{ return "battle-gamelift/editPreparation"; } }
   }
}