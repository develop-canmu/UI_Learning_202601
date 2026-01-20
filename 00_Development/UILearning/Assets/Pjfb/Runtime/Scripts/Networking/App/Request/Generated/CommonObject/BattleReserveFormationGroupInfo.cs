//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleReserveFormationGroupInfo {
		public long gradeNumber = 0; // ランク情報
		public long id = 0; // 闘技場グループのID
		public long groupType = 0; //  1 => ギルド, 2 => NPCグループ（現在は1のみ対応）
		public long groupId = 0; // groupTypeに紐づくID(gMasterIdなど)
		public long ranking = 0; // 順位
		public long winCount = 0; // 勝ち数
		public long loseCount = 0; // 負け数
		public long drawCount = 0; // 分け数
		public long winningPoint = 0; // 勝ち点
		public long mGuildEmblemId = 0; // エンブレムID
		public string name = ""; // name

   }
   
}