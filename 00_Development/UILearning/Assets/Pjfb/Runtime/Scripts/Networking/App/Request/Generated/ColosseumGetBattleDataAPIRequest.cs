//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
バトルデータ取得（現状、特にバリデーションは噛ませていないが、必要そうならやる）

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class ColosseumGetBattleDataAPIPost : AppAPIPostBase {
		public long opponentUserType = 0; // 対戦相手のユーザー種別（1 => 通常、2 => NPC）
		public long opponentUserId = 0; // 対象ユーザーID（uMasterId, m_colosseum_npc.id）
		public long sColosseumEventId = 0; // シーズンのID（SeasonHome.idと同じもの）
		public long getTurn = 0; // （共通）ターンを取得するかどうか。0、なし => しない、 1 => する。※別途sColosseumEventIdを指定してる必要があります

   }

   [Serializable]
   public class ColosseumGetBattleDataAPIResponse : AppAPIResponseBase {
		public BattleV2ClientData clientData = null; // バトル時に使う共通構造
		public ColosseumScoreBattleTurn scoreBattleTurn = null; // （共通）ターン情報。開催期間外か何らかのトラブルが有る場合、取得できない。getTurnを指定時にのみ返す

   }
      
   public partial class ColosseumGetBattleDataAPIRequest : AppAPIRequestBase<ColosseumGetBattleDataAPIPost, ColosseumGetBattleDataAPIResponse> {
      public override string apiName{get{ return "colosseum/getBattleData"; } }
   }
}