//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ブロックを解除する

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CommunityRemoveBlockAPIPost : AppAPIPostBase {
		public long targetUMasterId = 0; // ブロック解除するユーザーID

   }

   [Serializable]
   public class CommunityRemoveBlockAPIResponse : AppAPIResponseBase {

   }
      
   public partial class CommunityRemoveBlockAPIRequest : AppAPIRequestBase<CommunityRemoveBlockAPIPost, CommunityRemoveBlockAPIResponse> {
      public override string apiName{get{ return "community/removeBlock"; } }
   }
}