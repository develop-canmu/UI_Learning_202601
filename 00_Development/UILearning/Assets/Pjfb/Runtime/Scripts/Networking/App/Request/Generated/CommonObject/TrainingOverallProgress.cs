//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingOverallProgress {
		public long currentLevel = 0; // 現在の全体レベル
		public long currentValue = 0; // レベルを決める要因となるパラメータの現在の値
		public long previousLevelValue = 0; // このレベルになるために必要だった値
		public long nextLevelValue = 0; // 次のレベルになるために必要な値。残りの値ではなく必要な値そのもの
		public long beforeLevel = 0; // 進行前の全体レベル

   }
   
}