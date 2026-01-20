//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ホーム画面表示に必要な情報を取得する

デバッグユーザ（設定値 debugUMasterIdList に登録されているユーザ）の場合は newsBannerList, newsPopupList の内容がステージ環境相当のものになります。
お知らせバナーやお知らせポップアップの表示を本番環境反映前に確認したい場合に便利です。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class HomeGetDataAPIPost : AppAPIPostBase {
		public long ignoreUpdate = 0; // update処理を行わずデータだけ取得するときに用いるフラグ。1の場合有効。デフォルト0（無効）

   }

   [Serializable]
   public class HomeGetDataAPIResponse : AppAPIResponseBase {
		public long gMasterId = 0; // 所属しているギルドのID（ギルド未所属の場合は 0 が入る）
		public GuildBattleMatchingPeriod[] guildBattleMatchingPeriodList = null; // 本日の未終了のギルバトマッチングの開催時刻情報（当日のマッチングがまだ完了していない場合は空配列
		public LoginStampReceiveResult[] loginStampReceiveResultList = null; // ログインボーナス受取結果情報の配列
		public string newsArticleForcedDisplayTarget = ""; // お知らせ記事強制表示対象（強制表示対象のお知らせ記事がなければ空文字列）
		public string newsArticleForcedDisplayStartAt = ""; // お知らせ記事強制表示開始日時（この日時以降に強制表示対象のお知らせ記事を表示していなければ強制表示する）
		public string newsArticleForcedDisplayEndAt = ""; // お知らせ記事強制表示終了日時（この日時以降であればお知らせ記事の強制表示は行わない）
		public NewsBanner[] newsBannerList = null; // お知らせバナー情報の配列
		public NewsPopup[] newsPopupList = null; // お知らせポップアップ情報の配列
		public long unreceivedGiftCount = 0; // 未受取のプレゼントの数
		public long unreceivedGiftLockedCount = 0; // 受け取り可能な時限式プレゼントの数
		public string newestGiftLockedAt = ""; // 直近受け取り可能な時限式プレゼントの時間
		public long unopenedGiftBoxCount = 0; // 未解放の時限式プレゼントの数
		public long finishedMissionCount = 0; // クリア済かつ報酬未受取のミッションの数
		public long todayYelledCount = 0; // 当日の送信済みエール回数
		public long unViewedYellCount = 0; // エール未読数
		public long unViewedGuildChatCount = 0; // ギルドチャット未読数
		public long unViewedChatCount = 0; // チャット未読数
		public long unViewedGuildInfoCount = 0; // ギルドのお知らせ情報未読数
		public string newestGuildInvitationDate = ""; // 勧誘されているギルド情報の最新日時
		public string newestUserInvitationDate = ""; // 所属ギルドから勧誘しているユーザー情報の最新日時
		public string newestJoinRequestDate = ""; // 加入申請情報の最新日時
		public bool hasTrustPrize = false; // 未受取のキャラ信頼度報酬があれば真
		public bool hasFreeGacha = false; // 無料で引けるガチャがあれば真
		public bool hasPendingGacha = false; // 保留中のガチャがあれば真
		public long[] mBillingRewardBonusIdList = null; // 販売中の課金パックIDの配列（課金パックの未読判定に使う）
		public long freeBillingRewardBonusCount = 0; // 購入可能な無料課金パックの数
		public ColosseumHomeData colosseum = null; // colosseumの基本情報
		public MissionCategoryStatus beginnerMissionCategoryStatus = null; // 初心者ミッションの処理状況。表示不要な場合はnull
		public long[] unViewedSystemLockNumberList = null; // 未表示の機能解放番号リスト
		public TrainingAutoPendingStatus[] trainingAutoPendingStatusList = null; // 自動トレーニング状況リスト
		public bool isGuildFirstJoin = false; // ギルド初回加入が行われたかどうか

   }
      
   public partial class HomeGetDataAPIRequest : AppAPIRequestBase<HomeGetDataAPIPost, HomeGetDataAPIResponse> {
      public override string apiName{get{ return "home/getData"; } }
   }
}