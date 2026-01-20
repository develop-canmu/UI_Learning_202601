//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
lambdaバトルロジック連携関連

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DummyLambdaBattleLogicAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class DummyLambdaBattleLogicAPIResponse : AppAPIResponseBase {
		public LambdaResult result = null; // Lambdaでバトルロジックを起動した際に得られる、結果情報
		public LambdaSummary summary = null; // Lambdaでバトルロジックを起動した際に得られる、要約情報

   }
      
   public partial class DummyLambdaBattleLogicAPIRequest : AppAPIRequestBase<DummyLambdaBattleLogicAPIPost, DummyLambdaBattleLogicAPIResponse> {
      public override string apiName{get{ return "dummy/lambdaBattleLogic"; } }
   }
}