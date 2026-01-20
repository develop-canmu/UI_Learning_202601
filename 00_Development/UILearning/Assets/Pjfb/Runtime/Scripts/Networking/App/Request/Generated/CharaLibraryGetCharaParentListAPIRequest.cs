//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
親キャラ情報の一覧を得る
キャラ図鑑のトップ等にアクセスしたときに、取りに行くイメージ。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CharaLibraryGetCharaParentListAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class CharaLibraryGetCharaParentListAPIResponse : AppAPIResponseBase {
		public CharaParentBase[] charaParentList = null; // 親キャラ情報を得る

   }
      
   public partial class CharaLibraryGetCharaParentListAPIRequest : AppAPIRequestBase<CharaLibraryGetCharaParentListAPIPost, CharaLibraryGetCharaParentListAPIResponse> {
      public override string apiName{get{ return "chara-library/getCharaParentList"; } }
   }
}