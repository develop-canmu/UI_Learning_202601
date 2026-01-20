//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class CharaParentBase {
		public long parentMCharaId = 0; // 親キャラID
		public long trustExp = 0; // 信頼経験値
		public long trustLevel = 0; // 信頼レベル
		public long trustLevelRead = 0; // 信頼レベル既読（信頼レベル上昇に対して、既読（報酬受取）をどこまでおこなったか）
		public bool hasTrustPrize = false; // 未読の信頼報酬が存在するかどうか

   }
   
}