//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ギフトコードの使用

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GiftCodeUseAPIPost : AppAPIPostBase {
		public string friendCode = ""; // フレンドコード
		public string giftCode = ""; // ギフトコード

   }

   [Serializable]
   public class GiftCodeUseAPIResponse : AppAPIResponseBase {
		public string lockEndAt = ""; // 入力制限が解除される日時（ステータスコードが 2002 の場合のみ）

   }
      
   public partial class GiftCodeUseAPIRequest : AppAPIRequestBase<GiftCodeUseAPIPost, GiftCodeUseAPIResponse> {
      public override string apiName{get{ return "gift-code/use"; } }
   }
}