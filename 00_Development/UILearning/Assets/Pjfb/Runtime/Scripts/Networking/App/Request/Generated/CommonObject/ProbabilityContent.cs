//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class ProbabilityContent {
		public long id = 0; // m_common_prize_content.id
		public double percentage = 0.0; // 排出確率の百分率表現
		public long picked = 0; // ピックアップされたかどうか
		public PrizeJsonWrap[] prizeList = null; // 報酬情報

   }
   
}