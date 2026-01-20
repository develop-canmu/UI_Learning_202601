//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
機能解放演出閲覧フラグ更新

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class UserUpdateViewedSystemEffectAPIPost : AppAPIPostBase {
		public long systemNumber = 0; // 閲覧した機能解放番号

   }

   [Serializable]
   public class UserUpdateViewedSystemEffectAPIResponse : AppAPIResponseBase {

   }
      
   public partial class UserUpdateViewedSystemEffectAPIRequest : AppAPIRequestBase<UserUpdateViewedSystemEffectAPIPost, UserUpdateViewedSystemEffectAPIResponse> {
      public override string apiName{get{ return "user/updateViewedSystemEffect"; } }
   }
}