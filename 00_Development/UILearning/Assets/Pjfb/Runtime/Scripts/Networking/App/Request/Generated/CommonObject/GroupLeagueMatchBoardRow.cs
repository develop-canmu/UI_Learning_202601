//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class GroupLeagueMatchBoardRow {
		public long groupIndex = 0; // 表内でのグループの識別番号。1始まり。1=>A,2=>B…的な感じでラベリングする
		public long gradeNumber = 0; // ランク情報
		public long sColosseumGroupStatusId = 0; // sColosseumGroupStatusId
		public string name = ""; // グループ名
		public long mGuildEmblemId = 0; // mGuildEmblemId
		public long groupType = 0; // groupType
		public long groupId = 0; // groupId
		public GroupLeagueMatchBoardCell[] cellList = null; // 各マスの情報
		public long winCount = 0; // 勝利数
		public long score = 0; // 得点

   }
   
}