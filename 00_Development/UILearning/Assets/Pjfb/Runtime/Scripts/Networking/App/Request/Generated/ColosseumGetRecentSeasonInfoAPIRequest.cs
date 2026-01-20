//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
特定のmColosseumEventIdにおける「開始済みの最新のシーズン」を返却する。
それと同時に、ユーザーの参加状況データも返却する。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class ColosseumGetRecentSeasonInfoAPIPost : AppAPIPostBase {
		public long mColosseumEventId = 0; // イベントID
		public long getTurn = 0; // （共通）ターンを取得するかどうか。0、なし => しない、 1 => する。※別途sColosseumEventIdを指定してる必要があります

   }

   [Serializable]
   public class ColosseumGetRecentSeasonInfoAPIResponse : AppAPIResponseBase {
		public ColosseumSeasonHome season = null; // シーズン情報。ない場合はnull
		public ColosseumUserSeasonStatus userSeasonStatus = null; // ユーザーのシーズン参加情報。シーズン情報があっても、ユーザーが未参加の場合はnullがかえる
		public ColosseumScoreBattleTurn scoreBattleTurn = null; // （共通）ターン情報。開催期間外か何らかのトラブルが有る場合、取得できない。getTurnを指定時にのみ返す

   }
      
   public partial class ColosseumGetRecentSeasonInfoAPIRequest : AppAPIRequestBase<ColosseumGetRecentSeasonInfoAPIPost, ColosseumGetRecentSeasonInfoAPIResponse> {
      public override string apiName{get{ return "colosseum/getRecentSeasonInfo"; } }
   }
}