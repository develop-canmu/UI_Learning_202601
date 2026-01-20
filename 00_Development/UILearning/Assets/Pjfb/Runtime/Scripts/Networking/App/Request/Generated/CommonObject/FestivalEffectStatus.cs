//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class FestivalEffectStatus {
		public bool isCausedNow = false; // 今発動したかどうか。トレーニング完了時抽選で発動した場合のみ true
		public long mFestivalId = 0; // $mFestivalId イベントID
		public long mFestivalTimetableId = 0; // $mFestivalId イベントタイムテーブルID
		public long causeType = 0; // $causeType 発生原因。-1 => トレーニング終了時抽選で発生。それ以外は mFestivalItemId を指す。
		public string expireAt = ""; // $expireAt 終了時刻
		public long value = 0; // $value 補正数値

   }
   
}