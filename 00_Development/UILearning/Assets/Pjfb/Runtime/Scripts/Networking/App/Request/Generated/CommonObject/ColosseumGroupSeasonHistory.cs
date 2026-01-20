//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class ColosseumGroupSeasonHistory {
		public long id = 0; // s_colosseum_group_status.id
		public long mColosseumEventId = 0; // $mColosseumEventId イベントマスタID
		public long cycleCount = 0; // $cycleCount サイクル数（該当イベントにおける、何シーズン目か）
		public string startAt = ""; // 開始日時
		public string endAt = ""; // 終了日時
		public long scoreRanking = 0; // $ranking 順位（シーズン中は-1を入れておく。バッチ実行時に確定。それ以外の参照時は、データ取得時にscore準拠で値を代入する）
		public long score = 0; // $score スコア
		public long gradeBefore = 0; // 変動前ランク
		public long gradeAfter = 0; // 変動後ランク
		public long shiftMatchProgress = 0; // 入れ替え戦進捗：-1 => 未マッチング、 0 => マッチング済み, 1 => 勝利、 2 => 敗北, 3 => 引き分け
		public long entryChainProgress = 0; // エントリー引き継ぎ状況：0 => エントリー引き継ぎ無し, 1 => エントリー引き継ぎ発生

   }
   
}