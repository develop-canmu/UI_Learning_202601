//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ユーザ名のバリデーション

native-api/user/updateName 内でも同様のバリデーションが実行されます。
当該APIはユーザ名の変更は行わずにユーザ名の検証だけを行いたい場合に使用します。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class UserValidateNameAPIPost : AppAPIPostBase {
		public string name = ""; // 変更後のユーザ名

   }

   [Serializable]
   public class UserValidateNameAPIResponse : AppAPIResponseBase {

   }
      
   public partial class UserValidateNameAPIRequest : AppAPIRequestBase<UserValidateNameAPIPost, UserValidateNameAPIResponse> {
      public override string apiName{get{ return "user/validateName"; } }
   }
}