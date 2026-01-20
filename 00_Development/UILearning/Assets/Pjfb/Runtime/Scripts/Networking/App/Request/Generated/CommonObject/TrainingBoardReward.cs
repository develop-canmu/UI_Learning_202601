//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class TrainingBoardReward {
		public long id = 0; // id
		public long effectType = 0; // 効果タイプ。0 => なし、1 => 臨時練習能力等の獲得、2 => イベント発生、3 => 特定練習カードの発生
		public long effectValue = 0; // 効果値または効果に関する補助定数。効果タイプごとに意味合いが異なる。effectType:1 の場合、効果が継続するターン数。effectType:2 の場合、発生イベントを予約するtiming
		public long mTrainingBoardEventContentGroupId = 0; // 臨時練習能力グループID
		public long mTrainingEventId = 0; // 発生イベントID
		public long mTrainingCardId = 0; // 発生練習カードID
		public long mTrainingEventRewardId = 0; // イベント報酬ID
		public TrainingCandidateBoardEventStatus[] candidateStatusList = null; // 候補となった臨時練習能力実体詳細リスト
		public TrainingBoardSourceUnit eventSourceUnitInfo = null; // イベント発生の由来となったユニットの情報
		public TrainingBoardSourceUnit statusSourceUnitInfo = null; // 練習能力効果発生の由来となったユニットの情報
		public bool isOverwritten = false; // 同種の臨時練習能力の獲得により発生中の効果が延長されたかどうか
		public long boardEventId = 0; // マスイベントID
		public long mTrainingBoardEventStatusId = 0; // 臨時練習能力実体ID

   }
   
}