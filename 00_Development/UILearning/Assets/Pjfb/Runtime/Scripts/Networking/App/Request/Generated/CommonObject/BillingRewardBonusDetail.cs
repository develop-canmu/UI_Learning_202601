//
// This file is auto-generated
//

using System;

namespace Pjfb.Networking.App.Request {
   
   [Serializable]
   public partial class BillingRewardBonusDetail {
		public long mBillingRewardBonusId = 0; // 課金パックID。
		public long mBillingRewardId = 0; // 課金商品ID。
		public string name = ""; // 課金パック名。
		public string groupName = ""; // 課金パックのグループ名。バナー画像に文字を入れられない多言語版などで使用する。
		public string description = ""; // 説明文。
		public string appealText = ""; // 訴求テキスト。
		public long category = 0; // カテゴリ番号。
		public long subCategory = 0; // サブカテゴリ番号。
		public long imageNumber = 0; // バナー画像番号。バナー画像がないものは 0 になる。
		public long priority = 0; // 表示優先度。
		public long stepGroup = 0; // ステップアップのグループ番号。ステップアップパックでないなら 0 になる。
		public long stepNumber = 0; // ステップアップのステップ番号。最も低い番号が最初のステップになる。ステップアップパックでないなら 0 になる。
		public PrizeJsonWrap[] prizeJsonList = null; // 報酬情報の配列。
		public string releaseDatetime = ""; // 販売開始日時。
		public string closedDatetime = ""; // 販売終了日時。dayOfWeekList や availableSeconds も加味したうえでの日時が入る。
		public long buyLimit = 0; // 購入回数上限。0 以下の場合は上限なしとみなす。
		public long buyCount = 0; // 購入した回数。負荷軽減の都合で購入回数上限なしの場合は常に 0 になる。
		public string detailUrl = ""; // 詳細ページのURL。詳細ページがない場合は空文字列になる。
		public long mSaleIntroductionId = 0; // 課金連携商品の発生条件となっているシークレットセールID。シークレットセール条件がない場合は 0 になる。
		public bool saleIntroductionActiveFlg = false; // シークレットセールの販売が有効か。売り切れの場合はfalse

   }
   
}