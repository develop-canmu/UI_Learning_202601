//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
リーグ戦の場合、リーグ表を得る

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class ColosseumGetGroupLeagueBoardAPIPost : AppAPIPostBase {
		public long sColosseumEventId = 0; // シーズンのID（SeasonHome.idと同じもの）
		public long getTurn = 0; // （共通）ターンを取得するかどうか。0、なし => しない、 1 => する。※別途sColosseumEventIdを指定してる必要があります

   }

   [Serializable]
   public class ColosseumGetGroupLeagueBoardAPIResponse : AppAPIResponseBase {
		public GroupLeagueMatchBoardInfo boardInfo = null; // リーグ表表示に使うデータ
		public ColosseumScoreBattleTurn scoreBattleTurn = null; // （共通）ターン情報。開催期間外か何らかのトラブルが有る場合、取得できない。getTurnを指定時にのみ返す

   }
      
   public partial class ColosseumGetGroupLeagueBoardAPIRequest : AppAPIRequestBase<ColosseumGetGroupLeagueBoardAPIPost, ColosseumGetGroupLeagueBoardAPIResponse> {
      public override string apiName{get{ return "colosseum/getGroupLeagueBoard"; } }
   }
}