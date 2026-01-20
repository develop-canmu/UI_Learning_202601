//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
広告報酬用のワンタイムトークン発行

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class AdRewardGenerateOneTimeTokenAPIPost : AppAPIPostBase {
		public long mAdRewardId = 0; // mAdRewardのid

   }

   [Serializable]
   public class AdRewardGenerateOneTimeTokenAPIResponse : AppAPIResponseBase {
		public string oneTimeToken = ""; // ワンタイムトークン情報

   }
      
   public partial class AdRewardGenerateOneTimeTokenAPIRequest : AppAPIRequestBase<AdRewardGenerateOneTimeTokenAPIPost, AdRewardGenerateOneTimeTokenAPIResponse> {
      public override string apiName{get{ return "ad-reward/generateOneTimeToken"; } }
   }
}