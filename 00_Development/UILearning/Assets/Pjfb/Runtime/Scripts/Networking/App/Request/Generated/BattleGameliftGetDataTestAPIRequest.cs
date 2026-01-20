//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
対戦に必要な情報を、ギルドIDから検索して渡す（テスト用）

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class BattleGameliftGetDataTestAPIPost : AppAPIPostBase {
		public long gMasterIdLeft = 0; // 1つ目のギルドID
		public long gMasterIdRight = 0; // 2つ目のギルドID
		public long mColosseumEventId = 0; // イベントID
		public long deckUseType = 0; // デッキ使用目的タイプ
		public long[] deckUseTypeListSub = null; // サブ使用デッキ指定。m_colosseum_event.deckUseTypeListSub に準拠する

   }

   [Serializable]
   public class BattleGameliftGetDataTestAPIResponse : AppAPIResponseBase {
		public BattleV2ClientData clientData = null; // バトル時に共通で使う、クライアント側に渡す情報構造体

   }
      
   public partial class BattleGameliftGetDataTestAPIRequest : AppAPIRequestBase<BattleGameliftGetDataTestAPIPost, BattleGameliftGetDataTestAPIResponse> {
      public override string apiName{get{ return "battle-gamelift/getDataTest"; } }
   }
}