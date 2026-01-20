//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
当日のエール情報取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CommunityGetTodayYellDetailAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class CommunityGetTodayYellDetailAPIResponse : AppAPIResponseBase {
		public ModelsUYell[] todayYelledList = null; // 当日に送信したエールリスト
		public long guildMemberCount = 0; // ギルドメンバー数
		public long guildCanYellCount = 0; // エール送信可能なギルドメンバー数
		public long followedCount = 0; // フォロー数
		public long followedCanYellCount = 0; // エール送信可能なフォロー中のユーザー数
		public long yellCount = 0; // 当日のエール送信済み回数

   }
      
   public partial class CommunityGetTodayYellDetailAPIRequest : AppAPIRequestBase<CommunityGetTodayYellDetailAPIPost, CommunityGetTodayYellDetailAPIResponse> {
      public override string apiName{get{ return "community/getTodayYellDetail"; } }
   }
}