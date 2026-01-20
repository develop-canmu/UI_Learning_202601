//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BattleReserveFormationHistory {
		public long id = 0; // uBattleReserveFormationHistoryId
		public long opponentUserType = 0; // $opponentUserType 対戦相手ユーザー種別（1 => 通常ユーザー, 2 => NPC）
		public long opponentUserId = 0; // 対戦相手のID（typeによって、参照先変更）
		public long opponentGroupId = 0; // クライアント側には渡さない
		public ColosseumGroupMinimum groupInfo = null; // 所属グループ情報。所属グループがない場合は無い
		public string name = ""; // 対戦相手名
		public long mIconId = 0; // 対戦相手のアイコン
		public long result = 0; // 試合の結果（1=> 勝利, 2 => 敗北, 3 => 引き分け）
		public long sBattleReserveFormationLogId = 0; // バトルログID
		public CharaV2LeaderIcon leaderIconChara = null; // リーダーアイコン表示用キャラ
		public string openAt = ""; // 公開日付
		public BattleV2ResultDetail resultDetail = null; // 試合結果詳細。得失点などの情報を格納した構造体
		public BattleReserveFormationHistoryOptionLabel optionLabel = null; // 履歴のラベル表示に使うための情報
		public long roundNumber = 0; // ラウンド番号
		public long winningPoint = 0; // 勝ち点
		public long sBattleReserveFormationGroupId = 0; // イベント・シーズン管理ID（クライアントには渡さない）
		public long resultDetailBase = 0; // 試合結果詳細。pointGet, pointLost（クライアントには渡さない）
		public long optionJson = 0; // 一部区分に存在しうるオプション的なデータの情報（クライアントには渡さない）

   }
   
}