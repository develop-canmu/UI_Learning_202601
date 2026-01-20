//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingBoardEventStatusOccurrence {
		public long mTrainingBoardEventStatusId = 0; // 練習能力実体ID
		public long rate = 0; // 発生確率
		public long activationType = 0; // 有効化タイプ 1: 即時（未実装）、2: 次のtimingから、3: 次のturnから
		public long turnCount = 0; // 有効ターン数

   }
   
}