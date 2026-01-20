//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
キャラを指定し、対象キャラの信頼度経験値を得る（デバッグ用です）

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CharaLibraryTrainingTrustExpTestAPIPost : AppAPIPostBase {
		public long mCharaId = 0; // キャラID
		public long[] mCharaIdSubList = null; // キャラID

   }

   [Serializable]
   public class CharaLibraryTrainingTrustExpTestAPIResponse : AppAPIResponseBase {
		public EnhanceLevelProgress[] levelProgressList = null; // 信頼度レベルの変動を得る。idは「親キャラID」

   }
      
   public partial class CharaLibraryTrainingTrustExpTestAPIRequest : AppAPIRequestBase<CharaLibraryTrainingTrustExpTestAPIPost, CharaLibraryTrainingTrustExpTestAPIResponse> {
      public override string apiName{get{ return "chara-library/trainingTrustExpTest"; } }
   }
}