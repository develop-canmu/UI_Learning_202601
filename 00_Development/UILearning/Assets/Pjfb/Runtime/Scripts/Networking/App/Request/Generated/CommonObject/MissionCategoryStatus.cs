//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class MissionCategoryStatus {
		public long mDailyMissionCategoryId = 0; // ミッションカテゴリ
		public long progress = 0; // ミッション進捗。受取済みのm_daily_mission.sortNumber までの
		public MissionUserAndGuild targetMission = null; // 対象ミッション（現在挑戦中orクリア済み）。すでに対応するミッションがない場合は、nullが格納されます

   }
   
}