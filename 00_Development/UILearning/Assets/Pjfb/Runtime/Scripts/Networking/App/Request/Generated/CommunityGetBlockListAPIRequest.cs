//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ブロックしているユーザーの一覧を取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CommunityGetBlockListAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class CommunityGetBlockListAPIResponse : AppAPIResponseBase {
		public UserCommunityUserStatus[] communityUserStatusList = null; // ユーザー情報リスト

   }
      
   public partial class CommunityGetBlockListAPIRequest : AppAPIRequestBase<CommunityGetBlockListAPIPost, CommunityGetBlockListAPIResponse> {
      public override string apiName{get{ return "community/getBlockList"; } }
   }
}