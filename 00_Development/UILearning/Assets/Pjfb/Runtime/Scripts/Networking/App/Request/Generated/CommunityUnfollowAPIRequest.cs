//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ユーザーのフォローを解除する

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CommunityUnfollowAPIPost : AppAPIPostBase {
		public long targetUMasterId = 0; // フォローするユーザーID

   }

   [Serializable]
   public class CommunityUnfollowAPIResponse : AppAPIResponseBase {

   }
      
   public partial class CommunityUnfollowAPIRequest : AppAPIRequestBase<CommunityUnfollowAPIPost, CommunityUnfollowAPIResponse> {
      public override string apiName{get{ return "community/unfollow"; } }
   }
}