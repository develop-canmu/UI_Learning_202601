//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
個人チャットを送信

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CommunitySendChatAPIPost : AppAPIPostBase {
		public long type = 0; // チャットの種類。1:メッセージ、2:スタンプ
		public long targetUMasterId = 0; // 選択したチャットの送信相手
		public string body = ""; // メッセージ内容
		public long lastUChatId = 0; // 閲覧した最後のuChatListのid

   }

   [Serializable]
   public class CommunitySendChatAPIResponse : AppAPIResponseBase {
		public ModelsUChat[] uChatList = null; // チャット情報リスト
		public UserChatUserStatus chatUserStatus = null; // 送信相手のユーザー情報

   }
      
   public partial class CommunitySendChatAPIRequest : AppAPIRequestBase<CommunitySendChatAPIPost, CommunitySendChatAPIResponse> {
      public override string apiName{get{ return "community/sendChat"; } }
   }
}