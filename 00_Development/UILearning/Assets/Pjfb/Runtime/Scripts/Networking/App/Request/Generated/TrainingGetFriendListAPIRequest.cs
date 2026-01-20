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
   public class TrainingGetFriendListAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class TrainingGetFriendListAPIResponse : AppAPIResponseBase {
		public CharaV2FriendLend[] friendCharaList = null; // フレンドキャラ一覧

   }
      
   public partial class TrainingGetFriendListAPIRequest : AppAPIRequestBase<TrainingGetFriendListAPIPost, TrainingGetFriendListAPIResponse> {
      public override string apiName{get{ return "training/getFriendList"; } }
   }
}