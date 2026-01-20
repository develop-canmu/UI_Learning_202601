//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
指名報酬選択

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class HuntChoicePrizeAPIPost : AppAPIPostBase {
		public long choiceNumber = 0; // 選択報酬番号

   }

   [Serializable]
   public class HuntChoicePrizeAPIResponse : AppAPIResponseBase {
		public HuntPrizeSet[] prizeSetList = null; // 報酬情報（報酬区分ごとに振り分け済み）
		public HuntPrizeCorrection[] prizeCorrectionList = null; // 報酬獲得量補正情報
		public HuntUserPending huntPending = null; // 狩猟のユーザー保留情報

   }
      
   public partial class HuntChoicePrizeAPIRequest : AppAPIRequestBase<HuntChoicePrizeAPIPost, HuntChoicePrizeAPIResponse> {
      public override string apiName{get{ return "hunt/choicePrize"; } }
   }
}