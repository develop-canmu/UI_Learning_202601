//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
キャラを指定し、対象キャラの信頼度経験値を得る

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugTrainingTrustExpTestAPIPost : AppAPIPostBase {
		public long mCharaId = 0; // キャラID
		public long[] mCharaIdSubList = null; // キャラID

   }

   [Serializable]
   public class DebugTrainingTrustExpTestAPIResponse : AppAPIResponseBase {
		public EnhanceLevelProgress[] levelProgressList = null; // 信頼度レベルの変動を得る。idは「親キャラID」

   }
      
   public partial class DebugTrainingTrustExpTestAPIRequest : AppAPIRequestBase<DebugTrainingTrustExpTestAPIPost, DebugTrainingTrustExpTestAPIResponse> {
      public override string apiName{get{ return "debug/trainingTrustExpTest"; } }
   }
}