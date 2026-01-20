//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class FestivalPrizeMissionStatus {
		public long mDailyMissionCategoryId = 0; // ミッションカテゴリID
		public long nextPointValue = 0; // 次の報酬を獲得するために必要なイベントポイントの量
		public PrizeJsonWrap[] nextPrizeList = null; // 次の報酬の内容
		public bool hasNotification = false; // 通知のバッジをつけるか

   }
   
}