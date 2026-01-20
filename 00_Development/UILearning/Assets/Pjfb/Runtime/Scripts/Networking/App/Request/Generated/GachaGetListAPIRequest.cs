//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
一覧の取得
保留情報がある場合のみ、ガチャ保留情報が返却されます。

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class GachaGetListAPIPost : AppAPIPostBase {

   }

   [Serializable]
   public class GachaGetListAPIResponse : AppAPIResponseBase {
		public GachaPendingInfo pendingInfo = null; // （保留発生の場合のみ）ガチャ保留情報
		public TopSetting[] settingList = null; // ガチャ設定一覧
		public bool hasFreeGacha = false; // 無料で引けるガチャがあれば真

   }
      
   public partial class GachaGetListAPIRequest : AppAPIRequestBase<GachaGetListAPIPost, GachaGetListAPIResponse> {
      public override string apiName{get{ return "gacha/getList"; } }
   }
}