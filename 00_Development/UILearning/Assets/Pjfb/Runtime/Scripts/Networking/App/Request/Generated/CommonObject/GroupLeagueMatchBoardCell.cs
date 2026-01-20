//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class GroupLeagueMatchBoardCell {
		public long groupIndex = 0; // 表内でのグループの識別番号。1始まり。1=>A,2=>B…的な感じでラベリングする
		public long result = 0; // 試合結果（1 win 2 lose 3 draw 98 存在しない 99未処理）
		public long inGameMatchId = 0;

   }
   
}