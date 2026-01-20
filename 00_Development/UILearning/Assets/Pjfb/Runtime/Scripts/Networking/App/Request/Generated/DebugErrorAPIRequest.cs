//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
APIのエラーのテスト

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugErrorAPIPost : AppAPIPostBase {
		public string statusCode = ""; // 返して欲しいステータスコード
		public string errorMessage = ""; // 返して欲しいエラーメッセージ

   }

   [Serializable]
   public class DebugErrorAPIResponse : AppAPIResponseBase {

   }
      
   public partial class DebugErrorAPIRequest : AppAPIRequestBase<DebugErrorAPIPost, DebugErrorAPIResponse> {
      public override string apiName{get{ return "debug/error"; } }
   }
}