//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
エール受け取り履歴取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CommunityGetYellListAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class CommunityGetYellListAPIResponse : AppAPIResponseBase {
		public ModelsUYell[] uYellList = null; // エール受け取り履歴リスト
		public UserChatUserStatus[] chatUserStatusList = null; // ユーザー情報リスト

   }
      
   public partial class CommunityGetYellListAPIRequest : AppAPIRequestBase<CommunityGetYellListAPIPost, CommunityGetYellListAPIResponse> {
      public override string apiName{get{ return "community/getYellList"; } }
   }
}