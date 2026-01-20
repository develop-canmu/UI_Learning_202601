//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
所属ギルドのグループ戦の過去戦績を得る
clientHandlingTypeを指定した場合はそれに対応する戦績のみ取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class ColosseumGetGuildSeasonStatusPastListAPIPost : AppAPIPostBase {
		public long gMasterId = 0; // ギルドID。0の場合は、自身のギルドを参照
		public long[] clientHandlingTypeList = null; // 取得したいm_colosseum_eventのclientHandlingTypeを配列指定。指定しない場合は全種別取得
		public long getTurn = 0; // （共通）ターンを取得するかどうか。0、なし => しない、 1 => する。※別途sColosseumEventIdを指定してる必要があります

   }

   [Serializable]
   public class ColosseumGetGuildSeasonStatusPastListAPIResponse : AppAPIResponseBase {
		public ColosseumGroupSeasonHistory[] groupSeasonHistoryList = null; // ユーザー過去戦績と紐づく、グループの過去戦績を得る
		public ColosseumScoreBattleTurn scoreBattleTurn = null; // （共通）ターン情報。開催期間外か何らかのトラブルが有る場合、取得できない。getTurnを指定時にのみ返す

   }
      
   public partial class ColosseumGetGuildSeasonStatusPastListAPIRequest : AppAPIRequestBase<ColosseumGetGuildSeasonStatusPastListAPIPost, ColosseumGetGuildSeasonStatusPastListAPIResponse> {
      public override string apiName{get{ return "colosseum/getGuildSeasonStatusPastList"; } }
   }
}