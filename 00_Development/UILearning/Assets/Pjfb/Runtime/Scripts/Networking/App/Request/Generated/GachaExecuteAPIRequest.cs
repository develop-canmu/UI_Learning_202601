//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ガチャ実行

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GachaExecuteAPIPost : AppAPIPostBase {
		public long mGachaCategoryId = 0; // ガチャカテゴリID
		public long mGachaSettingId = 0; // ガチャ設定ID
		public long gachaCount = 0; // 実施回数（チケットガチャ等以外の場合は1を指定）
		public long bundleCount = 0; // まとめ引き回数（行わない場合は、1）
		public long mGachaCategoryDiscountId = 0; // 適用割引ID
		public long mGachaCategorySubPointId = 0; // 代替通貨指定ID
		public long[] mPointIdAlternativeList = null; // 代替ポイントのidリスト
		public AdRewardAdRewardInfo adRewardInfo = null; // 広告報酬情報

   }

   [Serializable]
   public class GachaExecuteAPIResponse : AppAPIResponseBase {
		public GachaPendingInfo pendingInfo = null; // ガチャ保留情報
		public WrapperPrizeList[] contentListList = null; // （保留発生の場合は、無し）ガチャ排出品を意味する2次元配列
		public long enabled = 0; // （保留発生の場合は、無し）ガチャが活性状態かどうか。0 => 非活性、1 => 活性
		public bool canShowReview = false; // （保留発生の場合は、無し）レビューの強制表示を行うか。0 => しない、1 => する
		public long executeLimitPersonal = 0; // ユーザーの実行回数上限。0以下の値の場合、制限無しとみなす
		public long executeCount = 0; // ユーザーの実行回数。executeLimitPersonalが入っていない場合は、基本あえて取得しない
		public RushRushCategoryInfo[] rushCategoryInfoList = null; // ガチャ実施による確変情報の受け渡し
		public long[] productionNumberList = null; // ガチャの演出として表現を行うための、演出番号指定
		public AdRewardUserStatus uAdReward = null; // 広告報酬ユーザー情報

   }
      
   public partial class GachaExecuteAPIRequest : AppAPIRequestBase<GachaExecuteAPIPost, GachaExecuteAPIResponse> {
      public override string apiName{get{ return "gacha/execute"; } }
   }
}