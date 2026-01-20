//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
スキルとスキル効果の紐付け一覧を得る

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugGetAbilityCombinationAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class DebugGetAbilityCombinationAPIResponse : AppAPIResponseBase {
		public DebugAbilityCombination[] abilityCombinationList = null; // スキルとスキル効果の紐付け一覧

   }
      
   public partial class DebugGetAbilityCombinationAPIRequest : AppAPIRequestBase<DebugGetAbilityCombinationAPIPost, DebugGetAbilityCombinationAPIResponse> {
      public override string apiName{get{ return "debug/getAbilityCombination"; } }
   }
}