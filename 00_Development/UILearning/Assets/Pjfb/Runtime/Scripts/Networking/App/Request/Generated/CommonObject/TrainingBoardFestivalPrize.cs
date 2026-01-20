//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingBoardFestivalPrize {
		public long mFestivalId = 0; // イベントID
		public long mFestivalPrizeContentId = 0; // 追加報酬内容ID
		public long festivalPoint = 0; // 獲得できるイベントポイント
		public bool isFeatured = false; // 目玉報酬かどうか
		public PrizeJsonWrap[] prizeJson = null; // 報酬内容
		public long value = 0; // 獲得個数（mFestivalPrizeContent 単位）

   }
   
}