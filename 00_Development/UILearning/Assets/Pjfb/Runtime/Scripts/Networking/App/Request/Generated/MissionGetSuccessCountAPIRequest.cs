//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ミッションをクリアしている件数を取得します。
　（最終、ホームAPI等にまとめられる想定）

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class MissionGetSuccessCountAPIPost : AppAPIPostBase {
		public long[] mDailyMissionCategoryIdList = null; // ミッションカテゴリのIDリスト。指定しない場合は全て取得

   }

   [Serializable]
   public class MissionGetSuccessCountAPIResponse : AppAPIResponseBase {
		public long missionSuccessCount = 0; // ミッションを達成済みの個数

   }
      
   public partial class MissionGetSuccessCountAPIRequest : AppAPIRequestBase<MissionGetSuccessCountAPIPost, MissionGetSuccessCountAPIResponse> {
      public override string apiName{get{ return "mission/getSuccessCount"; } }
   }
}