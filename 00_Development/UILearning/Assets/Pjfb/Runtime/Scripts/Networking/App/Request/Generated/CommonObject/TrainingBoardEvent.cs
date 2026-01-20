//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingBoardEvent {
		public long id = 0; // m_training_board_event.id
		public long imageId = 0; // 画像ID
		public long rarity = 0; // レア度
		public long effectType = 0; // 硬化タイプ
		public long sourceType = 0; // m_training_board_event.sourceType と同一の値。1 => 通常イベント、2 => サポートマスイベント（m_chara.cardType:12 のキャラ（不可変サポートキャラ）が持つもの）
		public long sourceMCharaId = 0; // マスイベントの持ち主となるキャラのID。存在しない場合は0

   }
   
}