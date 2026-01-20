//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
セルフピックアップの表（マスタデータ部分）の取得を行う

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GachaGetChoiceDetailAPIPost : AppAPIPostBase {
		public long mGachaChoiceId = 0; // ガチャ設定ID

   }

   [Serializable]
   public class GachaGetChoiceDetailAPIResponse : AppAPIResponseBase {
		public MasterDetailDetail masterChoiceDetail = null; // ピックアップ選択表（マスタ部分）
		public ChoiceUserChoice[] userChoiceList = null; // ピックアップ選択のユーザーが実際に選択しているもの

   }
      
   public partial class GachaGetChoiceDetailAPIRequest : AppAPIRequestBase<GachaGetChoiceDetailAPIPost, GachaGetChoiceDetailAPIResponse> {
      public override string apiName{get{ return "gacha/getChoiceDetail"; } }
   }
}