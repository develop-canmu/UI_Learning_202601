//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class ColosseumGroupSeasonStatus {
		public long id = 0; // s_colosseum_group_status.id
		public long mColosseumEventId = 0; // イベントマスタID
		public long sColosseumEventId = 0; // シーズンID
		public long gradeNumber = 0; // $gradeNumber グレード番号（1以上。グレード管理が無い方式の場合も1）
		public long roomNumber = 0; // $roomNumber 部屋番号
		public long gradeBefore = 0; // シーズン終了前のグレード
		public long gradeAfter = 0; // $gradeAfter シーズン終了後のグレード
		public long pointBefore = 0; // $gradeAfter ギルドポイント（終了前）
		public long pointAfter = 0; // $gradeAfter ギルドポイント（終了後）
		public long ranking = 0; // 順位
		public long scoreRanking = 0; // 順位（シーズン中は-1を入れておく。バッチ実行時に確定。それ以外の参照時は、データ取得時にscore準拠で値を代入する）
		public long score = 0; // $score スコア
		public long groupType = 0; // $groupType 1 => ギルド, 2 => NPCグループ（現在は1のみ対応）
		public long groupId = 0; // $groupId groupTypeに紐づくid（ギルドIDなど）
		public string name = ""; // $name 名称
		public long mGuildEmblemId = 0; // $mGuildEmblemId ギルドエンブレムID
		public string combatPower = ""; // $combatPower 開始前段階での、戦力値
		public ColosseumShiftMatchInfo shiftMatchInfo = null; // 入れ替え戦の進捗・結果の紐づけ
		public ColosseumEntryChainInfo entryChainInfo = null; // エントリー引き継ぎの発生状況などに関して記録

   }
   
}