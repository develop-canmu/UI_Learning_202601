//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
個人チャットの一覧を取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CommunityGetChatListAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class CommunityGetChatListAPIResponse : AppAPIResponseBase {
		public ModelsUChat[] uChatList = null; // チャット情報リスト
		public UserChatUserStatus[] chatUserStatusList = null; // ユーザー情報リスト

   }
      
   public partial class CommunityGetChatListAPIRequest : AppAPIRequestBase<CommunityGetChatListAPIPost, CommunityGetChatListAPIResponse> {
      public override string apiName{get{ return "community/getChatList"; } }
   }
}