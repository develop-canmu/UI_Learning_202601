//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
注目キャラクター情報取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CharaVariableGetRecommendCharaListAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class CharaVariableGetRecommendCharaListAPIResponse : AppAPIResponseBase {
		public CharaVariableRecommendStatus[] allList = null; // 全体の注目キャラクター情報リスト
		public CharaVariableRecommendStatus[] guildList = null; // ギルドの注目キャラクター情報リスト

   }
      
   public partial class CharaVariableGetRecommendCharaListAPIRequest : AppAPIRequestBase<CharaVariableGetRecommendCharaListAPIPost, CharaVariableGetRecommendCharaListAPIResponse> {
      public override string apiName{get{ return "chara-variable/getRecommendCharaList"; } }
   }
}