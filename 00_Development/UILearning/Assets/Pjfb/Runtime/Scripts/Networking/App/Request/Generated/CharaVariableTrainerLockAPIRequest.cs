//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
トレーニング補助キャラロック処理

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CharaVariableTrainerLockAPIPost : AppAPIPostBase {
		public long id = 0; // ロック対象キャラID
		public long onFlg = 0; // ON/OFF(0 => OFF, 1 => ON)

   }

   [Serializable]
   public class CharaVariableTrainerLockAPIResponse : AppAPIResponseBase {

   }
      
   public partial class CharaVariableTrainerLockAPIRequest : AppAPIRequestBase<CharaVariableTrainerLockAPIPost, CharaVariableTrainerLockAPIResponse> {
      public override string apiName{get{ return "chara-variable-trainer/lock"; } }
   }
}