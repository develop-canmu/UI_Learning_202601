//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
マスタダウンロード対象になっているテーブルとそのカラムを返却する

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugGetMasterConfigAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class DebugGetMasterConfigAPIResponse : AppAPIResponseBase {
		public object[] masters = null; // マスタデータ

   }
      
   public partial class DebugGetMasterConfigAPIRequest : AppAPIRequestBase<DebugGetMasterConfigAPIPost, DebugGetMasterConfigAPIResponse> {
      public override string apiName{get{ return "debug/getMasterConfig"; } }
   }
}