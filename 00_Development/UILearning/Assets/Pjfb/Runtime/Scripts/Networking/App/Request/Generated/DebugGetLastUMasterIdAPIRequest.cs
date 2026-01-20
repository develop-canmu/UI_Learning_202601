//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
uMasterId の最大値を取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugGetLastUMasterIdAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class DebugGetLastUMasterIdAPIResponse : AppAPIResponseBase {
		public long lastUMasterId = 0; // u_master.id の最大値

   }
      
   public partial class DebugGetLastUMasterIdAPIRequest : AppAPIRequestBase<DebugGetLastUMasterIdAPIPost, DebugGetLastUMasterIdAPIResponse> {
      public override string apiName{get{ return "debug/getLastUMasterId"; } }
   }
}