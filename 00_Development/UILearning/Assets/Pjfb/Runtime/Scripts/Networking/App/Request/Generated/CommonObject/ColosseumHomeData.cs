//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class ColosseumHomeData {
		public ColosseumSeasonHome[] seasonList = null; // シーズン情報
		public ColosseumUserSeasonStatus[] unreadList = null; // 未読の終了済みシーズン情報
		public ColosseumUserSeasonStatus[] userSeasonStatusList = null; // 開催中シーズンのユーザーシーズン情報
		public ColosseumGroupGrade[] groupGradeList = null; // 開催中ギルドが参加しているグレード情報の返却
		public long[] entryMColosseumEventIdList = null; // エントリー済みのmColosseumEventのid

   }
   
}