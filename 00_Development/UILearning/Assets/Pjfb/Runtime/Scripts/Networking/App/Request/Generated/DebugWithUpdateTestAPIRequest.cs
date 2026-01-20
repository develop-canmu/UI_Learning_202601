//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
withUpdate が指定されているAPIのテスト

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugWithUpdateTestAPIPost : AppAPIPostBase {
		public long sleepSeconds = 0; // 排他ロックをかけてから待機する秒数（排他ロックを持続する時間）
		public string throwsException = ""; // 待機終了直後に例外を投げるなら真（例外発生時にちゃんとロックが解除されるかの確認用）

   }

   [Serializable]
   public class DebugWithUpdateTestAPIResponse : AppAPIResponseBase {

   }
      
   public partial class DebugWithUpdateTestAPIRequest : AppAPIRequestBase<DebugWithUpdateTestAPIPost, DebugWithUpdateTestAPIResponse> {
      public override string apiName{get{ return "debug/withUpdateTest"; } }
   }
}