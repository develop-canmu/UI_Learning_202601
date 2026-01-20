//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ユーザデータの取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class UserGetDataAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class UserGetDataAPIResponse : AppAPIResponseBase {
		public UserLoggedIn user = null; // ユーザの基本情報
		public NativeApiConf conf = null; // ログイン後、基本的に使い続ける環境設定値類
		public long[] followUMasterIdList = null; // フォローしているユーザーのIDリスト
		public long[] guildUMasterIdList = null; // ギルドメンバーのユーザーのIDリスト
		public long[] todayYelledUMasterIdList = null; // 本日のエール済みユーザーIDリスト
		public HuntUserPending huntPending = null; // 狩猟のユーザー保留情報
		public DeckBase[] deckList = null; // デッキ一蘭
		public WrapperIntList[] useTypePartyNumberList = null; // [用途番号・partyNumber]が格納された配列を返却します。
		public FestivalEffectStatus[] festivalEffectStatusList = null; // 発生中のイベント特殊効果リスト
		public WrapperIntList[] pushSettingList = null; // プッシュ通知設定。[mPushId, オンオフ設定] を列挙した2次元配列
		public DataRegionRestrictionData regionRestrictionData = null; // 地域による機能制限の情報
		public AdRewardUserStatus[] uAdRewardList = null; // 広告報酬ユーザー情報リスト
		public PlayerEnhanceData[] playerEnhanceList = null; // 強化レベル情報一覧

   }
      
   public partial class UserGetDataAPIRequest : AppAPIRequestBase<UserGetDataAPIPost, UserGetDataAPIResponse> {
      public override string apiName{get{ return "user/getData"; } }
   }
}