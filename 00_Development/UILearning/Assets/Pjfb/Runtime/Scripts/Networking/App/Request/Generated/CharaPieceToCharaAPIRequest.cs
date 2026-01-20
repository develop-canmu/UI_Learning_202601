//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ピース・キャラ変換

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CharaPieceToCharaAPIPost : AppAPIPostBase {
		public long mCharaId = 0; // 獲得を行いたいキャラのマスタID

   }

   [Serializable]
   public class CharaPieceToCharaAPIResponse : AppAPIResponseBase {

   }
      
   public partial class CharaPieceToCharaAPIRequest : AppAPIRequestBase<CharaPieceToCharaAPIPost, CharaPieceToCharaAPIResponse> {
      public override string apiName{get{ return "chara/pieceToChara"; } }
   }
}