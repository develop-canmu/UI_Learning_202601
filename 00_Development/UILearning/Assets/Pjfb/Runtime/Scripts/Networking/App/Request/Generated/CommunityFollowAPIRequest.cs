//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ユーザーをフォローする

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CommunityFollowAPIPost : AppAPIPostBase {
		public long targetUMasterId = 0; // フォローするユーザーID

   }

   [Serializable]
   public class CommunityFollowAPIResponse : AppAPIResponseBase {

   }
      
   public partial class CommunityFollowAPIRequest : AppAPIRequestBase<CommunityFollowAPIPost, CommunityFollowAPIResponse> {
      public override string apiName{get{ return "community/follow"; } }
   }
}