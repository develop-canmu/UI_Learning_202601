//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class ProbabilityFrame {
		public long id = 0; // m_common_prize_frame.id
		public string name = ""; // 枠名称
		public double percentage = 0.0; // この枠が締める合計排出率　
		public double choicePercentage = 0.0; // この枠に内包されるピックアップ確率
		public ProbabilityContent[] contentList = null; // 排出内容一覧

   }
   
}