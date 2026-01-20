//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ミッション・進捗確認

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class MissionGetListAPIPost : AppAPIPostBase {
		public long[] mDailyMissionCategoryIdList = null; // ミッションカテゴリのIDリスト。指定しない場合は全て取得

   }

   [Serializable]
   public class MissionGetListAPIResponse : AppAPIResponseBase {
		public MissionUserAndGuild[] missionList = null; // ミッションのユーザー進捗リスト

   }
      
   public partial class MissionGetListAPIRequest : AppAPIRequestBase<MissionGetListAPIPost, MissionGetListAPIResponse> {
      public override string apiName{get{ return "mission/getList"; } }
   }
}