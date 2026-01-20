//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ユーザー検索

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CommunitySearchUserAPIPost : AppAPIPostBase {
		public string name = ""; // 名前
		public string friendCode = ""; // フレンドコード
		public string combatPowerFrom = ""; // 過去最大戦力の開始値
		public string combatPowerTo = ""; // 過去最大戦力の終了値
		public long deckRankFrom = 0; // 過去最大デッキランクの開始値。指定なしの場合は-1
		public long deckRankTo = 0; // 過去最大デッキランクの終了値。指定なしの場合は-1

   }

   [Serializable]
   public class CommunitySearchUserAPIResponse : AppAPIResponseBase {
		public UserCommunityUserStatus[] communityUserStatusList = null; // ユーザー情報リスト

   }
      
   public partial class CommunitySearchUserAPIRequest : AppAPIRequestBase<CommunitySearchUserAPIPost, CommunitySearchUserAPIResponse> {
      public override string apiName{get{ return "community/searchUser"; } }
   }
}