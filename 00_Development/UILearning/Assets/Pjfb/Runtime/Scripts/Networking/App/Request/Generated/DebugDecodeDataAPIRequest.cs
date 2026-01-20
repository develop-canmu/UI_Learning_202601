//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
復号化テスト

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugDecodeDataAPIPost : AppAPIPostBase {
		public string encodeData = ""; // エンコード後のデータ
		public string ivData = ""; // エンコードに対する鍵

   }

   [Serializable]
   public class DebugDecodeDataAPIResponse : AppAPIResponseBase {
		public object[] decodeData = null; // 複合化後のデータ

   }
      
   public partial class DebugDecodeDataAPIRequest : AppAPIRequestBase<DebugDecodeDataAPIPost, DebugDecodeDataAPIResponse> {
      public override string apiName{get{ return "debug/decodeData"; } }
   }
}