//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class UserLoggedIn {
		public long uMasterId = 0; // ユーザID
		public string friendCode = ""; // フレンドコード
		public long userType = 0; // ユーザ種別（0: 一般, 1: 社内, 2: 社内ヘビー）
		public string name = ""; // ユーザ名
		public long gender = 0; // 性別（1: 男性, 2: 女性）
		public long gMasterId = 0; // 所属しているギルドのID（ギルド未所属の場合は 0 が入る）
		public long mIconId = 0; // 設定しているアイコンID
		public string maxCombatPower = ""; // これまでに組んだ戦闘デッキの最大戦力
		public long maxDeckRank = 0; // これまでに組んだ戦闘デッキの最大ランク
		public long[] finishedTutorialNumberList = null; // 終了したチュートリアルの番号の配列
		public bool isTermsAgreed = false; // 規約同意済みであるか
		public bool hasRegisteredBirthday = false; // 生年月日が登録済みなら真
		public bool hasParentalConsent = false; // 保護者の同意を得ているなら真
		public long monthPayment = 0; // 今月の課金額
		public long monthPaymentLimit = 0; // 月の課金額上限（生年月日が未登録の場合および上限がない場合は PHP_INT_MAX = 9223372036854775807）
		public bool isPaid = false; // 課金したことがあるか
		public long paymentPenaltyLevel = 0; // 課金罰則レベル（0: 無し, 998: 警告, 999: BAN）
		public bool allowsGuildInvitation = false; // ギルド勧誘を受け付ける
		public long guildInvitationGuildRank = 0; // ギルド勧誘：希望ギルドランク
		public long guildInvitationPlayStyleType = 0; // ギルド勧誘：プレイスタイル
		public long guildInvitationGuildBattleType = 0; // ギルド勧誘：ギルドバトルのプレイスタイル
		public string guildInvitationMessage = ""; // ギルド勧誘：アピールコメント
		public long guildParticipationPriorityType = 0; // ギルド参加優先度種別

   }
   
}