//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギルド役職変更・役職解除

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GuildUpdateRoleAPIPost : AppAPIPostBase {
		public long beforeUMasterId = 0; // 解除する対象のユーザーID
		public long targetUMasterId = 0; // 設定する対象のユーザーID
		public long mGuildRoleId = 0; // 変更後の役職ID

   }

   [Serializable]
   public class GuildUpdateRoleAPIResponse : AppAPIResponseBase {

   }
      
   public partial class GuildUpdateRoleAPIRequest : AppAPIRequestBase<GuildUpdateRoleAPIPost, GuildUpdateRoleAPIResponse> {
      public override string apiName{get{ return "guild/updateRole"; } }
   }
}