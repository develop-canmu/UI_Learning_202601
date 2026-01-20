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
   public class CharaVariableTrainerGetListAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class CharaVariableTrainerGetListAPIResponse : AppAPIResponseBase {
		public CharaVariableTrainerBase[] charaVariableTrainerList = null; // トレーニング補助キャラ一覧

   }
      
   public partial class CharaVariableTrainerGetListAPIRequest : AppAPIRequestBase<CharaVariableTrainerGetListAPIPost, CharaVariableTrainerGetListAPIResponse> {
      public override string apiName{get{ return "chara-variable-trainer/getList"; } }
   }
}