//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
過去の戦績一覧を得る（現在は最大10件ずつ取得）
10件目以上を取得する場合は、取得済みリストの最後のsColosseumEventIdをlastSeasonIdとして指定する。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class ColosseumGetUserSeasonStatusPastListAPIPost : AppAPIPostBase {
		public long mColosseumEventId = 0; // イベントID
		public long lastSeasonId = 0; // 確認していた履歴の末尾ID（sColosseumEventId）。-1の場合は先頭から取得
		public long getTurn = 0; // （共通）ターンを取得するかどうか。0、なし => しない、 1 => する。※別途sColosseumEventIdを指定してる必要があります

   }

   [Serializable]
   public class ColosseumGetUserSeasonStatusPastListAPIResponse : AppAPIResponseBase {
		public ColosseumUserSeasonStatus[] userSeasonStatusList = null; // 過去のユーザーシーズン情報
		public ColosseumScoreBattleTurn scoreBattleTurn = null; // （共通）ターン情報。開催期間外か何らかのトラブルが有る場合、取得できない。getTurnを指定時にのみ返す

   }
      
   public partial class ColosseumGetUserSeasonStatusPastListAPIRequest : AppAPIRequestBase<ColosseumGetUserSeasonStatusPastListAPIPost, ColosseumGetUserSeasonStatusPastListAPIResponse> {
      public override string apiName{get{ return "colosseum/getUserSeasonStatusPastList"; } }
   }
}