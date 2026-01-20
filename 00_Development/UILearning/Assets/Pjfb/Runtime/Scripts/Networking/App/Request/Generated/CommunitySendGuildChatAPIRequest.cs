//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルドチャットを送信

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CommunitySendGuildChatAPIPost : AppAPIPostBase {
		public long type = 0; // チャットの種類。1:メッセージ、2:スタンプ
		public string body = ""; // メッセージ内容
		public long lastGChatId = 0; // 閲覧した最後のgChatListのid
		public string lastLogCreatedAt = ""; // 閲覧した最後のgActionLogListの作成日時

   }

   [Serializable]
   public class CommunitySendGuildChatAPIResponse : AppAPIResponseBase {
		public ModelsGChat[] gChatList = null; // チャット情報リスト
		public GuildLogGuildActionLog[] gActionLogList = null; // ギルドログ情報リスト
		public UserChatUserStatus[] chatUserStatusList = null; // ユーザー情報リスト

   }
      
   public partial class CommunitySendGuildChatAPIRequest : AppAPIRequestBase<CommunitySendGuildChatAPIPost, CommunitySendGuildChatAPIResponse> {
      public override string apiName{get{ return "community/sendGuildChat"; } }
   }
}