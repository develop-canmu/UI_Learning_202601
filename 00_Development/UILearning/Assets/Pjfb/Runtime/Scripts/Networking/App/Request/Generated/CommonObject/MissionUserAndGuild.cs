//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class MissionUserAndGuild {
		public long joinType = 0; // 参加タイプ（1 => 個人での参加、2 => ギルドでの参加）
		public long requireCount = 0; // m_daily_mission.requireCount
		public long mDailyMissionCategoryId = 0; // m_daily_mission.mDailyMissionCategoryId
		public string startAt = ""; // m_daily_mission.startAt
		public long gDailyMissionId = 0; // g_daily_mission.id
		public long guildPassingCount = 0; // m_daily_mission.requireCountに対する、進捗度合い
		public long guildProgressStatus = 0; // 進行状況（1 => 進行中、 2 => 完了・受け取り待ち、 99 => 不明）
		public bool canReceive = false; // 受け取り可能か
		public long id = 0; // uDailyMissionId
		public long mDailyMissionId = 0; // ミッションID
		public long passingCount = 0; // m_daily_mission.requireCountに対する、進捗度合い
		public long progressStatus = 0; // 進行状況（1 => 進行中、 2 => 完了・受け取り待ち、 3 => 受け取り済み、 99 => 不明）

   }
   
}