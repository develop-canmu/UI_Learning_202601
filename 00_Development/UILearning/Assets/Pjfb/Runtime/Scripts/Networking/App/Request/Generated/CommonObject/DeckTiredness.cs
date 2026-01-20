//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class DeckTiredness {
		public long value = 0; // 疲労値
		public string changeAt = ""; // 最終更新時刻（この日付を起点に、自然回復を計算）
		public string penaltyExpireAt = ""; // ペナルティ有効期限（この時刻がすぎるまで、特定の行動を制限）
		public string expireAt = ""; // 有効期限（この時刻をすぎると、一旦疲労度レコードがない状態と同等に扱う）
		public long conditionBase = 0; // 疲労度とは別に変動する、デッキの健康状態設定乱数シードのベース値
		public long conditionTableType = 0; // デッキの健康状態設定時に用いる、抽選テーブルの変更設定
		public long valueCalc = 0; // 時間経過なども含め計算した疲労度。デバッグ用の値。予告せずに消えます

   }
   
}