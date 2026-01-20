//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
キャラの育成操作

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CombinationProgressAPIPost : AppAPIPostBase {
		public long mCombinationId = 0; // 発動させたいcombinationマスタのID

   }

   [Serializable]
   public class CombinationProgressAPIResponse : AppAPIResponseBase {
		public CombinationOpenedMinimum[] openedList = null; // 今回更新が生じた開放済みコンビネーション情報一覧
		public CombinationStatusTrainingBase statusTrainingBase = null; // 実施後のトレーニング補正値

   }
      
   public partial class CombinationProgressAPIRequest : AppAPIRequestBase<CombinationProgressAPIPost, CombinationProgressAPIResponse> {
      public override string apiName{get{ return "combination/progress"; } }
   }
}