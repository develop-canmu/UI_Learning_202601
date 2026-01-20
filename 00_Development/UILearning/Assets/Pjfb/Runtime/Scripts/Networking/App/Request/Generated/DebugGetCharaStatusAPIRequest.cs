//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
キャラ・レベル・潜在値を指定して、計算済みキャラステータスを得る

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class DebugGetCharaStatusAPIPost : AppAPIPostBase {
		public long mCharaId = 0; // キャラID
		public long level = 0; // レベル
		public long newLiberationLevel = 0; // 潜在レベル

   }

   [Serializable]
   public class DebugGetCharaStatusAPIResponse : AppAPIResponseBase {
		public object[] debugMap = null; // 計算過程
		public CharaV2BattleCacheStatus charaStatus = null; // キャラ能力

   }
      
   public partial class DebugGetCharaStatusAPIRequest : AppAPIRequestBase<DebugGetCharaStatusAPIPost, DebugGetCharaStatusAPIResponse> {
      public override string apiName{get{ return "debug/getCharaStatus"; } }
   }
}