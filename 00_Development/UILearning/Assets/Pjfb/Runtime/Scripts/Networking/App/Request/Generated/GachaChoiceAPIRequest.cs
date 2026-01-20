//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
セルフピックアップの設定実行

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GachaChoiceAPIPost : AppAPIPostBase {
		public long mGachaChoiceId = 0; // ガチャ設定ID
		public ChoiceUserChoice[] userChoiceList = null; // どの枠に対してどういう選択をしたか？の送信情報

   }

   [Serializable]
   public class GachaChoiceAPIResponse : AppAPIResponseBase {
		public ChoiceUserChoice[] userChoiceList = null; // ピックアップ選択のユーザーが実際に選択しているもの
		public bool isSelected = false; // ピックアップのセレクトが完了し、ガチャが実行可能な状態かどうか

   }
      
   public partial class GachaChoiceAPIRequest : AppAPIRequestBase<GachaChoiceAPIPost, GachaChoiceAPIResponse> {
      public override string apiName{get{ return "gacha/choice"; } }
   }
}