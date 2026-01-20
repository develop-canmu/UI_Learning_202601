//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingActiveTrainingBoardEventStatus {
		public long mTrainingBoardEventStatusId = 0; // 臨時練習能力実体ID
		public long mTrainingBoardEventStatusName = 0; // 臨時練習能力実体表示名
		public long sourceType = 0; // 発生原因タイプ。 0 => 不明、1 => マスイベント、2 => 練習カード、3 => イベント
		public long mTrainingBoardEventId = 0; // 由来マスイベントID
		public long boardEventSourceType = 0; // マスイベントで発生した場合、マスイベントの発生原因（m_training_board_event.sourceType）
		public long boardEventSourceMCharaId = 0; // マスイベントで発生した場合かつ boardEventSourceType がサポートマスイベントであった場合、サポートマスイベントの発生要因となったキャラのID
		public long imageId = 0; // マスイベント画像ID
		public long rarity = 0; // マスイベントレア度
		public long mTrainingCardId = 0; // 由来カードID
		public long cardOwnerMCharaId = 0; // カード持ち主ID
		public long mTrainingEventId = 0; // 由来イベントID
		public long restTurnCount = 0; // 残り有効ターン数
		public long delayTurnCount = 0; // 発動まで残りターン数。1以上ならまだ発動していない
		public long lastAddedTurnCount = 0; // 最後に加算された有効ターン数。初回発動であれば初回のターン数

   }
   
}