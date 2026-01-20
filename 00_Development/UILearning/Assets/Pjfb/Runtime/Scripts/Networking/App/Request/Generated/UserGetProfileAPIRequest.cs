//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ユーザのプロフィールデータの取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class UserGetProfileAPIPost : AppAPIPostBase {
		public long targetUMasterId = 0; // プロフィールを取得するユーザーのID

   }

   [Serializable]
   public class UserGetProfileAPIResponse : AppAPIResponseBase {
		public UserProfileUserStatus user = null; // ユーザのプロフィール情報
		public long allowsGuildInvitation = 0; // ギルドからの勧誘許可設定

   }
      
   public partial class UserGetProfileAPIRequest : AppAPIRequestBase<UserGetProfileAPIPost, UserGetProfileAPIResponse> {
      public override string apiName{get{ return "user/getProfile"; } }
   }
}