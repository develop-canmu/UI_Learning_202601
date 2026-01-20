//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
狩猟ユーザー情報の取得（最終、ログインAPI等にマージ）

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class HuntGetDataAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class HuntGetDataAPIResponse : AppAPIResponseBase {
		public HuntUserPending huntPending = null; // 狩猟のユーザー保留情報
		public HuntResultStatus[] huntResultList = null; // 狩猟のユーザー結果情報

   }
      
   public partial class HuntGetDataAPIRequest : AppAPIRequestBase<HuntGetDataAPIPost, HuntGetDataAPIResponse> {
      public override string apiName{get{ return "hunt/getData"; } }
   }
}