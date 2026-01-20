//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
スタミナ回復操作

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class StaminaCureAPIPost : AppAPIPostBase {
		public long mStaminaCureId = 0; // スタミナ回復マスタID
		public long value = 0; // 何口分実行を行うか

   }

   [Serializable]
   public class StaminaCureAPIResponse : AppAPIResponseBase {
		public StaminaBase[] staminaList = null; // ユーザー所持スタミナ一覧

   }
      
   public partial class StaminaCureAPIRequest : AppAPIRequestBase<StaminaCureAPIPost, StaminaCureAPIResponse> {
      public override string apiName{get{ return "stamina/cure"; } }
   }
}