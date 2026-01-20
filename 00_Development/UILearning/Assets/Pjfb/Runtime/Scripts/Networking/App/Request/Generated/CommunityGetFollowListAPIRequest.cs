//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
フォローしているユーザーの一覧を取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CommunityGetFollowListAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class CommunityGetFollowListAPIResponse : AppAPIResponseBase {
		public UserCommunityUserStatus[] communityUserStatusList = null; // ユーザー情報リスト

   }
      
   public partial class CommunityGetFollowListAPIRequest : AppAPIRequestBase<CommunityGetFollowListAPIPost, CommunityGetFollowListAPIResponse> {
      public override string apiName{get{ return "community/getFollowList"; } }
   }
}