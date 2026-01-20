//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
日付変更後のユーザデータの取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class UserGetDateChangeDataAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class UserGetDateChangeDataAPIResponse : AppAPIResponseBase {
		public long[] followUMasterIdList = null; // フォローしているユーザーのIDリスト
		public long[] todayYelledUMasterIdList = null; // 本日のエール済みユーザーIDリスト

   }
      
   public partial class UserGetDateChangeDataAPIRequest : AppAPIRequestBase<UserGetDateChangeDataAPIPost, UserGetDateChangeDataAPIResponse> {
      public override string apiName{get{ return "user/getDateChangeData"; } }
   }
}