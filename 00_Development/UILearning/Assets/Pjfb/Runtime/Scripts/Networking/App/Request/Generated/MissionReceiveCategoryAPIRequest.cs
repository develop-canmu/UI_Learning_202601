//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
カテゴリ単位受け取り操作

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class MissionReceiveCategoryAPIPost : AppAPIPostBase {
		public long mDailyMissionCategoryId = 0; // ミッションカテゴリID

   }

   [Serializable]
   public class MissionReceiveCategoryAPIResponse : AppAPIResponseBase {
		public MissionUserAndGuild[] missionList = null; // ミッションのユーザー進捗リスト
		public long unreceivedGiftLockedCount = 0; // 受け取り可能な時限式プレゼントの数
		public string newestGiftLockedAt = ""; // 直近受け取り可能な時限式プレゼントの時間
		public long unopenedGiftBoxCount = 0; // 未解放の時限式プレゼントの数

   }
      
   public partial class MissionReceiveCategoryAPIRequest : AppAPIRequestBase<MissionReceiveCategoryAPIPost, MissionReceiveCategoryAPIResponse> {
      public override string apiName{get{ return "mission/receiveCategory"; } }
   }
}