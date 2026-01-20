//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ユーザーをブロックする

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CommunityBlockAPIPost : AppAPIPostBase {
		public long targetUMasterId = 0; // ブロックするユーザーID

   }

   [Serializable]
   public class CommunityBlockAPIResponse : AppAPIResponseBase {

   }
      
   public partial class CommunityBlockAPIRequest : AppAPIRequestBase<CommunityBlockAPIPost, CommunityBlockAPIResponse> {
      public override string apiName{get{ return "community/block"; } }
   }
}