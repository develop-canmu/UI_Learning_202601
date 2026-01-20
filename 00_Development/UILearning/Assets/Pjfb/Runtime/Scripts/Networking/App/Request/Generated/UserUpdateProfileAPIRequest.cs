//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ユーザのプロフィール更新

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class UserUpdateProfileAPIPost : AppAPIPostBase {
		public long mIconId = 0; // アイコンID
		public string wordIntroduction = ""; // 自己紹介文
		public UserProfileCardData profileData = null; // プロフィールデータ

   }

   [Serializable]
   public class UserUpdateProfileAPIResponse : AppAPIResponseBase {
		public UserProfileUserStatus user = null; // ユーザのプロフィール情報

   }
      
   public partial class UserUpdateProfileAPIRequest : AppAPIRequestBase<UserUpdateProfileAPIPost, UserUpdateProfileAPIResponse> {
      public override string apiName{get{ return "user/updateProfile"; } }
   }
}