//
// This file is auto-generated
//

using System;
using Pjfb.Networking.API;

/*
マスタのダウンロード実行

*/
namespace Pjfb.Networking.App.Request {

   
   [Serializable]
   public class MasterDownloadExecuteAPIPost : AppAPIPostBase {
		public long currentMasterVer = 0; // クライアント側のマスタバージョン（デフォルト値: 0）

   }

   [Serializable]
   public class MasterDownloadExecuteAPIResponse : AppAPIResponseBase {
		public long serverMasterVer = 0; // サーバー側のマスタバージョン
		public NativeMasterMasterDataPack[] masterDataPackList = null; // マスタ名と配列を対応させたリスト

   }
      
   public partial class MasterDownloadExecuteAPIRequest : AppAPIRequestBase<MasterDownloadExecuteAPIPost, MasterDownloadExecuteAPIResponse> {
      public override string apiName{get{ return "master-download/execute"; } }
   }
}