//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingActiveTrainingStatusType {
		public long cardIndex = 0; // 何番目のカードで発生しているか。常時発動の練習能力は全てのcardIndexに対してこのオブジェクトを発行する。
		public long mCharaId = 0; // 練習能力の持ち主となるキャラのID。負値はそれぞれ、-1 => training_combo_buff, -2 => combination_training, -4 => trainer
		public long type = 0; // 練習能力タイプ。m_training_status_type_detail.type の値を返している

   }
   
}