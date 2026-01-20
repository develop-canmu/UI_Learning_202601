//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleReserveFormationMatchHeader {
		public long id = 0; // sBattleReserveFormationMatchingId
		public long mBattleReserveFormationId = 0; // マスタID
		public long gradeNumber = 0; // $gradeNumber 試合グレード番号（1以上。グレード指定がない場合も1）
		public long eventType = 0; // イベント種別。1の場合colosseum
		public long eventId = 0; // イベント・シーズンID。 eventTypeが1の時sColosseumEventId
		public long eventMasterId = 0; // イベントマスタID。eventTypeが1の時、mColosseumEventId
		public long matchingType = 0; // $matchingType 試合区分：1 => シーズン戦、2 => 入れ替え戦
		public string startAt = ""; // シーズン開始日付
		public string endAt = ""; // シーズン終了日付
		public string formationLockAt = ""; // デッキロック時刻
		public long status = 0; // 処理状態 1 => 未処理、 99 => 処理済み
		public long result = 0; // 試合結果（1 win 2 lose 3 draw  99未処理）

   }
   
}