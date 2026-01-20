//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルド加入申請を拒否する

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GuildDisagreeJoinRequestAPIPost : AppAPIPostBase {
		public long targetUMasterId = 0; // 対象のユーザーID

   }

   [Serializable]
   public class GuildDisagreeJoinRequestAPIResponse : AppAPIResponseBase {

   }
      
   public partial class GuildDisagreeJoinRequestAPIRequest : AppAPIRequestBase<GuildDisagreeJoinRequestAPIPost, GuildDisagreeJoinRequestAPIResponse> {
      public override string apiName{get{ return "guild/disagreeJoinRequest"; } }
   }
}