//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
対象ユーザーのプロフィールにいいねを送信

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class UserSendProfileLikeAPIPost : AppAPIPostBase {
		public long targetUMasterId = 0; // 対象ユーザーID

   }

   [Serializable]
   public class UserSendProfileLikeAPIResponse : AppAPIResponseBase {
		public long likeCount = 0; // 対象ユーザーのいいね数

   }
      
   public partial class UserSendProfileLikeAPIRequest : AppAPIRequestBase<UserSendProfileLikeAPIPost, UserSendProfileLikeAPIResponse> {
      public override string apiName{get{ return "user/sendProfileLike"; } }
   }
}