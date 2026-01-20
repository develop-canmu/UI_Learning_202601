//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class ColosseumUserSeasonStatus {
		public long id = 0; // u_colosseum_room_status.id
		public long uMasterId = 0; // $uMasterId ユーザーID
		public long sColosseumGroupStatusId = 0; // グループID（グループ戦じゃない場合は、0）
		public long mColosseumEventId = 0; // $mColosseumEventId イベントマスタID
		public long sColosseumEventId = 0; // $sColosseumEventId イベント管理ID
		public string startAt = ""; // 開始日時
		public string endAt = ""; // 終了日時
		public long gradeNumber = 0; // $gradeNumber グレード番号（1以上。グレード管理がない場合も1）
		public long roomNumber = 0; // $roomNumber 部屋番号（1以上。隔離部屋の場合は-1とか）
		public long gradeAfter = 0; // $gradeAfter シーズン終了後のグレード（集計前は0が入っている）
		public long ranking = 0; // $ranking ランキング
		public long winCount = 0; // $winCount 勝利数
		public long loseCount = 0; // $loseCount 負け数
		public long drawCount = 0; // $drawCount 引き分け数
		public long pointGet = 0; // $pointGet 総得点
		public long pointLost = 0; // $pointLost 総失点
		public long score = 0; // 獲得得点（スコア戦のみ有効）
		public long scoreRanking = 0; // 獲得得点（スコア戦のみ有効）
		public string rankChangedAt = ""; // 順位変動時刻（順位変動時刻。レコード生成時には「シーズンの開始時刻」に設定される）。グループ戦以外の場合はクライアントには送らない
		public long defenseCount = 0; // 該当順位についてからの防衛回数。グループ戦以外の場合はクライアントには送らな
		public ColosseumGroupSeasonStatus groupSeasonStatus = null; // グループのシーズン情報
		public long todayResultQuick = 0; // 当日のバトル結果情報

   }
   
}