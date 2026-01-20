//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ポイント履歴、有効期限情報返却

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class PointGetPointDetailAPIPost : AppAPIPostBase {
		public long mPointId = 0; // ポイントID

   }

   [Serializable]
   public class PointGetPointDetailAPIResponse : AppAPIResponseBase {
		public NativeApiPointHistory[] pointHistoryList = null; // 有効期限ポイントの利用履歴リスト
		public NativeApiPointExpiry[] pointExpiryList = null; // 有効期限リスト

   }
      
   public partial class PointGetPointDetailAPIRequest : AppAPIRequestBase<PointGetPointDetailAPIPost, PointGetPointDetailAPIResponse> {
      public override string apiName{get{ return "point/getPointDetail"; } }
   }
}