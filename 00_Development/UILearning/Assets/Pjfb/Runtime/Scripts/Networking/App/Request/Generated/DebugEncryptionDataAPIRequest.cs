//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
暗号化テスト

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugEncryptionDataAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class DebugEncryptionDataAPIResponse : AppAPIResponseBase {
		public string data = ""; // エンコード後のデータ
		public string iv = ""; // エンコードに対する鍵

   }
      
   public partial class DebugEncryptionDataAPIRequest : AppAPIRequestBase<DebugEncryptionDataAPIPost, DebugEncryptionDataAPIResponse> {
      public override string apiName{get{ return "debug/encryptionData"; } }
   }
}