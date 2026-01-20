//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
履歴のデッキ情報。取得ができない場合はnullが帰る。
getHistoryListでとる一覧には、デッキ情報を内包していないため、必要であればこちらを実行して取得する。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class ColosseumGetHistoryDeckAPIPost : AppAPIPostBase {
		public long uColosseumBattleHistoryId = 0; // バトル履歴のID
		public long getTurn = 0; // （共通）ターンを取得するかどうか。0、なし => しない、 1 => する。※別途sColosseumEventIdを指定してる必要があります

   }

   [Serializable]
   public class ColosseumGetHistoryDeckAPIResponse : AppAPIResponseBase {
		public ColosseumDeck deck = null; // 指定の相手のデッキ
		public ColosseumScoreBattleTurn scoreBattleTurn = null; // （共通）ターン情報。開催期間外か何らかのトラブルが有る場合、取得できない。getTurnを指定時にのみ返す

   }
      
   public partial class ColosseumGetHistoryDeckAPIRequest : AppAPIRequestBase<ColosseumGetHistoryDeckAPIPost, ColosseumGetHistoryDeckAPIResponse> {
      public override string apiName{get{ return "colosseum/getHistoryDeck"; } }
   }
}