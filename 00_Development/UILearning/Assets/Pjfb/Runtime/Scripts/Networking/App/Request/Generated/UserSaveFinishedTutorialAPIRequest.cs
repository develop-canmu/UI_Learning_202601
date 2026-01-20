//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
終了したチュートリアルの番号を保存する

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class UserSaveFinishedTutorialAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class UserSaveFinishedTutorialAPIResponse : AppAPIResponseBase {

   }
      
   public partial class UserSaveFinishedTutorialAPIRequest : AppAPIRequestBase<UserSaveFinishedTutorialAPIPost, UserSaveFinishedTutorialAPIResponse> {
      public override string apiName{get{ return "user/saveFinishedTutorial"; } }
   }
}