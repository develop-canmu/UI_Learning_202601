//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
提供割合取得。
ピックアップ設定が有効なガチャの場合、ピックアップ込での確率表記が行われます。
ピックアップ設定が不完全な場合、ピックアップがなかった場合の確率表記が帰ります。
ボックスガチャの場合は提供割合の表示をしないので当該APIは使用しません。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GachaProbabilityDetailAPIPost : AppAPIPostBase {
		public long mGachaSettingId = 0; // ガチャ設定ID

   }

   [Serializable]
   public class GachaProbabilityDetailAPIResponse : AppAPIResponseBase {
		public ProbabilityInfo probabilityInfo = null; // ガチャ提供割合情報

   }
      
   public partial class GachaProbabilityDetailAPIRequest : AppAPIRequestBase<GachaProbabilityDetailAPIPost, GachaProbabilityDetailAPIResponse> {
      public override string apiName{get{ return "gacha/probabilityDetail"; } }
   }
}