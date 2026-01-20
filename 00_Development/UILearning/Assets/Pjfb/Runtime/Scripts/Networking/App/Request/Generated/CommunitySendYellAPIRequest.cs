//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
エール送信。targetUMasterIdを指定した場合は単体ユーザーへのエール、categoryは一括送信の場合に設定

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CommunitySendYellAPIPost : AppAPIPostBase {
		public long targetUMasterId = 0; // 選択したチャットの送信相手
		public long category = 0; // 送信先のカテゴリ。1:フォロー中、2:フォロワー、3:ギルド

   }

   [Serializable]
   public class CommunitySendYellAPIResponse : AppAPIResponseBase {
		public YellSendYellResult yellResult = null; // エール送信結果

   }
      
   public partial class CommunitySendYellAPIRequest : AppAPIRequestBase<CommunitySendYellAPIPost, CommunitySendYellAPIResponse> {
      public override string apiName{get{ return "community/sendYell"; } }
   }
}