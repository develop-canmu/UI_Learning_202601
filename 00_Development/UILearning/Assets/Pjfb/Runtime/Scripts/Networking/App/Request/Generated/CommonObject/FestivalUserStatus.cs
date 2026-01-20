//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class FestivalUserStatus {
		public long uMasterId = 0; // ユーザーID
		public long mFestivalTimetableId = 0; // イベントタイムテーブルID
		public long mFestivalId = 0; // イベントID
		public long pointValue = 0; // イベントポイント量
		public FestivalHuntTimetableStatus huntTimetableStatus = null; // 狩猟進捗
		public FestivalPrizeMissionStatus prizeMissionStatus = null; // 新イベントに付随するミッションのうちイベントポイント報酬にあたるほうの進捗
		public FestivalEffectStatus effectStatus = null; // 新イベントの特殊効果発生状況
		public FestivalPrizeStatus festivalPrizeStatus = null; // 獲得可能な残り追加報酬に関する情報
		public bool hasMissionNotification = false; // イベントミッションに通知用バッジがつくか

   }
   
}