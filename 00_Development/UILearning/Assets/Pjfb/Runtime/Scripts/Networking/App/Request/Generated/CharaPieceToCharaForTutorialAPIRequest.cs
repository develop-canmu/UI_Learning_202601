//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
ピース・キャラ変換（チュートリアル用）

実行する処理自体は native-api/chara/pieceToChara と同じですが、
本APIはアセット／マスタデータのバージョンのチェックを行いません。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CharaPieceToCharaForTutorialAPIPost : AppAPIPostBase {
		public long mCharaId = 0; // 獲得を行いたいキャラのマスタID

   }

   [Serializable]
   public class CharaPieceToCharaForTutorialAPIResponse : AppAPIResponseBase {

   }
      
   public partial class CharaPieceToCharaForTutorialAPIRequest : AppAPIRequestBase<CharaPieceToCharaForTutorialAPIPost, CharaPieceToCharaForTutorialAPIResponse> {
      public override string apiName{get{ return "chara/pieceToCharaForTutorial"; } }
   }
}