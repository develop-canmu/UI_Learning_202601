//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
可変キャラロック処理

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CharaVariableLockAPIPost : AppAPIPostBase {
		public long id = 0; // ロック対象キャラID
		public long onFlg = 0; // ON/OFF(0 => OFF, 1 => ON)
		public long[] idList = null; // ロック対象キャラID

   }

   [Serializable]
   public class CharaVariableLockAPIResponse : AppAPIResponseBase {

   }
      
   public partial class CharaVariableLockAPIRequest : AppAPIRequestBase<CharaVariableLockAPIPost, CharaVariableLockAPIResponse> {
      public override string apiName{get{ return "chara-variable/lock"; } }
   }
}