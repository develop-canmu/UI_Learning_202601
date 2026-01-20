//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ユーザ初期設定

ユーザ作成後にユーザ名や性別を入力することになるタイトルでのみ使用します。
詳しくは native-api/user/create の説明をご参照ください。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class UserInitializeAPIPost : AppAPIPostBase {
		public string name = ""; // ユーザ名
		public long gender = 0; // 性別（1: 男性, 2: 女性）
		public long mIconId = 0; // アイコンID（ユーザアイコンの概念が存在しないタイトルでは 1 を指定する）

   }

   [Serializable]
   public class UserInitializeAPIResponse : AppAPIResponseBase {
		public string name = ""; // DBに保存されたユーザ名
		public long gender = 0; // DBに保存された性別
		public long mIconId = 0; // DBに保存されたアイコンID

   }
      
   public partial class UserInitializeAPIRequest : AppAPIRequestBase<UserInitializeAPIPost, UserInitializeAPIResponse> {
      public override string apiName{get{ return "user/initialize"; } }
   }
}