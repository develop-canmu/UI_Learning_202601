//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
未読の大会終了記録を既読に変更

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class ColosseumReadFinishedListAPIPost : AppAPIPostBase {
		public long[] sColosseumEventIdList = null; // シーズンのID一覧（SeasonHome.idと同じもの）
		public long getTurn = 0; // （共通）ターンを取得するかどうか。0、なし => しない、 1 => する。※別途sColosseumEventIdを指定してる必要があります

   }

   [Serializable]
   public class ColosseumReadFinishedListAPIResponse : AppAPIResponseBase {
		public ColosseumScoreBattleTurn scoreBattleTurn = null; // （共通）ターン情報。開催期間外か何らかのトラブルが有る場合、取得できない。getTurnを指定時にのみ返す

   }
      
   public partial class ColosseumReadFinishedListAPIRequest : AppAPIRequestBase<ColosseumReadFinishedListAPIPost, ColosseumReadFinishedListAPIResponse> {
      public override string apiName{get{ return "colosseum/readFinishedList"; } }
   }
}