//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class ColosseumRankingUser {
		public long id = 0; // ID
		public long ranking = 0; // 順位
		public string name = ""; // 名前
		public long mIconId = 0; // ユーザーアイコン
		public long userType = 0; // ユーザー種別（1 => 通常のユーザー、 2 => NPC）
		public long uMasterId = 0; // ユーザーID
		public string combatPower = ""; // 総戦力
		public CharaV2LeaderIcon leaderIconChara = null; // リーダーアイコン表示用キャラ
		public long groupType = 0; // $groupType 1 => ギルド, 2 => NPCグループ（現在は1のみ対応）（チーム戦以外では活用しない）
		public long groupId = 0; // $groupId groupTypeに紐づくid（ギルドIDなど）（チーム戦以外では活用しない）
		public string groupName = ""; // グループ名（チーム戦以外では取得しない）
		public long mGuildEmblemId = 0; // ギルドエンブレムID（チーム戦以外では取得しない）
		public long score = 0; // スコア（スコア方式のバトル以外では使用しない）
		public long scoreRanking = 0; // スコアランキング順位（スコア方式のバトル以外では使用しない。最終集計後以外は、-1が入る。）
		public string rankChangedAt = ""; // 順位変動時刻（順位変動時刻。レコード生成時には「シーズンの開始時刻」に設定される）
		public long defenseCount = 0; // 該当順位についてからの防衛回数

   }
   
}