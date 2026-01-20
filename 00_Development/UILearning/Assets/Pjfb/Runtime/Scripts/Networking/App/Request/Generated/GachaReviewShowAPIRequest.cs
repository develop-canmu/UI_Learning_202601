//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
レビュー既読操作（ユーザーがレビュー操作をしてくれた or レビュー促しを止めるボタンを押した場合に実行）

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GachaReviewShowAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class GachaReviewShowAPIResponse : AppAPIResponseBase {

   }
      
   public partial class GachaReviewShowAPIRequest : AppAPIRequestBase<GachaReviewShowAPIPost, GachaReviewShowAPIResponse> {
      public override string apiName{get{ return "gacha/reviewShow"; } }
   }
}