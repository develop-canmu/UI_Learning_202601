//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class ColosseumHistory {
		public long id = 0; // id
		public long sColosseumEventId = 0; // イベント・シーズン管理ID
		public long opponentUserType = 0; // $opponentUserType 対戦相手ユーザー種別（1 => 通常ユーザー, 2 => NPC）
		public long opponentUserId = 0; // 対戦相手のID（typeによって、参照先変更）
		public long opponentSColosseumGroupStatusId = 0; // クライアント側には渡さない
		public ColosseumGroupMinimum groupInfo = null; // 所属グループ情報。所属グループがない場合は無い
		public string name = ""; // 対戦相手名
		public long mIconId = 0; // 対戦相手のアイコン
		public long directionType = 0; // 攻撃の方向種別。1 =>攻撃した、 2 => 攻撃された
		public long result = 0; // 試合の結果（1=> 勝利, 2 => 敗北, 3 => 引き分け）
		public long rankBefore = 0; // 対戦前の順位
		public long rankAfter = 0; // 対戦後に変動した順位
		public long sColosseumBattleLogId = 0; // バトルログID
		public CharaV2LeaderIcon leaderIconChara = null; // リーダーアイコン表示用キャラ
		public string createdAt = ""; // 生成日時

   }
   
}