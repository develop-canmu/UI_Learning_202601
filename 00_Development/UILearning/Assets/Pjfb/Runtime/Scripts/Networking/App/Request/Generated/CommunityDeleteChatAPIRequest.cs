//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ユーザーのチャット削除

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CommunityDeleteChatAPIPost : AppAPIPostBase {
		public long targetUMasterId = 0; // 選択したチャットの送信相手

   }

   [Serializable]
   public class CommunityDeleteChatAPIResponse : AppAPIResponseBase {

   }
      
   public partial class CommunityDeleteChatAPIRequest : AppAPIRequestBase<CommunityDeleteChatAPIPost, CommunityDeleteChatAPIResponse> {
      public override string apiName{get{ return "community/deleteChat"; } }
   }
}