//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
解放済みのコンビネーションをすべて取得

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class CombinationGetListOpenedAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class CombinationGetListOpenedAPIResponse : AppAPIResponseBase {
		public CombinationOpenedMinimum[] openedList = null; // 開放済みコンビネーション情報一覧

   }
      
   public partial class CombinationGetListOpenedAPIRequest : AppAPIRequestBase<CombinationGetListOpenedAPIPost, CombinationGetListOpenedAPIResponse> {
      public override string apiName{get{ return "combination/getListOpened"; } }
   }
}