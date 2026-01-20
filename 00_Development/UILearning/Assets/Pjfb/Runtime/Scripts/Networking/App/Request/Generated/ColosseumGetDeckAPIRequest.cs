//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
特定の相手のデッキ情報を得る。
getRankingやgetTargetList等で得た情報にはデッキ情報を内包していないので、アコーディオン等で開く場合は都度このAPIで情報を得るようにする。
また、バトル直前の画面でも、こちらのレスポンスを使ってもらう想定。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class ColosseumGetDeckAPIPost : AppAPIPostBase {
		public long sColosseumEventId = 0; // シーズンのID（SeasonHome.idと同じもの）
		public long targetUMasterId = 0; // 対象ユーザーID
		public long userType = 0; // ユーザー区分（1 => 通常、 2 => npc）
		public long getTurn = 0; // （共通）ターンを取得するかどうか。0、なし => しない、 1 => する。※別途sColosseumEventIdを指定してる必要があります

   }

   [Serializable]
   public class ColosseumGetDeckAPIResponse : AppAPIResponseBase {
		public ColosseumDeck deck = null; // 指定の相手のデッキ
		public ColosseumScoreBattleTurn scoreBattleTurn = null; // （共通）ターン情報。開催期間外か何らかのトラブルが有る場合、取得できない。getTurnを指定時にのみ返す

   }
      
   public partial class ColosseumGetDeckAPIRequest : AppAPIRequestBase<ColosseumGetDeckAPIPost, ColosseumGetDeckAPIResponse> {
      public override string apiName{get{ return "colosseum/getDeck"; } }
   }
}