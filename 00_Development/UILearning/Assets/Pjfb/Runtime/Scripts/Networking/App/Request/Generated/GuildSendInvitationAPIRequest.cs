//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルド勧誘送信

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GuildSendInvitationAPIPost : AppAPIPostBase {
		public string targetUMasterIdListStr = ""; // 勧誘を送るユーザーIDリストの文字列。カンマ区切りで設定
		public string message = ""; // 招待メッセージ

   }

   [Serializable]
   public class GuildSendInvitationAPIResponse : AppAPIResponseBase {
		public long successCount = 0; // 勧誘を送信した数

   }
      
   public partial class GuildSendInvitationAPIRequest : AppAPIRequestBase<GuildSendInvitationAPIPost, GuildSendInvitationAPIResponse> {
      public override string apiName{get{ return "guild/sendInvitation"; } }
   }
}