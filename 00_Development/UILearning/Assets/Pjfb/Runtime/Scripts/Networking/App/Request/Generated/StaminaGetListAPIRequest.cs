//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
スタミナ一覧の取得（itemContainer経由での処理に内包）

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class StaminaGetListAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class StaminaGetListAPIResponse : AppAPIResponseBase {
		public StaminaBase[] staminaList = null; // ユーザー所持スタミナ一覧

   }
      
   public partial class StaminaGetListAPIRequest : AppAPIRequestBase<StaminaGetListAPIPost, StaminaGetListAPIResponse> {
      public override string apiName{get{ return "stamina/getList"; } }
   }
}