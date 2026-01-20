//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingCard {
		public long mTrainingCardId = 0; // カードID
		public long mCharaId = 0; // カードの持ち主のキャラID
		public long level = 0; // カードレベル
		public long cardGroupType = 0; // カード種別。0: 基本、1: スペシャルトレーニング
		public long practiceType = 0; // 練習種別。行動では毎回、各練習タイプのカードが1枚ずつ選択される。練習カードが5枚とする場合は0から4で指定する。
		public long imageId = 0; // 練習カードの画像ID
		public long mTrainingCardCharaId = 0; // mTrainingCardCharaのid

   }
   
}