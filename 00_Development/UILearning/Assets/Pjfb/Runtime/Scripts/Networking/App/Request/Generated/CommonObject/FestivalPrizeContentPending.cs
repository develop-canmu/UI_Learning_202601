//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class FestivalPrizeContentPending {
		public long mFestivalPrizeContentId = 0; // 追加報酬内容ID
		public bool isFeatured = false; // 目玉報酬かどうか
		public PrizeJsonWrap[] prizeJson = null; // 報酬内容
		public long value = 0; // 獲得個数（mFestivalPrizeContent 単位）

   }
   
}