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
   public class CharaGetListAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class CharaGetListAPIResponse : AppAPIResponseBase {
		public CharaV2Base[] charaList = null; // キャラ一覧

   }
      
   public partial class CharaGetListAPIRequest : AppAPIRequestBase<CharaGetListAPIPost, CharaGetListAPIResponse> {
      public override string apiName{get{ return "chara/getList"; } }
   }
}