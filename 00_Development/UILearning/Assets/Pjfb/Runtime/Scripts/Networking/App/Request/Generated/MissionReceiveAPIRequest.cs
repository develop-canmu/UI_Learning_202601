//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
受け取り操作

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class MissionReceiveAPIPost : AppAPIPostBase {
		public long uDailyMissionId = 0; // ユーザーミッションデータのID
		public long mDailyMissionId = 0; // ミッションID。uDailyMissionId が存在しない場合に、マスタIDベースで受け取り操作を行いたい場合に使用する

   }

   [Serializable]
   public class MissionReceiveAPIResponse : AppAPIResponseBase {
		public MissionUserAndGuild[] missionList = null; // ミッションのユーザー進捗リスト
		public long unreceivedGiftLockedCount = 0; // 受け取り可能な時限式プレゼントの数
		public string newestGiftLockedAt = ""; // 直近受け取り可能な時限式プレゼントの時間
		public long unopenedGiftBoxCount = 0; // 未解放の時限式プレゼントの数
		public MissionCategoryStatus missionCategoryStatus = null; // ミッションカテゴリ（主に初心者ミッション）の処理状況。表示不要な場合はnull

   }
      
   public partial class MissionReceiveAPIRequest : AppAPIRequestBase<MissionReceiveAPIPost, MissionReceiveAPIResponse> {
      public override string apiName{get{ return "mission/receive"; } }
   }
}