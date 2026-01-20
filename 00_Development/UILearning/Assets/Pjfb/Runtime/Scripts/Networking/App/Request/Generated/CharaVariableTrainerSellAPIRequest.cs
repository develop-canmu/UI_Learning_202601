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
   public class CharaVariableTrainerSellAPIPost : AppAPIPostBase {
		public long[] idList = null; // 売却対象キャラID

   }

   [Serializable]
   public class CharaVariableTrainerSellAPIResponse : AppAPIResponseBase {
		public long[] deleteCharaIdList = null; // 削除済みキャラID一覧

   }
      
   public partial class CharaVariableTrainerSellAPIRequest : AppAPIRequestBase<CharaVariableTrainerSellAPIPost, CharaVariableTrainerSellAPIResponse> {
      public override string apiName{get{ return "chara-variable-trainer/sell"; } }
   }
}