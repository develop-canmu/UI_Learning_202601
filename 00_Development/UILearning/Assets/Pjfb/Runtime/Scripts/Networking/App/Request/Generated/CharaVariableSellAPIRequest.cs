//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
売却

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CharaVariableSellAPIPost : AppAPIPostBase {
		public long[] idList = null; // 売却対象キャラID

   }

   [Serializable]
   public class CharaVariableSellAPIResponse : AppAPIResponseBase {
		public long[] deleteCharaIdList = null; // 削除済みキャラID一蘭

   }
      
   public partial class CharaVariableSellAPIRequest : AppAPIRequestBase<CharaVariableSellAPIPost, CharaVariableSellAPIResponse> {
      public override string apiName{get{ return "chara-variable/sell"; } }
   }
}