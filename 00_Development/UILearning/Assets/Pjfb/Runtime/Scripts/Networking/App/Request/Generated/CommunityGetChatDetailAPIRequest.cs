//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
個人チャットの詳細を取得。lastUChatIdが指定されている場合は差分のみ返却

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CommunityGetChatDetailAPIPost : AppAPIPostBase {
		public long targetUMasterId = 0; // 選択したチャットの送信相手
		public long lastUChatId = 0; // 閲覧した最後のuChatListのid

   }

   [Serializable]
   public class CommunityGetChatDetailAPIResponse : AppAPIResponseBase {
		public ModelsUChat[] uChatList = null; // チャット情報リスト
		public UserChatUserStatus chatUserStatus = null; // 送信相手のユーザー情報

   }
      
   public partial class CommunityGetChatDetailAPIRequest : AppAPIRequestBase<CommunityGetChatDetailAPIPost, CommunityGetChatDetailAPIResponse> {
      public override string apiName{get{ return "community/getChatDetail"; } }
   }
}