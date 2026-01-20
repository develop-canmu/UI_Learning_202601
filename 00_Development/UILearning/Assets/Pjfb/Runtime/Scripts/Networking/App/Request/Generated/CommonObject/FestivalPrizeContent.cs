//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class FestivalPrizeContent {
		public long mFestivalPrizeContentId = 0; // 追加報酬内容ID
		public long samePrizeContentId = 0; // 同一とみなす追加報酬内容ID
		public bool isUnlimited = false; // 無制限かどうか
		public long maxObtainCount = 0; // 初期の最大獲得回数。無制限の場合は0が入るので注意
		public long restCount = 0; // 残り獲得可能回数。無制限の場合は0が入るので注意
		public long festivalPoint = 0; // 追加イベントポイント量
		public PrizeJsonWrap[] prizeJson = null; // 追加報酬内容
		public bool isFeatured = false; // 目玉報酬かどうか

   }
   
}