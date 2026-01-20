//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
キャラ一覧の取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CharaVariableGetListAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class CharaVariableGetListAPIResponse : AppAPIResponseBase {
		public CharaV2Base[] charaList = null; // キャラ一覧

   }
      
   public partial class CharaVariableGetListAPIRequest : AppAPIRequestBase<CharaVariableGetListAPIPost, CharaVariableGetListAPIResponse> {
      public override string apiName{get{ return "chara-variable/getList"; } }
   }
}