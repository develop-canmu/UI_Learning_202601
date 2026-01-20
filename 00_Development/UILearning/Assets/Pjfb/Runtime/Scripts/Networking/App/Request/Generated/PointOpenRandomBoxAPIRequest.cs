//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
抽選BOXを開封する

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class PointOpenRandomBoxAPIPost : AppAPIPostBase {
		public long mPointId = 0; // 開封するポイントのマスタID
		public long count = 0; // 開封する数量。最大100

   }

   [Serializable]
   public class PointOpenRandomBoxAPIResponse : AppAPIResponseBase {

   }
      
   public partial class PointOpenRandomBoxAPIRequest : AppAPIRequestBase<PointOpenRandomBoxAPIPost, PointOpenRandomBoxAPIResponse> {
      public override string apiName{get{ return "point/openRandomBox"; } }
   }
}