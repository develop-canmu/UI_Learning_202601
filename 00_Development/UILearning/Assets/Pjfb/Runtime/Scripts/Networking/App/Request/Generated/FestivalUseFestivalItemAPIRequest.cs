//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
イベントアイテムの使用

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class FestivalUseFestivalItemAPIPost : AppAPIPostBase {
		public long mFestivalItemId = 0; // イベントアイテム効果マスタID
		public long value = 0; // 何口分実行を行うか

   }

   [Serializable]
   public class FestivalUseFestivalItemAPIResponse : AppAPIResponseBase {
		public FestivalEffectStatus effectStatus = null; // トレーニング特殊効果情報

   }
      
   public partial class FestivalUseFestivalItemAPIRequest : AppAPIRequestBase<FestivalUseFestivalItemAPIPost, FestivalUseFestivalItemAPIResponse> {
      public override string apiName{get{ return "festival/useFestivalItem"; } }
   }
}