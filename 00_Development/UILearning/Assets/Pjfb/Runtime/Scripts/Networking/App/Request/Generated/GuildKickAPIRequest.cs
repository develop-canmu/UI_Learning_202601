//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルド追放

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GuildKickAPIPost : AppAPIPostBase {
		public long targetUMasterId = 0; // 対象のユーザーID。追放の場合に設定。脱退の場合は0

   }

   [Serializable]
   public class GuildKickAPIResponse : AppAPIResponseBase {

   }
      
   public partial class GuildKickAPIRequest : AppAPIRequestBase<GuildKickAPIPost, GuildKickAPIResponse> {
      public override string apiName{get{ return "guild/kick"; } }
   }
}