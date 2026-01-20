//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingSupport {
		public long id = 0; // uCharaのid
		public long mCharaId = 0; // mCharaId
		public long level = 0; // レベル
		public long newLiberationLevel = 0; // 潜在解放レベル
		public long supportType = 0; // サポートタイプ。0 => 育成キャラ自身、1 => 通常、2 => フレンド、3 => スペシャルサポート、4 => 追加サポートキャラ、5 => トレーニング補助キャラ
		public long trainerId = 0; // トレーニング補助キャラのID
		public long[] statusIdList = null; // トレーニング補助キャラのサブ能力IDリスト
		public long cardType = 0; // m_charaのカード種別
		public long[] mTrainingCardCharaIdList = null; // mCharaIdにひもづくm_training_card_charaのidリスト

   }
   
}