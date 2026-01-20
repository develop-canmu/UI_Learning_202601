//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルドチャットを取得。lastGChatIdが指定されている場合は差分のみ返却

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CommunityGetGuildChatAPIPost : AppAPIPostBase {
		public long lastGChatId = 0; // 閲覧した最後のgChatListのid
		public long lastLogId = 0; // 閲覧した最後のgActionLogListのid（現在は使わない）
		public string lastLogCreatedAt = ""; // 閲覧した最後のgActionLogListの作成日時

   }

   [Serializable]
   public class CommunityGetGuildChatAPIResponse : AppAPIResponseBase {
		public ModelsGChat[] gChatList = null; // チャット情報リスト
		public GuildLogGuildActionLog[] gActionLogList = null; // ギルドログ情報リスト
		public UserChatUserStatus[] chatUserStatusList = null; // ユーザー情報リスト

   }
      
   public partial class CommunityGetGuildChatAPIRequest : AppAPIRequestBase<CommunityGetGuildChatAPIPost, CommunityGetGuildChatAPIResponse> {
      public override string apiName{get{ return "community/getGuildChat"; } }
   }
}