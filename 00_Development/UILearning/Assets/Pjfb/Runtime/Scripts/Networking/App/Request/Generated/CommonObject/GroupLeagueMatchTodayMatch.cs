//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class GroupLeagueMatchTodayMatch {
		public long sColosseumGroupStatusId = 0; // 対応するs_colosseum_group_status.id
		public string name = ""; // グループ名
		public string date = ""; // 試合日（試合開始日次ではないので、それが必要な場合はクライアントで計算する）
		public long mGuildEmblemId = 0; // mGuildEmblemId
		public long groupType = 0; // groupType
		public long groupId = 0; // groupId
		public long matchingType = 0; // 試合区分：1 => シーズン戦、2 => 入れ替え戦
		public long inGameMatchId = 0; // インゲームシステム側の試合ID（ない場合0）
		public long result = 0; // 試合結果。1 => 勝利、 2 => 敗北、 3 => 引き分け、 99 => 未実施
		public long resultQuick = 0; // 試合結果速報。インゲーム側の試合データに登録されている試合結果を表示する。1 => 勝利、 2 => 敗北、 3 => 引き分け、 99 => 未実施

   }
   
}